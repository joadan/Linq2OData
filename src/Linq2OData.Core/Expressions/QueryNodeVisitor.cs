using System.Linq.Expressions;
using System.Reflection;

namespace Linq2OData.Core.Expressions
{
    public class QueryNodeVisitor : ExpressionVisitor
    {
        private readonly Stack<QueryNode> _nodeStack = new();
        private readonly Stack<ParameterExpression> _lambdaParameters = new();
        private readonly QueryNode _root;
        private readonly Type _attributeType;
        private bool _insideAnonymousType = false;

        public QueryNodeVisitor()
        {
            _attributeType = typeof(ODataMemberAttribute);
            _root = new QueryNode();
            _nodeStack.Push(_root);
        }

        public QueryNode Parse(Expression expression)
        {
            Visit(expression);
            return _root;
        }

        //public QueryNode Parse<T, TResult>(Expression<Func<T, TResult>> expression)
        //{
        //    Visit(expression);
        //    return _root;
        //}

        protected override Expression VisitMember(MemberExpression node)
        {
            // Determine which lambda parameter this member belongs to
            var parameterExpr = FindParameterExpression(node);

            // Find the correct parent node based on the lambda scope
            QueryNode targetParentNode = FindCorrectParentNode(parameterExpr);

            // Build the complete member chain first
            var memberChain = new Stack<QueryNode>();
            var current = node;

            while (current != null)
            {
                if (TryGetCustomAttribute(current.Member, out var oDataMemberAttribute))
                {
                    // If the member has the custom attribute, we want to add it to the tree
                    memberChain.Push(new QueryNode(current.Member, oDataMemberAttribute!));
                }
                if (current.Expression is MemberExpression memberExpr)
                {
                    current = memberExpr;
                }
                else
                {
                    break;
                }
            }

            // Now add the chain to the tree starting from the correct parent
            var currentNode = targetParentNode;
            QueryNode? lastNode = null;

            while (memberChain.Count > 0)
            {
                //var member = memberChain.Pop();
                //var newNode = new QueryNode(member);
                var newNode = memberChain.Pop();
                currentNode.AddOrMergeChild(newNode);
                currentNode = currentNode.Children.First(c => c.Member == newNode.Member && ArgumentsEqual(c.Arguments, newNode.Arguments));
                lastNode = currentNode;
            }

            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            // Determine which lambda parameter this method belongs to
            ParameterExpression? parameterExpr = null;
            if (node.Object != null)
            {
                parameterExpr = FindParameterExpression(node.Object);
            }

            // Find the correct parent node based on the lambda scope
            QueryNode targetParentNode = FindCorrectParentNode(parameterExpr);

            // Visit the object instance first (if any)
            if (node.Object != null)
            {
                if (node.Object is MemberExpression memberExpr)
                {
                    Visit(memberExpr);
                }
                else if (node.Object is ParameterExpression)
                {
                    // Don't visit parameter expressions
                }
                else
                {
                    Visit(node.Object);
                }
            }
            else
            {
                // For extension methods (where Object is null), 
                // the source might be in the first argument
                // We need to visit it if it's a member expression to add it to the tree
                if (node.Arguments.Count > 0 && node.Arguments[0] is MemberExpression sourceMemberExpr)
                {
                    Visit(sourceMemberExpr);
                }
                else if (node.Arguments.Count > 0 && node.Arguments[0] is MethodCallExpression)
                {
                    // If it's a chained method call, visit it
                    Visit(node.Arguments[0]);
                }
            }

            if (TryGetCustomAttribute(node.Method, out var oDataMemberAttribute))
            {
                var newNode = new QueryNode(node.Method, oDataMemberAttribute!);

                // Extract method arguments
                foreach (var arg in node.Arguments)
                {
                    var argValue = TryGetConstantValue(arg);
                    newNode.Arguments.Add(argValue);
                }

                targetParentNode.AddOrMergeChild(newNode);
            }
            else
            {
                // For LINQ methods like Select, Where, etc.
                // Find the source member (the collection being operated on)
                var sourceMember = FindSourceMemberExpression(node);
                QueryNode? nodeToStack = null;

                if (sourceMember != null)
                {
                    // Find the node corresponding to this source member
                    nodeToStack = FindNodeForMemberExpression(sourceMember, targetParentNode);
                }

                // Push the source node onto the stack so lambda parameters know their context
                if (nodeToStack != null)
                {
                    _nodeStack.Push(nodeToStack);
                }

                // Visit the method arguments (typically lambdas)
                // Skip the first argument if it's the source (for extension methods)
                var startIndex = (node.Object == null && node.Arguments.Count > 0 && node.Arguments[0] == sourceMember) ? 1 : 0;
                for (int i = startIndex; i < node.Arguments.Count; i++)
                {
                    Visit(node.Arguments[i]);
                }

                // Pop the source node after visiting arguments
                if (nodeToStack != null)
                {
                    _nodeStack.Pop();
                }
            }

            return node;
        }

        private MemberExpression? FindSourceMemberExpression(MethodCallExpression methodCall)
        {
            // For extension methods (like LINQ operators), the source is the first argument
            if (methodCall.Object == null && methodCall.Arguments.Count > 0)
            {
                var firstArg = methodCall.Arguments[0];

                if (firstArg is MemberExpression memberExpr)
                {
                    return memberExpr;
                }
                else if (firstArg is MethodCallExpression methodCallExpr)
                {
                    return FindSourceMemberExpression(methodCallExpr);
                }
            }

            // For instance methods
            var current = methodCall.Object;

            while (current != null)
            {
                if (current is MemberExpression memberExpr)
                {
                    return memberExpr;
                }
                else if (current is MethodCallExpression methodCallExpr)
                {
                    current = methodCallExpr.Object;
                }
                else
                {
                    break;
                }
            }

            return null;
        }

