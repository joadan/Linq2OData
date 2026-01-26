using Linq2OData.Client.Expressions;
using System.Linq.Expressions;
using System.Text;

namespace Linq2OData.Client
{

    public class ODataEntityQuery<T>(ODataClient odataClient, string entitySetName, string keyString)
    {
        private string? expandExpression;

        public ODataClient ODataClient => odataClient;

        public ODataEntityQuery<T> Expand(string? expand = null)
        {
            if (string.IsNullOrWhiteSpace(expand)) { expandExpression = null; }

            expandExpression = $"$expand={expand}";
            return this;
        }

        public ODataEntityExecute<T, T> Select()
        {
            return new ODataEntityExecute<T, T>(this, null);
        }

        public ODataEntityExecute<T, TResult> Select<TResult>(Expression<Func<T, TResult>> selector)
        {
            //ParseExpression(selector);
            return new ODataEntityExecute<T, TResult>(this, selector);
        }


        internal string GenerateRequestUrl()
        {
            var urlBuilder = new StringBuilder();
            urlBuilder.Append($"{entitySetName}({keyString})");

            var queryParameters = new List<string>();
            if (!string.IsNullOrWhiteSpace(expandExpression))
            {
                queryParameters.Add(expandExpression);
            }

            if (queryParameters.Count > 0)
            {
                urlBuilder.Append("?");
                urlBuilder.Append(string.Join("&", queryParameters));
            }
            return urlBuilder.ToString();
        }

    }


    public class ODataEntitySetQuery<T>(ODataClient odataClient, string entitySetName)
    {
        private string? topExpression;
        private string? skipExpression;
        private string? expandExpression;
        private string? filterExpression;
        private string? inlineCountExpression;

        public ODataClient ODataClient => odataClient;
        public string EntitySetName => entitySetName;

        public ODataEntitySetQuery<T> Top(int? count)
        {
            if (!count.HasValue) { topExpression = null; }

            topExpression = $"$top={count}";
            return this;
        }

        public ODataEntitySetQuery<T> InlineCount(bool include = true)
        {
            if (!include) { inlineCountExpression = null; }

            inlineCountExpression = $"$inlinecount=allpages";
            return this;
        }

        public ODataEntitySetQuery<T> Skip(int? skip)
        {
            if (!skip.HasValue) { skipExpression = null; }

            skipExpression = $"$skip={skip}";
            return this;
        }


        public ODataEntitySetQuery<T> Expand(string? expand = null)
        {
            if (string.IsNullOrWhiteSpace(expand)) { expandExpression = null; }

            expandExpression = $"$expand={expand}";
            return this;
        }

        public ODataEntitySetQuery<T> Filter(string? filter = null)
        {
            if (string.IsNullOrWhiteSpace(filter)) { filterExpression = null; }

            filterExpression = $"$filter={filter}";
            return this;
        }

        public ODataEntitySetQuery<T> Filter(Expression<Func<T, bool>> expression)
        {
            var oDataFilterVisitor = new ODataFilterVisitor();
            var filter = oDataFilterVisitor.ToFilter(expression);

            filterExpression = $"$filter={filter}";
            return this;
        }

        public ODataEntitySetExecute<T, List<T>> Select()
        {
            return new ODataEntitySetExecute<T, List<T>>(this, null);
        }

        public ODataEntitySetExecute<T, TResult> Select<TResult>(Expression<Func<List<T>, TResult>> selector)
        {
            //ParseExpression(selector);
            return new ODataEntitySetExecute<T, TResult>(this, selector);
        }


        internal string GenerateRequestUrl()
        {
            var urlBuilder = new StringBuilder();
            urlBuilder.Append(EntitySetName);


            var queryParameters = new List<string>();
            if (!string.IsNullOrWhiteSpace(topExpression))
            {
                queryParameters.Add(topExpression);
            }

            if (!string.IsNullOrWhiteSpace(skipExpression))
            {
                queryParameters.Add(skipExpression);
            }

            if (!string.IsNullOrWhiteSpace(expandExpression))
            {
                queryParameters.Add(expandExpression);
            }

            if (!string.IsNullOrWhiteSpace(filterExpression))
            {
                queryParameters.Add(filterExpression);
            }

            if (!string.IsNullOrWhiteSpace(inlineCountExpression))
            {
                queryParameters.Add(inlineCountExpression);
            }

            if (queryParameters.Count > 0)
            {
                urlBuilder.Append("?");
                urlBuilder.Append(string.Join("&", queryParameters));
            }
            return urlBuilder.ToString();
        }


    }
}
