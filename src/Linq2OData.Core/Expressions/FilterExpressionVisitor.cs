using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

/// <summary>
/// This code comes from https://github.com/EduVencovsky/Linq2OData/blob/master/Linq2OData.Core/ODataFilterVisitor.cs

namespace Linq2OData.Core.Expressions
{
    /// <summary>
    /// Used convert Expressions to OData.
    /// </summary>
    public sealed class ODataFilterVisitor : ExpressionVisitor
    {
        private StringBuilder sb = new();
        private ODataVersion odataVersion;

        #region Constants
        private static class ODataConstants
        {
            public const string True = "true";

            public const string False = "false";

            public const string And = " and ";

            public const string Or = " or ";

            public const string Equal = " eq ";

            public const string NotEqual = " ne ";

            public const string LessThan = " lt ";

            public const string LessThanOrEqual = " le ";

            public const string GreaterThan = " gt ";

            public const string GreaterThanOrEqual = " ge ";

            public const string Null = "null";

            public const string Not = " NOT ";

            public const string LeftParentheses = "(";

            public const string RightParentheses = ")";
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Converts a expression into a odata filter.
        /// </summary>
        /// <typeparam name="T">Type of the entity to create the filter from.</typeparam>
        /// <param name="expression">Expression to be converted to OData.</param>
        /// <returns>Converted OData.</returns>
        public string ToFilter<T>(Expression<Func<T, bool>> expression, ODataVersion oDataVersion)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            this.odataVersion = oDataVersion;
            sb = new StringBuilder();
            Visit(expression);
            return sb.ToString();
        }
        #endregion

        #region Override        
        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            // Example of how to implement methods call in the future if needed.
            // if (m.Method.Name == "METHOD_NAME")
            // {
            //     // handle odata parsing for METHOD_NAME
            //     // e.g. Count, Contains, EndsWith                
            //     return m;
            // }

            throw new NotSupportedException($"The method '{m.Method.Name}' is not supported");
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Convert:
                    Visit(u.Operand);
                    break;
                default:
                    throw new NotSupportedException($"The unary operator '{u.NodeType}' is not supported");
            }
            return u;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            sb.Append(ODataConstants.LeftParentheses);
            Visit(b.Left);

            switch (b.NodeType)
            {
                case ExpressionType.And:
                    sb.Append(ODataConstants.And);
                    break;
                case ExpressionType.AndAlso:
                    sb.Append(ODataConstants.And);
                    break;
                case ExpressionType.Or:
                    sb.Append(ODataConstants.Or);
                    break;
                case ExpressionType.OrElse:
                    sb.Append(ODataConstants.Or);
                    break;
                case ExpressionType.Equal:
                    sb.Append(ODataConstants.Equal);
                    break;
                case ExpressionType.NotEqual:
                    sb.Append(ODataConstants.NotEqual);
                    break;
                case ExpressionType.LessThan:
                    sb.Append(ODataConstants.LessThan);
                    break;
                case ExpressionType.LessThanOrEqual:
                    sb.Append(ODataConstants.LessThanOrEqual);
                    break;
                case ExpressionType.GreaterThan:
                    sb.Append(ODataConstants.GreaterThan);
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    sb.Append(ODataConstants.GreaterThanOrEqual);
                    break;
                default:
                    throw new NotSupportedException($"The binary operator '{b.NodeType}' is not supported");
            }

