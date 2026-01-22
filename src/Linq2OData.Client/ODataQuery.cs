using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Linq2OData.Client
{
    public class ODataQuery<T>(ODataClient odataClient, string entitySetName)
    {
        private string? topExpression;

        public string? TopExpression => topExpression;
        public ODataClient ODataClient => odataClient;
        public string EntitySetName => entitySetName;

        public ODataQuery<T> Top(int? count)
        {
            if (!count.HasValue) { topExpression = null; }

            topExpression = $"$top={count}";
            return this;
        }

        public ODataQueryExecute<T, T> Select()
        {
            //ParseExpression(selector);
            return new ODataQueryExecute<T, T>(this, null);
        }

        public ODataQueryExecute<T, TResult> Select<TResult>(Expression<Func<T, TResult>> selector)
        {
            //ParseExpression(selector);
            return new ODataQueryExecute<T, TResult>(this, selector);
        }


        internal string? GenerateRequestUrl()
        {
            var urlBuilder = new StringBuilder();
            urlBuilder.Append(EntitySetName);
            var queryParameters = new List<string>();
            if (!string.IsNullOrEmpty(TopExpression))
            {
                queryParameters.Add(TopExpression);
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
