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



        public string GetOnlyExpand(ODataVersion oDataVersion)
        {
            if (!IsRoot)
                return "";

            return BuildExpandForChildren(oDataVersion, includeSelect: false);
        }

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

                expand = BuildExpandForChildren(oDataVersion, includeSelect: true);
            }

            return (select, expand);
        }

        private string BuildExpandForChildren(ODataVersion oDataVersion, bool includeSelect)
        {
            var expandParts = new List<string>();
            foreach (var child in Children.Where(c => c.IsComplex))
            {
                var childExpand = child.BuildExpand(oDataVersion, includeSelect);
                if (!string.IsNullOrEmpty(childExpand))
                {
                    expandParts.Add(childExpand);
                }
            }
            return string.Join(",", expandParts);
        }

        private string BuildExpand(ODataVersion oDataVersion, bool includeSelect)
        {
            if (string.IsNullOrEmpty(Name))
                return "";

            var complexChildren = Children.Where(c => c.IsComplex).ToList();

            if (oDataVersion == ODataVersion.V4)
            {
                return BuildExpandV4(includeSelect, complexChildren);
            }
            else
            {
                return BuildExpandV2V3(complexChildren);
            }
        }

        private string BuildExpandV4(bool includeSelect, List<QueryNode> complexChildren)
        {
            var parts = new List<string>();

            if (includeSelect)
            {
                var selectedProps = SelectedProperties;
                if (!string.IsNullOrEmpty(selectedProps))
                {
                    parts.Add($"$select={selectedProps}");
                }
            }

            if (complexChildren.Any())
            {
                var nestedExpands = complexChildren
                    .Select(c => c.BuildExpand(ODataVersion.V4, includeSelect))
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

            return Name;
        }

        private string BuildExpandV2V3(List<QueryNode> complexChildren)
        {
            // OData v2/v3: no nested select, only slash-separated expand paths
            if (complexChildren.Any())
            {
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

        private string BuildExpandPath()
        {
            if (string.IsNullOrEmpty(Name))
                return "";

            var complexChildren = Children.Where(c => c.IsComplex).ToList();

            // For a linear chain (single complex child), build the nested path
            if (complexChildren.Count == 1)
            {
                var childPath = complexChildren[0].BuildExpandPath();
                if (!string.IsNullOrEmpty(childPath))
                {
                    return $"{Name}/{childPath}";
                }
            }

            // For leaf nodes or nodes with multiple branches, just return the name
            return Name;
        }


        public void AddMergeChildren(QueryNode queryNode)
        {
            foreach (var child in queryNode.Children)
            {
                AddOrMergeChild(child);
            }


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
                {
                    existing.AddOrMergeChild(grandChild);
                }

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
