using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Linq2OData.Client
{
    public class ODataQuery<T>(ODataClient odataClient, string entitySetName, string? keyString = null)
    {
        private string? topExpression;
        private string? expandExpression;
        private string? filterExpression;


        public ODataClient ODataClient => odataClient;
        public string EntitySetName => entitySetName;

        public ODataQuery<T> Top(int? count)
        {
            if (!count.HasValue) { topExpression = null; }

            topExpression = $"$top={count}";
            return this;
        }

        public ODataQuery<T> Expand(string? expand = null)
        {
            if (string.IsNullOrWhiteSpace(expand)) { expandExpression = null; }

            expandExpression = $"$expand={expand}";
            return this;
        }

        public ODataQuery<T> Filter(string? filter = null)
        {
            if (string.IsNullOrWhiteSpace(filter)) { filterExpression = null; }

            filterExpression = $"$filter={filter}";
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

            if (string.IsNullOrWhiteSpace(keyString))
            {
                urlBuilder.Append(EntitySetName);
            }
            else
            {
                urlBuilder.Append($"{EntitySetName}({keyString})");
            }


            var queryParameters = new List<string>();
            if (!string.IsNullOrWhiteSpace(topExpression))
            {
                queryParameters.Add(topExpression);
            }

            if (!string.IsNullOrWhiteSpace(expandExpression))
            {
                queryParameters.Add(expandExpression);
            }

            if (!string.IsNullOrWhiteSpace(filterExpression))
            {
                queryParameters.Add(filterExpression);
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
