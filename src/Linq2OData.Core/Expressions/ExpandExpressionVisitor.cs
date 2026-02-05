using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Linq2OData.Core.Expressions
{
    /// <summary>
    /// Used to convert Expressions to OData expand strings.
    /// </summary>
    public sealed class ODataExpandVisitor : ExpressionVisitor
    {
        private StringBuilder sb = new();
        private readonly ODataVersion odataVersion;

        public ODataExpandVisitor(ODataVersion odataVersion)
        {
            this.odataVersion = odataVersion;
        }

        /// <summary>
        /// Converts an expression into an OData expand string.
        /// </summary>
        /// <typeparam name="T">Type of the entity to create the expand from.</typeparam>
        /// <typeparam name="TProperty">Type of the property being expanded.</typeparam>
        /// <param name="expression">Expression to be converted to OData expand.</param>
        /// <returns>Converted OData expand string.</returns>
        public string ToExpand<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            sb = new StringBuilder();
            Visit(expression);
            return sb.ToString();
        }

        /// <summary>
        /// Adds a nested expand path.
        /// </summary>
        /// <param name="basePath">The base expand path.</param>
        /// <param name="nestedPath">The nested path to append.</param>
        /// <returns>The combined expand string.</returns>
        public string AppendNestedExpand(string basePath, string nestedPath)
        {
            if (string.IsNullOrEmpty(basePath))
                return nestedPath;

            if (string.IsNullOrEmpty(nestedPath))
                return basePath;

            // OData v2/v3 uses forward slashes for nested expands
            // OData v4 uses parentheses with $expand
            if (odataVersion == ODataVersion.V4)
            {
                // OData v4: Orders($expand=OrderDetails($expand=Product))
                return $"{basePath}($expand={nestedPath})";
            }
            else
            {
                // OData v2/v3: Orders/OrderDetails/Product
                return $"{basePath}/{nestedPath}";
            }
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            // Handle .Select() on collections for nested expands
            // e.g., products.Select(p => p.Category) should become just "Category"
            if (m.Method.Name == "Select" && m.Arguments.Count == 2)
            {
                var lambda = m.Arguments[1];
                if (lambda is UnaryExpression unary && unary.Operand is LambdaExpression lambdaExpr)
                {
                    // Extract the property from the lambda body
                    Visit(lambdaExpr.Body);
                    return m;
                }
                else if (lambda is LambdaExpression lambdaExpression)
                {
                    Visit(lambdaExpression.Body);
                    return m;
                }
            }

            throw new NotSupportedException($"The method '{m.Method.Name}' is not supported in expand expressions");
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
                // e.g. Expression = x => x.Parent.Child
                // Original = Child, Parent
                // Reversed = Parent, Child
                memberAccessNames.Reverse();

                if (expression is ParameterExpression)
                {
                    // Member access from parameter
                    // e.g. x => x.Parent.Child
                    // Resulting OData = Parent/Child
                    sb.Append(string.Join("/", memberAccessNames));
                    return m;
                }
            }

            throw new NotSupportedException($"The member '{m.Member.Name}' is not supported in expand expressions");
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Convert:
                    Visit(u.Operand);
                    break;
                default:
                    throw new NotSupportedException($"The unary operator '{u.NodeType}' is not supported in expand expressions");
            }
            return u;
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            return Visit(node.Body);
        }
    }
}
