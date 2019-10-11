
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using log4net;
using TestJaya.Data.Filters;
using TestJaya.Data.Models;
using TestJaya.Data.Util;

namespace TestJaya.Data.DAL
{
    public class BaseRepository : IBaseDataRepository
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(BaseRepository));

        protected readonly DbContext DbContext;

        [ExcludeFromCodeCoverage]
        public BaseRepository() => DbContext = new DbContext();

        /// <summary>
        /// Creates a new instance of <see cref="BaseStoredDataRepository"/>
        /// </summary>
        /// <param name="dbContext">Entity Framework DbContext to use with this repository</param>
        public BaseRepository(DbContext dbContext) => DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        /// <summary>
        /// Creates a new entity in the database, specifying an entity type that is not the default type for
        /// this repository.
        /// </summary>
        /// <typeparam name="TOtherEntity">Type of the entity to save.</typeparam>
        /// <typeparam name="TOtherEntityId">Type of the Id of the entity to save.</typeparam>
        /// <param name="entity">Entity to create.</param>
        /// <param name="save">When <c>true</c>, changes are saved after insertion. Otherwise, the <see cref="Save"/>method must be called after this call.</param>
        /// <returns>When successful, returns the created entity; otherwise returns null.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="entity"/> is null.</exception>
        public TOtherEntity Create<TOtherEntity, TOtherEntityId>(TOtherEntity entity, bool save = true)
            where TOtherEntity : class, IEntity<TOtherEntityId>
        {
            if (entity == default(TOtherEntity)) throw new ArgumentNullException(nameof(entity));
            TOtherEntity result = DbContext.Set<TOtherEntity>().Add(entity).Entity;
            if (!save) return result;
            return Save() ? result : null;
        }

        /// <summary>
        /// Retrieves an entity of type <typeparamref name="TEntity"/> from the database by <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity to retrieve.</typeparam>
        /// <typeparam name="TId">Type of the Id of the entity to retrieve.</typeparam>
        /// <param name="id">ID of the entity to search.</param>
        /// <returns>If found, the entity with the specified <paramref name="id"/></returns>
        public TEntity Retrieve<TEntity, TId>(TId id)
            where TEntity : class, IEntity<TId>
        {
            try { return DbContext.Set<TEntity>().Find(id); }
            catch (Exception ex)
            {
                logger.Error($"Error while trying to retrieve an entity of type {typeof(TEntity).Name} with Id = {id}.", ex);
                return null;
            }
        }

        /// <summary>
        /// Retrieves all entries of type <typeparamref name="TEntity"/> from the database with an optional <paramref name="where"/> clause.
        /// </summary>
        /// <typeparam name="TEntity">Type of entities to retrieve.</typeparam>
        /// <typeparam name="TId">Type of Id of the entities to retrieve.</typeparam>
        /// <param name="where">Optional WHERE expression to filter results.</param>
        /// <param name="skip">Number of entries to skip during pagination.</param>
        /// <param name="take">Number of entries to take during pagination. If zero, results won't be paginated.</param>
        /// <returns>Enumeration of <typeparamref name="TEntity"/> found in the database.</returns>
        public IEnumerable<TEntity> RetrieveAll<TEntity, TId>(Expression<Func<TEntity, bool>> where = null, string searchString = null, int skip = 0, int take = 0, string orderBy = null, bool orderAsc = true)
            where TEntity : class, IEntity<TId>
        {
            if (searchString != null) searchString = searchString.ToLowerInvariant();
            try
            {
                var set = DbContext.Set<TEntity>().AsQueryable();
                if (where != null) set = set.Where(where);   // Filter results
                var result = string.IsNullOrWhiteSpace(searchString) ? set : set.ToList().Where(entity => entity.ContainsText<TEntity, TId>(searchString)).AsQueryable();
                return SearchUtil.SkipTakeOrder<TEntity, TId>(result, skip, take, orderBy, orderAsc);
            }
            catch (NotSupportedException ex)
            {
                if (ex.Message == "Cannot rename objects with Jet" || ex.Message.Contains("not supported by SQLite"))
                {
                    logger.Warn("The data model for the Discovery Tool has changed. Please delete the database file and try again.");
                    return new List<TEntity>();
                }
                else throw ex;
            }
        }

        /// <summary>
        /// Retrieves all entries of type <typeparamref name="TEntity"/> from the database with an optional <paramref name="extraFilter"/> clause and it returns the Total Number of Records found.
        /// </summary>
        /// <typeparam name="TEntity">Type of entities to retrieve.</typeparam>
        /// <typeparam name="TId">Type of Id of the entities to retrieve.</typeparam>
        /// <param name="pagedSearchAndFilterInfo">Contains all the information for the dynamic filtering, paging, ordering and sorting of the results.</param>
        /// <param name="extraFilter">Adds an additional expression to filter the results.</param>
        /// <param name="totalNumberOfRecords">the Total Number of Records found.</param>
        /// <returns>Enumeration of <see cref="TEntity"/> found in the database.</returns>
        //TODO : Considering remove the out variable and the extra filter parameter.
        public IEnumerable<TEntity> RetrieveAll<TEntity, TId>(PagedSearchAndFilterInfo pagedSearchAndFilterInfo, Expression<Func<TEntity, bool>> extraFilter, out int totalNumberOfRecords)
            where TEntity : class, IEntity<TId>
        {
            var set = DbContext.Set<TEntity>().AsQueryable();
            if (extraFilter != null) { set = set.Where(extraFilter); }

            if (pagedSearchAndFilterInfo?.Filters != null && pagedSearchAndFilterInfo.Filters.Any())
            {
                var filterData = pagedSearchAndFilterInfo.ToWhereClause<TEntity>();
                set = set.Where(filterData.Predicate, filterData.Values);   // Filter results
            }
            var result = set.ToList(); // TODO This line forces the data to be retrieved from the database, but it does it before skip and take, so it always returns all the results, which can be a performance issue.
            totalNumberOfRecords = result.Count;

            return result.AsQueryable().SkipTakeOrder<TEntity, TId>(pagedSearchAndFilterInfo);
        }

        /// <summary>
        /// Removes an entity from the database by ID.
        /// </summary>
        /// <param name="id">ID of the entity to delete.</param>
        /// <param name="save">When <c>true</c>, changes are saved after removal. Otherwise, the <see cref="Save"/>method must be called after this call.</param>
        /// <returns><c>true</c> when the removal of the entity was successful, otherwise <c>false</c>.</returns>
        public bool Delete<TEntity, TId>(TId id, bool save = true)
            where TEntity : class, IEntity<TId>
        {
            var entity = Retrieve<TEntity, TId>(id);
            if (entity == null) throw new KeyNotFoundException($"Entity with ID {id} was not found.");
            return Delete<TEntity, TId>(entity, save);
        }

        /// <summary>
        /// Deletes the specified <paramref name="entity"/> from the database.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity to delete.</typeparam>
        /// <typeparam name="TId">Type of the Id of the entity to delete.</typeparam>
        /// <param name="entity">Entity to delete</param>
        /// <param name="save">When <c>true</c>, changes are saved after removal. Otherwise, the <see cref="Save"/>method must be called after this call.</param>
        /// <returns><c>true</c> when the removal of the entity was successful, otherwise <c>false</c>.</returns>
        public bool Delete<TEntity, TId>(TEntity entity, bool save = true) where TEntity : class, IEntity<TId>
        {
            DbContext.Set<TEntity>().Remove(entity);
            return save ? Save() : save;
        }

        /// <summary>
        /// Determines wether the Entity of type <typeparamref name="TEntity"/> with the specified <paramref name="id"/> exists in the database.
        /// </summary>
        /// <param name="id">ID of the entity to find.</param>
        /// <returns><c>true</c> when the entity with the specified <paramref name="id"/> exists in the database, otherwise <c>false</c></returns>
        public bool Exists<TEntity, TId>(TId id) where TEntity : class, IEntity<TId> => Exists<TEntity, TId>(entity => entity.Id.Equals(id));

        /// <summary>
        /// Determines whether there exist entities of <typeparamref name="TEntity"/> in the database, optionally filtered by a <paramref name="where"/> expression.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity to search.</typeparam>
        /// <typeparam name="TId">Type of Id of the entity to search.</typeparam>
        /// <param name="where">Optional WHERE clause to filter the search.</param>
        /// <returns><c>true</c> when entities exist with the specified parameters, otherwise <c>false</c>.</returns>
        public bool Exists<TEntity, TId>(Expression<Func<TEntity, bool>> where = null) where TEntity : class, IEntity<TId> => where != null ? DbContext.Set<TEntity>().ToList().AsQueryable().Any(where) : DbContext.Set<TEntity>().Any(); // HACK ToList should not be called!

        /// <summary>
        /// Returns the total number of entities of type <typeparamref name="TEntity"/> in the database that match the specified <paramref name="where"/> clause and <paramref name="searchString"/>.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity to count.</typeparam>
        /// <typeparam name="TId">Type of the Id of the entity to count.</typeparam>
        /// <param name="where">Expression to filter entities.</param>
        /// <param name="searchString">String to look for into the entities.</param>
        /// <returns>Number of entities that match the specified <paramref name="where"/> clause and search string.</returns>
        [ExcludeFromCodeCoverage]
        public int Count<TEntity>(Expression<Func<TEntity, bool>> where = null, string searchString = null) where TEntity : class, IEntity<int> => Count<TEntity, int>(where, searchString);

        /// <summary>
        /// Returns the total number of entities of type <typeparamref name="TEntity"/> in the database that match the specified <paramref name="where"/> clause and <paramref name="searchString"/>.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity to count.</typeparam>
        /// <typeparam name="TId">Type of the Id of the entity to count.</typeparam>
        /// <param name="where">Expression to filter entities.</param>
        /// <param name="searchString">String to look for into the entities.</param>
        /// <returns>Number of entities that match the specified <paramref name="where"/> clause and search string.</returns>
        public int Count<TEntity, TId>(Expression<Func<TEntity, bool>> where = null, string searchString = null)
            where TEntity : class, IEntity<TId>
        {
            try
            {
                if (searchString != null) searchString = searchString.ToLowerInvariant();
                var set = DbContext.Set<TEntity>().AsQueryable();
                if (where != null) set = set.Where(where);
                if (searchString != null)
                {
                    searchString = searchString.ToLowerInvariant();
                    set = set.ToList().Where(entity => entity.ContainsText<TEntity, TId>(searchString)).AsQueryable();
                }
                return set.Count();
            }
            catch (NotSupportedException ex)
            {
                if (ex.Message == "Cannot rename objects with Jet" || ex.Message.Contains("not supported by SQLite"))
                {
                    logger.Fatal($"Couldn't count results from the database. The data model for the Discovery Tool may have been changed. Please delete the database file and try again.", ex);
                    return 0;
                }
                else throw ex;
            }
        }

        /// <summary>
        /// Saves changes to the entities handled by this repository.
        /// </summary>
        /// <returns><c>true</c> when the operation was successful, otherwise <c>false</c></returns>
        public bool Save()
        {
            try
            {
                DbContext.SaveChanges();
                return true;
            }
            catch (ValidationException e) { logValidationIssues(e); }
            catch (Exception ex) { logger.Error("An error ocurred while trying to save changes to the database.", ex); }
            return false;
        }


        [ExcludeFromCodeCoverage]
        private void logValidationIssues(ValidationException e)
        {
            // Debug detailed validation issues.
            //var message = new StringBuilder();
            //message.Append("Errors were found while trying to save changes to the database:");
            //foreach (var eve in e.EntityValidationErrors)
            //{
            //    message.Append("\r\n");
            //    message.Append($"Entity of type \"{eve.Entry.Entity.GetType().Name}\" in state \"{eve.Entry.State}\" has the following validation errors:");
            //    foreach (var ve in eve.ValidationErrors)
            //    {
            //        message.Append("\r\n");
            //        message.Append($"- Property: \"{ve.PropertyName}\", Error: \"{ve.ErrorMessage}\"");
            //    }
            //}
            //logger.Error(message.ToString());
        }
    }

    /// <summary>
    /// Base class for access to entities from the database.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity this repository handles.</typeparam>
    /// <typeparam name="TId">Type of ID of the entity this repository handles.</typeparam>
    public class BaseStoredDataRepository<TEntity, TId> : BaseRepository, IBaseDataRepository<TEntity, TId>
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(BaseStoredDataRepository<,>));

        [ExcludeFromCodeCoverage]
        public BaseStoredDataRepository() : base(new DbContext()) { }

        /// <summary>
        /// Creates a new instance of <see cref="BaseStoredDataRepository{TEntity, TId}"/>
        /// </summary>
        /// <param name="dbContext">Entity Framework DbContext to use with this repository</param>
        public BaseStoredDataRepository(DbContext dbContext) : base(dbContext) { }

        /// <summary>
        /// Creates a new entity in the data base.
        /// </summary>
        /// <param name="entity">Entity to create.</param>
        /// <param name="save">When <c>true</c>, changes are saved after insertion. Otherwise, the <see cref="Save"/>method must be called after this call.</param>
        /// <returns>When successful, returns the created entity; otherwise returns null.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="entity"/> is null.</exception>
        public TEntity Create(TEntity entity, bool save = true) => Create<TEntity, TId>(entity, save);

        /// <summary>
        /// Retrieves an entity from the database by <paramref name="id"/>.
        /// </summary>
        /// <param name="id">ID of the entity to search.</param>
        /// <returns>If found, the entity with the specified <paramref name="id"/></returns>
        public TEntity Retrieve(TId id) => Retrieve<TEntity, TId>(id);

        /// <summary>
        /// Retrieves all entries for <see cref="TEntity"/> from the database with an optional <paramref name="where"/> clause, <paramref name="searchString"/> and
        /// paging options.
        /// </summary>
        /// <param name="where">Optional WHERE expression to filter results.</param>
        /// <param name="searchString">Search string to filter results.</param>
        /// <param name="skip">Number of entries to skip during pagination.</param>
        /// <param name="take">Number of entries to take during pagination. If zero, results won't be paginated.</param>
        /// <returns>Enumeration of <see cref="TEntity"/> found in the database.</returns>
        public IEnumerable<TEntity> RetrieveAll(Expression<Func<TEntity, bool>> where = null, string searchString = null, int skip = 0, int take = 0, string orderBy = null, bool orderAsc = true) => RetrieveAll<TEntity, TId>(where, searchString, skip, take, orderBy, orderAsc);

        /// <summary>
        /// Removes an entity from the database by ID.
        /// </summary>
        /// <param name="id">ID of the entity to delete.</param>
        /// <returns><c>true</c> when the removal of the entity was successful, otherwise <c>false</c>.</returns>
        public bool Delete(TId id, bool save = true) => Delete<TEntity, TId>(id, save);

        /// <summary>
        /// Determines wether the Entity with the specified <paramref name="id"/> exists in the database.
        /// </summary>
        /// <param name="id">ID of the entity to find.</param>
        /// <returns><c>true</c> when the entity with the specified <paramref name="id"/> exists in the database, otherwise <c>false</c></returns>
        public bool Exists(TId id) => Exists<TEntity, TId>(id);

        /// <summary>
        /// Returns the total number of entities in the database that match the specified <paramref name="where"/> clause and <paramref name="searchString"/>.
        /// </summary>
        /// <param name="where">Expression to filter entities.</param>
        /// <param name="searchString">String to look for into the entities.</param>
        /// <returns>Number of entities that match the specified <paramref name="where"/> clause and search string.</returns>
        public int Count(Expression<Func<TEntity, bool>> where = null, string searchString = null) => Count<TEntity, TId>(where, searchString);
    }
}