            Visit(b.Right);
            sb.Append(ODataConstants.RightParentheses);
            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            AppendByValueType(c.Value!, sb);
            return c;
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Expression?.NodeType == ExpressionType.Parameter)
            {
                sb.Append(m.Member.Name);
                return m;
            }
            else if (m.Expression?.NodeType == ExpressionType.MemberAccess) // Expression when some member is being accessed
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

                // Reverse the other so it's in the order of the code
                // e.g. 
                // Expression = x => x.Access.Some.Prop
                // Original = Prop, Some, Access
                // Reversed = Access, Some, Prop
                memberAccessNames.Reverse();

                if (expression is ConstantExpression constantExpression)
                {
                    var value = constantExpression.Value!;
                    value = AccessMultipleMembers(value, memberAccessNames);
                    AppendByValueType(value, sb);
                    return m;
                }
                else if (expression is ParameterExpression) // Member access from parameter
                {
                    // e.g. 
                    // Original = x => x.Accessing.Some.Member.From.The.Parameter
                    // Resulting OData = Accessing/Some/Member/From/The/Parameter
                    sb.Append(string.Join("/", memberAccessNames));
                    return m;
                }
                else if (expression is MemberExpression memberExpression)
                {
                    if (memberExpression.Member is FieldInfo fieldInfo && fieldInfo.IsStatic)
                    {
                        // static field
                        var value = fieldInfo.GetValue(null)!;
                        value = AccessMultipleMembers(value, memberAccessNames.Skip(1));
                        AppendByValueType(value, sb);
                        return m;
                    }
                    else if (memberExpression.Member is PropertyInfo propertyInfo)
                    {
                        // static property
                        var value = propertyInfo.GetValue(null)!;
                        value = AccessMultipleMembers(value, memberAccessNames.Skip(1));
                        AppendByValueType(value, sb);
                        return m;
                    }
                }
            }
            else
            {
                object? value;
                if (m.Expression is ConstantExpression constantExpression)
                {
                    if (m.Member is FieldInfo fieldInfo)
                    {
                        value = fieldInfo.GetValue(constantExpression.Value);
                        AppendByValueType(value, sb);
                        return m;
                    }
                    else if (m.Member is PropertyInfo propertyInfo)
                    {
                        value = propertyInfo.GetValue(constantExpression.Value);
                        AppendByValueType(value, sb);
                        return m;
                    }
                }
                else if (m.Member is PropertyInfo propertyInfo)
                {
                    if (m.Expression is MemberExpression exp)
                    {
                        if (exp.Expression is ConstantExpression constant && exp.Member is FieldInfo fieldInfo)
                        {
                            var fieldInfoValue = fieldInfo.GetValue(constant.Value)!;
                            value = propertyInfo.GetValue(fieldInfoValue, null);
                            AppendByValueType(value, sb);

                            return m;
                        }
                    }
                    else
                    {
                        // static property
                        value = propertyInfo.GetValue(null)!;
                        AppendByValueType(value, sb);

                        return m;
                    }
                }
                else if (m.Member is FieldInfo fieldInfo && fieldInfo.IsStatic)
                {
                    // static field
                    value = fieldInfo.GetValue(null)!;
                    AppendByValueType(value, sb);

                    return m;
                }
            }
            throw new NotSupportedException($"The member '{m.Member.Name}' is not supported");
        }
        #endregion

        #region Helpers
        private void AppendByValueType(object? value, StringBuilder sb)
        {
            if (value == null)
            {
                sb.Append(ODataConstants.Null);
                return;
            }

            var type = value.GetType();
            var underlyingType = Nullable.GetUnderlyingType(type);

            // Handle nullable types
            if (underlyingType != null)
            {
                // This shouldn't happen due to boxing, but handle it just in case
                var hasValueProperty = type.GetProperty("HasValue");
                var valueProperty = type.GetProperty("Value");

                if (hasValueProperty == null || valueProperty == null)
                {
                    throw new NotSupportedException($"Cannot access nullable properties for type '{type.Name}'");
                }

                var hasValue = (bool)hasValueProperty.GetValue(value)!;
                if (!hasValue)
                {
                    sb.Append(ODataConstants.Null);
                    return;
                }
                value = valueProperty.GetValue(value)!;
                type = underlyingType;
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    sb.Append(((bool)value) ? ODataConstants.True : ODataConstants.False);
                    break;
                case TypeCode.String:
                    sb.Append("'");
                    sb.Append(value);
                    sb.Append("'");
                    break;
                case TypeCode.DateTime:
                    sb.Append(FilterHelper.ToODataFilter((DateTime)value, odataVersion));
                    break;
                case TypeCode.Object:
                    if (value is DateTimeOffset dateTimeOffset)
                    {
                        sb.Append(FilterHelper.ToODataFilter(dateTimeOffset, odataVersion));
                    }
                    else
                    {
                        throw new NotSupportedException($"The constant for '{value}' is not supported");
                    }
                    break;
                default:
                    sb.Append(value);
                    break;
            }
        }

        private static object? AccessMultipleMembers(object value, IEnumerable<string> memberAccessNames)
        {
            foreach (var accessName in memberAccessNames)
            {
                var property = value.GetType().GetProperty(accessName);
                if (property == null)
                {
                    throw new NotSupportedException($"Property '{accessName}' not found on type '{value.GetType().Name}'");
                }

                var nextValue = property.GetValue(value);
                if (nextValue == null)
                {
                    return null;
                }
                value = nextValue;
            }
            return value;
        }
        #endregion
    }
}
