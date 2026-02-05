using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Linq2OData.Core.Expressions
{
    /// <summary>
    /// Used to convert Expressions to OData orderby strings.
    /// </summary>
    public sealed class ODataOrderByVisitor : ExpressionVisitor
    {
        private StringBuilder sb = new();

        /// <summary>
        /// Converts an expression into an OData orderby property name.
        /// </summary>
        /// <typeparam name="T">Type of the entity to create the orderby from.</typeparam>
        /// <typeparam name="TProperty">Type of the property being ordered.</typeparam>
        /// <param name="expression">Expression to be converted to OData orderby.</param>
        /// <returns>Converted OData orderby property name.</returns>
        public string ToOrderBy<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            sb = new StringBuilder();
            Visit(expression);
            return sb.ToString();
        }

        /// <summary>
        /// Appends another orderby clause.
        /// </summary>
        /// <param name="currentOrderBy">The current orderby string.</param>
        /// <param name="propertyName">The property name to append.</param>
        /// <param name="descending">Whether to order descending.</param>
        /// <returns>The combined orderby string.</returns>
        public string AppendOrderBy(string currentOrderBy, string propertyName, bool descending = false)
        {
            var orderByClause = descending ? $"{propertyName} desc" : propertyName;

            if (string.IsNullOrEmpty(currentOrderBy))
                return orderByClause;

            return $"{currentOrderBy},{orderByClause}";
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Expression?.NodeType == ExpressionType.Parameter)
            {
                sb.Append(m.Member.Name);
                return m;
            }
            else if (m.Expression?.NodeType == ExpressionType.MemberAccess)
            {
                Expression expression = m;
                var memberAccessNames = new List<string>();

                // Recursively get all access members
                while (expression?.NodeType == ExpressionType.MemberAccess && expression is MemberExpression memberExpression)
                {
                    var propertyAccessName = memberExpression.Member.Name;
                    memberAccessNames.Add(propertyAccessName);

                    if (memberExpression.Expression == null) break;

                    expression = memberExpression.Expression;
                }

                // Reverse the order so it's in the order of the code
                // e.g. Expression = x => x.Address.City
                // Original = City, Address
                // Reversed = Address, City
                memberAccessNames.Reverse();

                if (expression is ParameterExpression)
                {
                    // Member access from parameter
                    // e.g. x => x.Address.City
                    // Resulting OData = Address/City
                    sb.Append(string.Join("/", memberAccessNames));
                    return m;
                }
            }

            throw new NotSupportedException($"The member '{m.Member.Name}' is not supported in orderby expressions");
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Convert:
                    Visit(u.Operand);
                    break;
                default:
                    throw new NotSupportedException($"The unary operator '{u.NodeType}' is not supported in orderby expressions");
            }
            return u;
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            return Visit(node.Body);
        }
    }
}
