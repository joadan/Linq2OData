using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Linq2OData.Core.Expressions
{
    public class QueryNode
    {
        public MemberInfo? Member { get; }
        public ODataMemberAttribute? MemberAttribute { get; }
        public List<QueryNode> Children { get; } = new();
        public List<object?> Arguments { get; } = new();
        public List<string> ParameterNames { get; set; } = [];

        public QueryNode()
        {
            Member = null;
        }

        public QueryNode(MemberInfo member, ODataMemberAttribute attribute)
        {
            Member = member;
            MemberAttribute = attribute;
        }


        public bool IsRoot => Member == null;

        public bool IsComplex => MemberAttribute?.IsComplex == true;
        public string Name => MemberAttribute?.Name ?? "";

        private string SelectedProperties => string.Join(",", Children.Where(e => !e.IsComplex).Select(e => e.Name));

        public (string select, string expand) GetSelectExpand(ODataVersion oDataVersion)
        {
            var select = "";
            var expand = "";

            if (IsRoot)
            {
                if (oDataVersion == ODataVersion.V4)
                {
                    select = SelectedProperties;
                }
                else
                {
                    //In OData v2/v3, we have to explicitly select complex and none at the root level, otherwise they won't be included in the response
                    select = string.Join(",", Children.Select(e => e.Name));
                }
               

                // Build expand string for complex children
                var expandParts = new List<string>();
                foreach (var child in Children.Where(c => c.IsComplex))
                {
                    var childExpand = child.BuildExpand(oDataVersion);
                    if (!string.IsNullOrEmpty(childExpand))
                    {
                        expandParts.Add(childExpand);
                    }
                }
                expand = string.Join(",", expandParts);
            }

            return (select, expand);
        }

        private string BuildExpand(ODataVersion oDataVersion)
        {
            if (string.IsNullOrEmpty(Name))
                return "";

            var selectedProps = SelectedProperties;
            var complexChildren = Children.Where(c => c.IsComplex).ToList();

            if (oDataVersion == ODataVersion.V4)
                {
                    // OData v4: supports nested select and expand within parentheses
                    var parts = new List<string>();

                    if (!string.IsNullOrEmpty(selectedProps))
                    {
                        parts.Add($"$select={selectedProps}");
                    }

                    if (complexChildren.Any())
                    {
                        var nestedExpands = complexChildren
                            .Select(c => c.BuildExpand(oDataVersion))
                            .Where(e => !string.IsNullOrEmpty(e));

                        if (nestedExpands.Any())
                        {
                            parts.Add($"$expand={string.Join(",", nestedExpands)}");
                        }
                    }

                    if (parts.Any())
                    {
                        return $"{Name}({string.Join(";", parts)})";
                    }
                    else
                    {
                        return Name;
                    }
                }
            else
            {
                // OData v2/v3: no nested select, only expand
                if (complexChildren.Any())
                {
                    // Build nested path like "Orders/OrderDetails"
                    var nestedPaths = new List<string>();
                    foreach (var child in complexChildren)
                    {
                        var childPath = child.BuildExpandPath();
                        if (!string.IsNullOrEmpty(childPath))
                        {
                            nestedPaths.Add($"{Name}/{childPath}");
                        }
                    }

                    if (nestedPaths.Any())
                    {
                        return string.Join(",", nestedPaths);
                    }
                }

                return Name;
            }
        }

        private string BuildExpandPath()
        {
            if (string.IsNullOrEmpty(Name))
                return "";

            var complexChildren = Children.Where(c => c.IsComplex).ToList();

            if (complexChildren.Any())
            {
                // Build nested paths
                var paths = new List<string>();
                foreach (var child in complexChildren)
                {
                    var childPath = child.BuildExpandPath();
                    if (!string.IsNullOrEmpty(childPath))
                    {
                        paths.Add($"{Name}/{childPath}");
                    }
                }

                if (paths.Any())
                {
                    return string.Join(",", paths);
                }
            }

            return Name;
        }


       



        public void AddOrMergeChild(QueryNode child)
        {
            if (child == null) return;

            var existing = Children.FirstOrDefault(c =>
                c.Member == child.Member &&
                ArgumentsEqual(c.Arguments, child.Arguments));

            if (existing != null)
            {
                foreach (var grandChild in child.Children)
                    existing.AddOrMergeChild(grandChild);
            }
            else
            {
                Children.Add(child);
            }
        }

        private static bool ArgumentsEqual(List<object?> a1, List<object?> a2)
        {
            if (a1.Count != a2.Count) return false;
            for (int i = 0; i < a1.Count; i++)
            {
                if (!Equals(a1[i], a2[i])) return false;
            }
            return true;
        }
    }
}
