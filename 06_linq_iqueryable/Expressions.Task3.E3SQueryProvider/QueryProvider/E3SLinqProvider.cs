using Expressions.Task3.E3SQueryProvider.Helpers;
using Expressions.Task3.E3SQueryProvider.Services;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Expressions.Task3.E3SQueryProvider.QueryProvider
{
    public class E3SLinqProvider : IQueryProvider
    {
        private readonly E3SSearchService _e3SClient;

        public E3SLinqProvider(E3SSearchService client)
        {
            _e3SClient = client;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            Type elementType = TypeHelper.GetElementType(expression.Type);
            Type queryType = typeof(E3SQuery<>).MakeGenericType(elementType);

            return (IQueryable)Activator.CreateInstance(queryType, expression, this);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new E3SQuery<TElement>(expression, this);
        }

        public object Execute(Expression expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            Type itemType = TypeHelper.GetElementType(expression.Type);
            var translator = new ExpressionToFtsRequestTranslator();

            var statements = translator.Translate(expression);

            return _e3SClient.SearchFts(itemType, statements);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)Execute(expression);
        }
    }
}