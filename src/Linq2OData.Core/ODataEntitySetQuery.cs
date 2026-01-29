using Linq2OData.Core.Expressions;
using System.Linq.Expressions;
using System.Text;

namespace Linq2OData.Core
{

    public class ODataEntityQuery<T>(ODataClient odataClient, string entitySetName, string keyString)
    {
        public string? ExpandValue { get; private set; }
        public ODataClient ODataClient => odataClient;
        public string EntitySetName => entitySetName;
        public string KeyString => keyString;

        public ODataEntityQuery<T> Expand(string? expand = null)
        {
            ExpandValue = expand;
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

    }


    public class ODataEntitySetQuery<T>(ODataClient odataClient, string entitySetName)
    {
        public ODataClient ODataClient => odataClient;
        public string EntitySetName => entitySetName;


        public bool CountValue { get; private set; }
        public int? TopValue { get; private set; }
        public int? SkipValue { get; private set; }
        public string? SelectValue { get; private set; }
        public string? ExpandValue { get; private set; }
        public string? FilterValue { get; private set; }


        public ODataEntitySetQuery<T> Top(int? top)
        {
            TopValue = top;
            return this;
        }

        public ODataEntitySetQuery<T> Count(bool count = true)
        {
            CountValue = count;
            return this;
        }

        public ODataEntitySetQuery<T> Skip(int? skip)
        {
            SkipValue = skip;
            return this;
        }

        public ODataEntitySetQuery<T> Expand(string? expand = null)
        {
            ExpandValue = expand;
            return this;
        }

        public ODataEntitySetQuery<T> Filter(string? filter = null)
        {
            FilterValue = filter;
            return this;
        }

        public ODataEntitySetQuery<T> Filter(Expression<Func<T, bool>> expression)
        {
            var oDataFilterVisitor = new ODataFilterVisitor();
            FilterValue = oDataFilterVisitor.ToFilter(expression);
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
    }
}
