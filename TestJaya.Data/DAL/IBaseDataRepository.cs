using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TestJaya.Data.Filters;
using TestJaya.Data.Models;

namespace TestJaya.Data.DAL
{
    public interface IBaseDataRepository
    {
        /// <summary>
        /// Creates a new entity of type <typeparamref name="TEntity"/> in the database.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity to save.</typeparam>
        /// <typeparam name="TId">Type of the Id of the entity to save.</typeparam>
        /// <param name="entity">Entity to create.</param>
        /// <param name="save">When <c>true</c>, changes are saved after insertion. Otherwise, the <see cref="Save"/>method must be called after this call.</param>
        /// <returns>When successful, returns the created entity; otherwise returns null.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="entity"/> is null.</exception>
        TEntity Create<TEntity, TId>(TEntity entity, bool save = true) where TEntity : class, IEntity<TId>;

        /// <summary>
        /// Retrieves an entity of type <typeparamref name="TEntity"/> from the database by <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity to retrieve.</typeparam>
        /// <typeparam name="TId">Type of the Id of the entity to retrieve.</typeparam>
        /// <param name="id">ID of the entity to search.</param>
        /// <returns>If found, the entity with the specified <paramref name="id"/></returns>
        TEntity Retrieve<TEntity, TId>(TId id) where TEntity : class, IEntity<TId>;

        /// <summary>
        /// Retrieves all entries of type <typeparamref name="TEntity"/> from the database with an optional <paramref name="where"/> clause.
        /// </summary>
        /// <typeparam name="TEntity">Type of entities to retrieve.</typeparam>
        /// <typeparam name="TId">Type of Id of the entities to retrieve.</typeparam>
        /// <param name="where">Optional WHERE expression to filter results.</param>
        /// <param name="skip">Number of entries to skip during pagination.</param>
        /// <param name="take">Number of entries to take during pagination. If zero, results won't be paginated.</param>
        /// <returns>Enumeration of <typeparamref name="TEntity"/> found in the database.</returns>
        IEnumerable<TEntity> RetrieveAll<TEntity, TId>(Expression<Func<TEntity, bool>> where = null, string searchString = null, int skip = 0, int take = 0, string orderBy = null, bool orderAsc = true) where TEntity : class, IEntity<TId>;

        /// <summary>
        /// Retrieves all entries of type <typeparamref name="TEntity"/> from the database with an optional <paramref name="extraFilter"/> clause and it returns the Total Number of Records found.
        /// </summary>
        /// <typeparam name="TEntity">Type of entities to retrieve.</typeparam>
        /// <typeparam name="TId">Type of Id of the entities to retrieve.</typeparam>
        /// <param name="pagedSearchAndFilterInfo">Contains all the information for the dynamic filtering, paging, ordering and sorting of the results.</param>
        /// <param name="extraFilter">Adds an additional expression to filter the results.</param>
        /// <param name="totalNumberOfRecords">the Total Number of Records found.</param>
        /// <returns>Enumeration of <see cref="TEntity"/> found in the database.</returns>
        IEnumerable<TEntity> RetrieveAll<TEntity, TId>(PagedSearchAndFilterInfo pagedSearchAndFilterInfo, Expression<Func<TEntity, bool>> extraFilter, out int totalNumberOfRecords) where TEntity : class, IEntity<TId>;

        /// <summary>
        /// Removes an entity of type <typeparamref name="TEntity"/> from the database by ID.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity to delete.</typeparam>
        /// <typeparam name="TId">Type of the Id of the entity to delete.</typeparam>
        /// <param name="id">ID of the entity to delete.</param>
        /// <param name="save">When <c>true</c>, changes are saved after removal. Otherwise, the <see cref="Save"/>method must be called after this call.</param>
        /// <returns><c>true</c> when the removal of the entity was successful, otherwise <c>false</c>.</returns>
        bool Delete<TEntity, TId>(TId id, bool save = true) where TEntity : class, IEntity<TId>;

        /// <summary>
        /// Deletes the specified <paramref name="entity"/> from the database.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity to delete.</typeparam>
        /// <typeparam name="TId">Type of the Id of the entity to delete.</typeparam>
        /// <param name="entity">Entity to delete</param>
        /// <param name="save">When <c>true</c>, changes are saved after removal. Otherwise, the <see cref="Save"/>method must be called after this call.</param>
        /// <returns><c>true</c> when the removal of the entity was successful, otherwise <c>false</c>.</returns>
        bool Delete<TEntity, TId>(TEntity entity, bool save = true) where TEntity : class, IEntity<TId>;

        /// <summary>
        /// Determines whether there exist an entity of <typeparamref name="TEntity"/> in the database with the specified <paramref name="id"/>.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity to search.</typeparam>
        /// <typeparam name="TId">Type of Id of the entity to search.</typeparam>
        /// <param name="id">Id of the entity to search.</param>
        /// <returns><c>true</c> if an entity of type <typeparamref name="TEntity"/> with the specified <paramref name="id"/> exists, otherwise <c>false</c>.</returns>
        bool Exists<TEntity, TId>(TId id) where TEntity : class, IEntity<TId>;