        private QueryNode? FindNodeForMemberExpression(MemberExpression memberExpr, QueryNode searchRoot)
        {
            // Build the member chain
            var memberChain = new Stack<MemberInfo>();
            var current = memberExpr;

            while (current != null)
            {
                if (HasCustomAttribute(current.Member))
                {
                    memberChain.Push(current.Member);
                }

                if (current.Expression is MemberExpression nextMember)
                {
                    current = nextMember;
                }
                else
                {
                    break;
                }
            }

            // Navigate through the tree to find the matching node
            var currentNode = searchRoot;
            while (memberChain.Count > 0)
            {
                var member = memberChain.Pop();
                var childNode = currentNode.Children.FirstOrDefault(c => c.Member == member);
                if (childNode == null)
                {
                    return null;
                }
                currentNode = childNode;
            }

            return currentNode;
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            // Push the lambda parameter onto the stack
            if (node.Parameters.Count > 0)
            {
                _lambdaParameters.Push(node.Parameters[0]);
            }

            // Visit the lambda body
            Visit(node.Body);

            // Pop the lambda parameter
            if (node.Parameters.Count > 0)
            {
                _lambdaParameters.Pop();
            }

            return node;
        }

        protected override Expression VisitNew(NewExpression node)
        {
            var wasInsideAnonymousType = _insideAnonymousType;
            _insideAnonymousType = true;

            // Visit all constructor arguments (for anonymous types)
            foreach (var arg in node.Arguments)
            {
                // Save the current stack depth before visiting each argument
                var stackDepthBefore = _nodeStack.Count;

                Visit(arg);

                // Restore the stack to its state before this argument
                // This prevents members from becoming nested under previous members
                while (_nodeStack.Count > stackDepthBefore)
                {
                    _nodeStack.Pop();
                }
            }

            _insideAnonymousType = wasInsideAnonymousType;
            return node;
        }

        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            var wasInsideAnonymousType = _insideAnonymousType;
            _insideAnonymousType = true;

            Visit(node.NewExpression);

            foreach (var binding in node.Bindings)
            {
                if (binding is MemberAssignment assignment)
                {
                    // Save the current stack depth before visiting each assignment
                    var stackDepthBefore = _nodeStack.Count;

                    Visit(assignment.Expression);

                    // Restore the stack to its state before this assignment
                    while (_nodeStack.Count > stackDepthBefore)
                    {
                        _nodeStack.Pop();
                    }
                }
            }

            _insideAnonymousType = wasInsideAnonymousType;
            return node;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            // Handle conversions (like Convert expressions)
            Visit(node.Operand);
            return node;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            Visit(node.Left);
            Visit(node.Right);
            return node;
        }

        private ParameterExpression? FindParameterExpression(Expression expression)
        {
            while (expression != null)
            {
                if (expression is ParameterExpression paramExpr)
                {
                    return paramExpr;
                }
                else if (expression is MemberExpression memberExpr)
                {
                    expression = memberExpr.Expression!;
                }
                else if (expression is MethodCallExpression methodExpr)
                {
                    expression = methodExpr.Object!;
                }
                else
                {
                    break;
                }
            }
            return null;
        }

        private QueryNode FindCorrectParentNode(ParameterExpression? parameterExpr)
        {
            if (parameterExpr == null)
            {
                return _nodeStack.Peek();
            }

            // If this is the current (innermost) lambda parameter, use the current node stack position
            if (_lambdaParameters.Count > 0 && _lambdaParameters.Peek() == parameterExpr)
            {
                return _nodeStack.Peek();
            }

            // If this is an outer lambda parameter, we need to find the correct parent
            // by going back in the lambda parameter stack
            var lambdaList = _lambdaParameters.ToList();
            var paramIndex = lambdaList.IndexOf(parameterExpr);

            if (paramIndex >= 0)
            {
                // This is an outer lambda parameter
                // We want to place it at the root level if it's from an outer scope
                // Calculate how many levels up we need to go
                var levelsUp = paramIndex;

                if (levelsUp > 0)
                {
                    // It's from an outer lambda, place it at the root
                    return _root;
                }
            }

            // Default to current stack position
            return _nodeStack.Peek();
        }

        private bool TryGetCustomAttribute(MemberInfo member, out ODataMemberAttribute? oDataMemberAttribute)
        {
            oDataMemberAttribute = member.GetCustomAttribute<ODataMemberAttribute>();
            if (oDataMemberAttribute == null)
            {
                return false;
            }

            return true;
        }

        private bool HasCustomAttribute(MemberInfo member)
        {
            return member.GetCustomAttributes(_attributeType, true).Any();
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

        private static object? TryGetConstantValue(Expression expression)
        {
            try
            {
                if (expression is ConstantExpression constant)
                {
                    return constant.Value;
                }

                if (expression is MemberExpression memberExpr &&
                    memberExpr.Expression is ConstantExpression constantExpr)
                {
                    var member = memberExpr.Member;
                    if (member is FieldInfo field)
                    {
                        return field.GetValue(constantExpr.Value);
                    }
                    if (member is PropertyInfo property)
                    {
                        return property.GetValue(constantExpr.Value);
                    }
                }

                // Try to compile and evaluate the expression
                var lambda = Expression.Lambda(expression);
                var compiled = lambda.Compile();
                return compiled.DynamicInvoke();
            }
            catch
            {
                return $"<{expression.NodeType}>";
            }
        }
    }
}
