using System;
using System.Linq;
using System.Linq.Expressions;

namespace Application.Common.Specifications
{
    public static class ExpressionCombiner
    {
        public static Expression<Func<T, bool>> And<T>(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            var visitor = new ParameterReplaceVisitor
            {
                Target = right.Parameters[0],
                Replacement = left.Parameters[0],
            };

            var rewrittenRight = visitor.Visit(right.Body);
            var andExpression = Expression.AndAlso(left.Body, rewrittenRight);
            return Expression.Lambda<Func<T, bool>>(andExpression, left.Parameters);
        }

        public static Expression<Func<T, bool>> Or<T>(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            var visitor = new ParameterReplaceVisitor
            {
                Target = right.Parameters[0],
                Replacement = left.Parameters[0],
            };

            var rewrittenRight = visitor.Visit(right.Body);
            var orExpression = Expression.OrElse(left.Body, rewrittenRight);
            return Expression.Lambda<Func<T, bool>>(orExpression, left.Parameters);
        }

        private sealed class ParameterReplaceVisitor : ExpressionVisitor
        {
            public ParameterExpression Target { get; set; }
            public ParameterExpression Replacement { get; set; }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == Target ? Replacement : base.VisitParameter(node);
            }
        }
    }
}