        /// <summary>
        /// Determines whether there exist entities of <typeparamref name="TEntity"/> in the database, optionally filtered by a <paramref name="where"/> expression.
        /// </summary>
        /// <typeparam name="TEntity">Type of entity to search.</typeparam>
        /// <typeparam name="TId">Type of Id of the entity to search.</typeparam>
        /// <param name="where">Optional WHERE clause to filter the search.</param>
        /// <returns><c>true</c> when entities exist with the specified parameters, otherwise <c>false</c>.</returns>
        bool Exists<TEntity, TId>(Expression<Func<TEntity, bool>> where = null) where TEntity : class, IEntity<TId>;

        /// <summary>
        /// Returns the total number of entities of type <typeparamref name="TEntity"/> in the database that match the specified <paramref name="where"/> clause and <paramref name="searchString"/>.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity to count.</typeparam>
        /// <typeparam name="TId">Type of the Id of the entity to count.</typeparam>
        /// <param name="where">Expression to filter entities.</param>
        /// <param name="searchString">String to look for into the entities.</param>
        /// <returns>Number of entities that match the specified <paramref name="where"/> clause and search string.</returns>
        int Count<TEntity>(Expression<Func<TEntity, bool>> where = null, string searchString = null) where TEntity : class, IEntity<int>;

        /// <summary>
        /// Returns the total number of entities of type <typeparamref name="TEntity"/> in the database that match the specified <paramref name="where"/> clause and <paramref name="searchString"/>.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity to count.</typeparam>
        /// <typeparam name="TId">Type of the Id of the entity to count.</typeparam>
        /// <param name="where">Expression to filter entities.</param>
        /// <param name="searchString">String to look for into the entities.</param>
        /// <returns>Number of entities that match the specified <paramref name="where"/> clause and search string.</returns>
        int Count<TEntity, TId>(Expression<Func<TEntity, bool>> where = null, string searchString = null) where TEntity : class, IEntity<TId>;

        /// <summary>
        /// Saves changes to the context handling this repository.
        /// </summary>
        /// <returns><c>true</c> when the operation was successful, otherwise <c>false</c></returns>
        bool Save();
    }

    /// <summary>
    /// Defines a common interface for methods to access entities from the database.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity the repository handles.</typeparam>
    /// <typeparam name="TId">Type of ID of the entity.</typeparam>
    public interface IBaseDataRepository<TEntity, TId> : IBaseDataRepository
        where TEntity : IEntity<TId>
    {
        /// <summary>
        /// Creates a new entity in the data base.
        /// </summary>
        /// <param name="entity">Entity to create.</param>
        /// <param name="save">When <c>true</c>, changes are saved after insertion. Otherwise, the <see cref="Save"/>method must be called after this call.</param>
        /// <returns>When successful, returns the created entity; otherwise returns null.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="entity"/> is null.</exception>
        TEntity Create(TEntity entity, bool save = true);

        /// <summary>
        /// Retrieves an entity from the database by <paramref name="id"/>.
        /// </summary>
        /// <param name="id">ID of the entity to search.</param>
        /// <returns>If found, the entity with the specified <paramref name="id"/></returns>
        TEntity Retrieve(TId id);

        /// <summary>
        /// Retrieves all entries for <see cref="TEntity"/> from the database with an optional <paramref name="where"/> clause, <paramref name="searchString"/> and
        /// paging options.
        /// </summary>
        /// <param name="where">Optional WHERE expression to filter results.</param>
        /// <param name="searchString">Search string to filter results.</param>
        /// <param name="skip">Number of entries to skip during pagination.</param>
        /// <param name="take">Number of entries to take during pagination. If zero, results won't be paginated.</param>
        /// <returns>Enumeration of <see cref="TEntity"/> found in the database.</returns>
        IEnumerable<TEntity> RetrieveAll(Expression<Func<TEntity, bool>> where = null, string searchString = null, int skip = 0, int take = 0, string orderBy = null, bool orderAsc = true);

        /// <summary>
        /// Removes an entity from the database by ID.
        /// </summary>
        /// <param name="id">ID of the entity to delete.</param>
        /// <param name="save">When <c>true</c>, changes are saved after removal. Otherwise, the <see cref="Save"/>method must be called after this call.</param>
        /// <returns><c>true</c> when the removal of the entity was successful, otherwise <c>false</c>.</returns>
        bool Delete(TId id, bool save = true);

        /// <summary>
        /// Determines wether the Entity with the specified <paramref name="id"/> exists in the database.
        /// </summary>
        /// <param name="id">ID of the entity to find.</param>
        /// <returns><c>true</c> when the entity with the specified <paramref name="id"/> exists in the database, otherwise <c>false</c></returns>
        bool Exists(TId id);

        /// <summary>
        /// Returns the total number of entities in the database that match the specified <paramref name="where"/> clause and <paramref name="searchString"/>.
        /// </summary>
        /// <param name="where">Expression to filter entities.</param>
        /// <param name="searchString">String to look for into the entities.</param>
        /// <returns>Number of entities that match the specified <paramref name="where"/> clause and search string.</returns>
        int Count(Expression<Func<TEntity, bool>> where = null, string searchString = null);
    }
}
