using Linq2OData.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Linq2OData.Core.Builders
{
    /// <summary>
    /// Builder for creating nested OData expand expressions using a fluent API.
    /// </summary>
    /// <typeparam name="TEntity">The entity type being queried.</typeparam>
    /// <typeparam name="TProperty">The property type being expanded.</typeparam>
    public class ExpandBuilder<TEntity, TProperty> where TEntity : IODataEntitySet, new()
    {
        private readonly QueryBuilder<TEntity> queryBuilder;
        private string currentExpandPath;

        internal ExpandBuilder(QueryBuilder<TEntity> queryBuilder, string currentExpandPath)
        {
            this.queryBuilder = queryBuilder;
            this.currentExpandPath = currentExpandPath;
        }

        /// <summary>
        /// Adds a nested expand for a navigation property.
        /// </summary>
        /// <typeparam name="TNestedProperty">The type of the nested property.</typeparam>
        /// <param name="expression">Expression selecting the nested property to expand.</param>
        /// <returns>A new ExpandBuilder for further chaining.</returns>
        public ExpandBuilder<TEntity, TNestedProperty> ThenExpand<TNestedProperty>(Expression<Func<TProperty, TNestedProperty>> expression)
        {
            var visitor = new ODataExpandVisitor(queryBuilder.ODataClient.ODataVersion);
            var nestedPath = visitor.ToExpand<TProperty, TNestedProperty>(expression);

            // Combine with current expand path using nested expand syntax
            currentExpandPath = visitor.AppendNestedExpand(currentExpandPath, nestedPath);

            queryBuilder.UpdateLastExpandPath(currentExpandPath);
            return new ExpandBuilder<TEntity, TNestedProperty>(queryBuilder, currentExpandPath);
        }

        /// <summary>
        /// Adds a nested expand for a navigation property on a collection.
        /// Use this when TProperty is IEnumerable&lt;T&gt; to expand properties on the collection items.
        /// </summary>
        /// <typeparam name="TItem">The item type of the collection.</typeparam>
        /// <typeparam name="TNestedProperty">The type of the nested property.</typeparam>
        /// <param name="expression">Expression selecting the nested property from the collection item.</param>
        /// <returns>A new ExpandBuilder for further chaining.</returns>
        public ExpandBuilder<TEntity, TNestedProperty> ThenExpandCollection<TItem, TNestedProperty>(Expression<Func<TItem, TNestedProperty>> expression)
        {
            var visitor = new ODataExpandVisitor(queryBuilder.ODataClient.ODataVersion);
            var nestedPath = visitor.ToExpand<TItem, TNestedProperty>(expression);

            // Combine with current expand path using nested expand syntax
            currentExpandPath = visitor.AppendNestedExpand(currentExpandPath, nestedPath);

            queryBuilder.UpdateLastExpandPath(currentExpandPath);
            return new ExpandBuilder<TEntity, TNestedProperty>(queryBuilder, currentExpandPath);
        }

        /// <summary>
        /// Adds another expand at the same level (sibling navigation property).
        /// </summary>
        /// <typeparam name="TNextProperty">The type of the next property to expand.</typeparam>
        /// <param name="expression">Expression selecting the next property to expand.</param>
        /// <returns>A new ExpandBuilder for the next property.</returns>
        public ExpandBuilder<TEntity, TNextProperty> Expand<TNextProperty>(Expression<Func<TEntity, TNextProperty>> expression)
        {
            return queryBuilder.Expand(expression);
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
        /// Executes the query and returns the results.
        /// </summary>
        public Task<List<TEntity>?> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            return queryBuilder.ExecuteAsync(cancellationToken);
        }

        /// <summary>
        /// Configures the select clause without specifying properties (returns all).
        /// </summary>
        public QueryExecutor<TEntity, List<TEntity>> Select()
        {
            return queryBuilder.Select();
        }

        /// <summary>
        /// Configures the select clause with a custom selector expression.
        /// </summary>
        public QueryExecutor<TEntity, TResult> Select<TResult>(Expression<Func<List<TEntity>, TResult>> selector)
        {
            return queryBuilder.Select(selector);
        }

        /// <summary>
        /// Orders the results by a property in ascending order using a LINQ expression.
        /// </summary>
        public OrderByBuilder<TEntity> Order<TOrderProperty>(Expression<Func<TEntity, TOrderProperty>> expression)
        {
            return queryBuilder.Order(expression);
        }

        /// <summary>
        /// Orders the results by a property in descending order using a LINQ expression.
        /// </summary>
        public OrderByBuilder<TEntity> OrderDescending<TOrderProperty>(Expression<Func<TEntity, TOrderProperty>> expression)
        {
            return queryBuilder.OrderDescending(expression);
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
        public static implicit operator QueryBuilder<TEntity>(ExpandBuilder<TEntity, TProperty> builder)
        {
            return builder.queryBuilder;
        }
    }
}
