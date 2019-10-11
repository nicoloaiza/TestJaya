using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Exceptions;
using System.Reflection;
using TestJaya.Data.Filters;
using TestJaya.Data.Models;

namespace TestJaya.Data.Util
{
    public static class SearchUtil
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(SearchUtil));
        private static readonly Dictionary<Type, PropertyInfo[]> entityProperties = new Dictionary<Type, PropertyInfo[]>();

        /// <summary>
        /// Gets a value indicating whether the <paramref name="searchString"/> is contained in any property of the <typeparamref name="TEntity"/> instance.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity in which the search will be performed.</typeparam>
        /// <typeparam name="TId">Type of Id of the entity in which the search will be performed.</typeparam>
        /// <param name="entity">Instance of <typeparamref name="TEntity"/> in which the search will be performed.</param>
        /// <param name="searchString">Terms to search in the properties of the entity.</param>
        /// <returns><c>true</c> if <paramref name="entity"/> contains the <paramref name="searchString"/> in any of its properties, otherwise <c>false</c>.</returns>
        public static bool ContainsText<TEntity, TId>(this TEntity entity, string searchString)
            where TEntity : IEntity<TId>
        {
            var properties = getCachedProperties<TEntity>();
            return properties.Any(property =>
            {
                var value = property.GetValue(entity);
                return value != null && value.ToString().ToLowerInvariant().Contains(searchString);
            });
        }

        public static IQueryable<TEntity> SkipTakeOrder<TEntity, TId>(this IQueryable<TEntity> set, PagedSearchAndFilterInfo filterInfo)
            where TEntity : IEntity<TId>
        {
            if (filterInfo == null) return set;
            return set.SkipTakeOrder<TEntity, TId>(filterInfo.Offset, filterInfo.Limit, filterInfo.OrderBy, filterInfo.OrderAsc);
        }

        /// <summary>
        /// Performs skip, take and order operations in the specified <paramref name="set"/>.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity contained in the set.</typeparam>
        /// <typeparam name="TId">Type of Id of the entity contained in the set.</typeparam>
        /// <param name="set">Set to perform skip, take and order operations.</param>
        /// <param name="skip">Number of entities in the set to skip from the result</param>
        /// <param name="take">Number of entities to take from the set.</param>
        /// <param name="orderBy">Name of the entity field to sort.</param>
        /// <param name="orderAsc">Indicates whether the results should be sorted in ascending (<c>true</c>) or descending (<c>false</c>) order.</param>
        /// <returns></returns>
        public static IQueryable<TEntity> SkipTakeOrder<TEntity, TId>(this IQueryable<TEntity> set, int skip, int take, string orderBy = null, bool orderAsc = true)
            where TEntity : IEntity<TId>
        {
            if (orderBy == null) orderBy = "Id";

            try { set = set.OrderBy($"{orderBy}{(!orderAsc ? " DESC" : "")}"); }
            catch (ParseException ex)
            {
                logger.Warn($"Trying to sort results of type {typeof(TEntity)} by {orderBy} property or navigation path, but it was not possible due the following reason: {ex.Message}. The default sort (by Id) will be used.");
                set = set.OrderBy(o => o.Id);
            }
            try
            {
                if (take > 0)
                {
                    set = set.Skip(skip);
                    set = set.Take(take);
                }
                return set;
            }
            catch (NotSupportedException ex)
            {
                if (ex.Message == "Cannot rename objects with Jet" || ex.Message.Contains("not supported by SQLite"))
                {
                    logger.Warn("The data model for the Discovery Tool has changed. Please delete the database file and try again.");
                    return new List<TEntity>().AsQueryable();
                }
                else throw ex;
            }
        }

        /// <summary>
        /// Gets the properties of type <typeparamref name="T"/>, either with reflection or from their cached value.
        /// </summary>
        /// <typeparam name="T"><see cref="Type"/> to get its properties.</typeparam>
        /// <returns>Array of <see cref="PropertyInfo"/> from <typeparamref name="T"/></returns>
        private static PropertyInfo[] getCachedProperties<T>()
        {
            if (!entityProperties.ContainsKey(typeof(T))) entityProperties.Add(typeof(T), typeof(T).GetProperties());
            return entityProperties[typeof(T)];
        }
    }
}
