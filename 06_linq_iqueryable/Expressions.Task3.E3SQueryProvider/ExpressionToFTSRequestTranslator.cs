using Expressions.Task3.E3SQueryProvider.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Expressions.Task3.E3SQueryProvider
{
    public class ExpressionToFtsRequestTranslator : ExpressionVisitor
    {
        private readonly List<string> _statements = new List<string>();
        readonly StringBuilder _currentStatement;

        public ExpressionToFtsRequestTranslator()
        {
            _currentStatement = new StringBuilder();
        }

        public IReadOnlyList<string> Translate(Expression exp)
        {
            _statements.Clear();
            _currentStatement.Clear();

            Visit(exp);
            CommitStatementIfNotEmpty();

            return _statements;
        }

        #region protected methods

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Queryable) && node.Method.Name == QueryableMethods.Where)
            {
                Visit(node.Arguments[1]);
                return node;
            }
            else if (node.Method.DeclaringType == typeof(string))
            {
                return HandleStringMethod(node);
            }

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Equal:
                    HandleEqualExpression(node);
                    break;

                case ExpressionType.AndAlso:
                    Visit(node.Left);
                    CommitStatementIfNotEmpty();
                    Visit(node.Right);
                    break;

                default:
                    throw new NotSupportedException($"Operation '{node.NodeType}' is not supported");
            };

            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            _currentStatement.Append(node.Member.Name).Append(":");

            return base.VisitMember(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            _currentStatement.Append(node.Value);

            return node;
        }

        #endregion

        #region private methods
        private Expression HandleStringMethod(MethodCallExpression node)
        {
            ValidateStringMethodCall(node);

            if (!StringMethods.Patterns.TryGetValue(node.Method.Name, out var pattern))
            {
                throw new NotSupportedException($"String method '{node.Method.Name}' is not supported");
            }

            BuildWithParentheses(node.Object, node.Arguments[0], pattern.useLeft, pattern.useRight);
            return node;
        }

        private void HandleEqualExpression(BinaryExpression node)
        {
            ValidateNodeForEqualExpression(node);

            var constant = node.Left.NodeType == ExpressionType.Constant ? node.Left : node.Right;
            var member = node.Left.NodeType == ExpressionType.MemberAccess ? node.Left : node.Right;

            BuildWithParentheses(member, constant);
        }
        private void CommitStatementIfNotEmpty()
        {
            if (_currentStatement.Length == 0)
                return;

            _statements.Add(_currentStatement.ToString());
            _currentStatement.Clear();
        }

        private void ValidateNodeForEqualExpression(BinaryExpression node)
        {
            if (!IsConstantOrMember(node.Left))
                throw new NotSupportedException($"Left operand should be property, field or constant: {node.Left.NodeType}");

            if (!IsConstantOrMember(node.Right))
                throw new NotSupportedException($"Right operand should be property, field or constant: {node.Right.NodeType}");

            if (node.Left.NodeType == node.Right.NodeType)
                throw new NotSupportedException("Left and Right operands cannot have the same NodeType");
        }

        private bool IsConstantOrMember(Expression expression)
        {
            return expression.NodeType == ExpressionType.MemberAccess
                || expression.NodeType == ExpressionType.Constant;
        }

        private void ValidateStringMethodCall(MethodCallExpression node)
        {
            if (node.Arguments.Count != 1)
                throw new NotSupportedException($"Method '{node.Method.Name}' with {node.Arguments.Count} arguments is not supported");

            if (node.Object?.NodeType != ExpressionType.MemberAccess)
                throw new NotSupportedException($"Method '{node.Method.Name}' must be called on a property or field");
        }

        private void BuildWithParentheses(Expression left, Expression right, bool useLeftAsterisk = false, bool useRightAsterisk = false)
        {
            Visit(left);

            _currentStatement.Append("(");
            if (useLeftAsterisk) _currentStatement.Append("*");

            Visit(right);

            if (useRightAsterisk) _currentStatement.Append("*");
            _currentStatement.Append(")");
        }

        #endregion
    }
}
