using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TestJaya.Data.DTO;
using TestJaya.Data.Filters;
using TestJaya.Data.Models;

namespace TestJaya.Business.Services
{
    /// <summary>
    /// Common contract for business services that work with the given <typeparamref name="TEntity"/>
    /// and return results as <typeparamref name="TDto"/>.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity</typeparam>
    /// <typeparam name="TDto">Type of DTO</typeparam>
    /// <typeparam name="TId">Type of Id from the entity and the DTO.</typeparam>
    public interface IStoredDataService<TEntity, TDto, TId>
        where TEntity : IEntity<TId>
    {
        /// <summary>
        /// Creates a new entity using the data sent in the <paramref name="dto"/>
        /// </summary>
        /// <param name="dto">DTO that contains the data for the new entity.</param>
        /// <param name="save">Indicates wether the transaction should be immediatly commited or not.
        /// <returns>DTO containing the data of the newly created entity.</returns>
        TDto Create(TDto dto, bool save = true);

        /// <summary>
        /// Retrieves an entity from the database and sets its data as a DTO in the result.
        /// </summary>
        /// <param name="id">Id of the entity to retrieve.</param>
        /// <returns>DTO with the data of the entity to retrieve.</returns>
        TDto Retrieve(TId id);

        /// <summary>
        /// Retrieves a <see cref="PagedResult{TDto}"/> containing DTOs representing entities of the type handled by this service from the database, optionally
        /// filtered by a <paramref name="extraFilter"/> clause and a <paramref name="pagedSearchAndFilterInfo"/>.
        /// </summary>
        /// <param name="pagedSearchAndFilterInfo">Contains all the information for the dynamic filtering, paging, ordering and sorting of the results.</param>
        /// <param name="extraFilter">Adds an additional expression to filter the results.</param>
        /// <returns><see cref="PagedResult{TDto}"/> containing entity information.</returns>
        PagedResult<TDto> RetrieveAll(PagedSearchAndFilterInfo pagedSearchAndFilterInfo, Expression<Func<TEntity, bool>> extraFilter = null);

        /// <summary>
        /// Retrieves a <see cref="PagedResult{TDto}"/> containing DTOs representing entities of the type handled by this service from the database, optionally
        /// filtered by a <paramref name="where"/> clause and a <paramref name="searchString"/>.
        /// </summary>
        /// <param name="where">Expression to filter the results.</param>
        /// <param name="searchString">Search terms to filter the results.</param>
        /// <param name="skip">Number of entries to skip during pagination.</param>
        /// <param name="take">Number of entries to take during pagination. If zero, results won't be paginated.</param>
        /// <param name="orderBy">Name of the property to sort the results.</param>
        /// <param name="orderAsc">Indicates wether the results should be sorted in ascending (<c>true</c>) or descending (<c>false</c>) order.</param>
        /// <returns><see cref="PagedResult{TDto}"/> containing entity information.</returns>
        PagedResult<TDto> RetrieveAll(Expression<Func<TEntity, bool>> where = null, string searchString = null, int skip = 0, int take = 0, string orderBy = null, bool orderAsc = true);

        /// <summary>
        /// Retrieves a <see cref="PagedResult{TDto}"/> containing DTOs representing instances of <typeparamref name="TOtherEntity"/> from the database, optionally
        /// filtered by a <paramref name="where"/> clause and a <paramref name="searchString"/>.
        /// </summary>
        /// <typeparam name="TOtherEntity">Type of the entity to retrieve.</typeparam>
        /// <typeparam name="TOtherDto">Type of the DTO to package the results.</typeparam>
        /// <typeparam name="TOtherId">Type of the Id of both <typeparamref name="TOtherEntity"/> and <typeparamref name="TOtherDto"/>.</typeparam>
        /// <param name="where">Expression to filter the results.</param>
        /// <param name="searchString">Search terms to filter the results.</param>
        /// <param name="skip">Number of entries to skip during pagination.</param>
        /// <param name="take">Number of entries to take during pagination. If zero, results won't be paginated.</param>
        /// <param name="orderBy">Name of the property to sort the results.</param>
        /// <param name="orderAsc">Indicates wether the results should be sorted in ascending (<c>true</c>) or descending (<c>false</c>) order.</param>
        /// <returns><see cref="PagedResult{TDto}"/> containing entity information.</returns>
        PagedResult<TOtherDto> RetrieveAll<TOtherEntity, TOtherDto, TOtherId>(Expression<Func<TOtherEntity, bool>> where = null, string searchString = null, int skip = 0, int take = 0, string orderBy = null, bool orderAsc = true)
            where TOtherEntity : class, IEntity<TOtherId>
            where TOtherDto : class, IDto<TOtherId>
            where TOtherId : IEquatable<TOtherId>;

        /// <summary>
        /// Updates an entity with the data specified in the <paramref name="dto"/>.
        /// </summary>
        /// <param name="dto">DTO that contains the data to update an entity.</param>
        /// <returns>DTO that contains the data of the updated entity.</returns>
        TDto Update(TDto dto);

        /// <summary>
        /// Deletes the entity identified by <paramref name="id"/>
        /// </summary>
        /// <param name="id">ID of the entity to delete.</param>
        /// <returns><c>true</c> when the deletion was successful, otherwise false.</returns>
        bool Delete(TId id);

        /// <summary>
        /// Determines wether the Entity with the specified <paramref name="id"/> exists in the database.
        /// </summary>
        /// <param name="id">ID of the entity to find.</param>
        /// <returns><c>true</c> when the entity with the specified <paramref name="id"/> exists in the database, otherwise <c>false</c></returns>
        bool Exists(TId id);

        /// <summary>
        /// Retrieves the number of entities that match the optional specified <paramref name="where"/> clause and <paramref name="searchString"/>.
        /// </summary>
        /// <param name="where">Filtering expression for the entities to count.</param>
        /// <param name="searchString">>Search terms to filter the entities to count.</param>
        /// <returns>Number of entities that match the optional specified <paramref name="where"/> clause and <paramref name="searchString"/>.</returns>
        int Count(Expression<Func<TEntity, bool>> where = null, string searchString = null);

        /// <summary>
        /// Retrieves a list of all the identifiers for the entity managed by this service.
        /// </summary>
        /// <param name="filters">Filters to apply</param>
        /// <returns>List of identifiers.</returns>
        IEnumerable<TId> GetAllIds(PagedSearchAndFilterInfo filters);

    }
}
