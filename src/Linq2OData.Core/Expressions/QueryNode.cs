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
                select= SelectedProperties;
            }



            return (select, expand);
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
