using Linq2OData.Core.Expressions;
using System;
using System.Linq.Expressions;

namespace Linq2OData.Core.Builders
{
    /// <summary>
    /// Builder for creating OData orderby expressions using a fluent API.
    /// </summary>
    /// <typeparam name="TEntity">The entity type being queried.</typeparam>
    public class OrderByBuilder<TEntity> where TEntity : IODataEntitySet, new()
    {
        private readonly QueryBuilder<TEntity> queryBuilder;

        internal OrderByBuilder(QueryBuilder<TEntity> queryBuilder)
        {
            this.queryBuilder = queryBuilder;
        }

        /// <summary>
        /// Adds an additional ascending orderby clause.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property to order by.</typeparam>
        /// <param name="expression">Expression selecting the property to order by.</param>
        /// <returns>An OrderByBuilder for further chaining.</returns>
        public OrderByBuilder<TEntity> ThenBy<TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            var visitor = new ODataOrderByVisitor();
            var propertyName = visitor.ToOrderBy(expression);
            
            queryBuilder.AppendOrderBy(propertyName, descending: false);
            return this;
        }

        /// <summary>
        /// Adds an additional descending orderby clause.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property to order by.</typeparam>
        /// <param name="expression">Expression selecting the property to order by descending.</param>
        /// <returns>An OrderByBuilder for further chaining.</returns>
        public OrderByBuilder<TEntity> ThenByDescending<TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            var visitor = new ODataOrderByVisitor();
            var propertyName = visitor.ToOrderBy(expression);
            
            queryBuilder.AppendOrderBy(propertyName, descending: true);
            return this;
        }

        /// <summary>
        /// Adds another orderby at the same level (alternative to ThenBy).
        /// </summary>
        /// <typeparam name="TProperty">The type of the property to order by.</typeparam>
        /// <param name="expression">Expression selecting the property to order by.</param>
        /// <returns>A new OrderByBuilder.</returns>
        public OrderByBuilder<TEntity> Order<TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            return queryBuilder.Order(expression);
        }

        /// <summary>
        /// Adds another descending orderby at the same level.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property to order by.</typeparam>
        /// <param name="expression">Expression selecting the property to order by descending.</param>
        /// <returns>A new OrderByBuilder.</returns>
        public OrderByBuilder<TEntity> OrderDescending<TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            return queryBuilder.OrderDescending(expression);
        }

        /// <summary>
        /// Sets the maximum number of items to return.
        /// </summary>
        public QueryBuilder<TEntity> Top(int? top)
        {
            return queryBuilder.Top(top);
        }

        /// <summary>
        /// Includes the count of matching items in the response.
        /// </summary>
        public QueryBuilder<TEntity> Count(bool count = true)
        {
            return queryBuilder.Count(count);
        }

        /// <summary>
        /// Skips the specified number of items.
        /// </summary>
        public QueryBuilder<TEntity> Skip(int? skip)
        {
            return queryBuilder.Skip(skip);
        }

        /// <summary>
        /// Filters the results using an OData filter string.
        /// </summary>
        public QueryBuilder<TEntity> Filter(string? filter = null)
        {
            return queryBuilder.Filter(filter);
        }

        /// <summary>
        /// Filters the results using a LINQ expression.
        /// </summary>
        public QueryBuilder<TEntity> Filter(Expression<Func<TEntity, bool>> expression)
        {
            return queryBuilder.Filter(expression);
        }

        /// <summary>
        /// Expands navigation properties using a string.
        /// </summary>
        public QueryBuilder<TEntity> Expand(string? expand = null)
        {
            return queryBuilder.Expand(expand);
        }

        /// <summary>
        /// Expands navigation properties using a LINQ expression.
        /// </summary>
        public ExpandBuilder<TEntity, TProperty> Expand<TProperty>(Expression<Func<TEntity, TProperty>> expression)
        {
            return queryBuilder.Expand(expression);
        }

        /// <summary>
        /// Executes the query and returns the results.
        /// </summary>
        public Task<List<TEntity>?> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            return queryBuilder.ExecuteAsync(cancellationToken);
        }

        /// <summary>
        /// Configures the select clause with a custom selector expression.
        /// </summary>
        public QueryProjectionBuilder<TEntity, TResult> Select<TResult>(Expression<Func<List<TEntity>, TResult>> selector)
        {
            return queryBuilder.Select(selector);
        }


        /// <summary>
        /// Returns the QueryBuilder to continue building the query.
        /// </summary>
        /// <returns>The parent QueryBuilder.</returns>
        public QueryBuilder<TEntity> QueryBuilder()
        {
            return queryBuilder;
        }

        // Implicit conversion to QueryBuilder for convenience
        public static implicit operator QueryBuilder<TEntity>(OrderByBuilder<TEntity> builder)
        {
            return builder.queryBuilder;
        }
    }
}
