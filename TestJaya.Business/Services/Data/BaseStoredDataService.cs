using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using TestJaya.Business.Exceptions;
using TestJaya.Data.DAL;
using TestJaya.Data.DTO;
using TestJaya.Data.Filters;
using TestJaya.Data.Models;

namespace TestJaya.Business.Services.Data
{

    /// <summary>
    /// Base implementation of <see cref="IStoredDataService{TEntity, TDto, TId}"/>.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity</typeparam>
    /// <typeparam name="TDto">Type of DTO</typeparam>
    /// <typeparam name="TId">Type of Id from the entity and the DTO.</typeparam>
    public class BaseStoredDataService<TEntity, TDto, TId> : IStoredDataService<TEntity, TDto, TId>
        where TEntity : class, IEntity<TId>
        where TDto : class, IDto<TId>
        where TId : IEquatable<TId>
    {
        protected readonly IMapper Mapper;
        protected readonly IBaseDataRepository<TEntity, TId> Repository;

        public BaseStoredDataService(IBaseDataRepository<TEntity, TId> repository, IMapper mapper)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Creates a new entity using the data sent in the <paramref name="dto"/>
        /// </summary>
        /// <param name="dto">DTO that contains the data for the new entity.</param>
        /// <param name="save">Indicates wether the transaction should be immediatly commited or not.
        /// <returns>DTO containing the data of the newly created entity.</returns>
        public virtual TDto Create(TDto dto, bool save = true)
        {
            var validationResult = ValidateForCreation(dto);
            if (!validationResult.IsValid) throw new ValidationException(validationResult.Message);
            var entity = Mapper.Map<TDto, TEntity>(dto);
            entity = Repository.Create(entity, save);
            return Mapper.Map<TEntity, TDto>(entity);
        }

        /// <summary>
        /// Retrieves an entity from the database and sets its data as a DTO in the result.
        /// </summary>
        /// <param name="id">Id of the entity to retrieve.</param>
        /// <returns>DTO with the data of the entity to retrieve.</returns>
        public virtual TDto Retrieve(TId id)
        {
            var entity = Repository.Retrieve(id);
            if (entity == null) return null;
            return Mapper.Map<TEntity, TDto>(entity);
        }

        #region RetrieveAll

        /// <summary>
        /// Retrieves a <see cref="PagedResult{TDto}"/> containing DTOs representing entities of the type handled by this service from the database, optionally
        /// filtered by a <paramref name="extraFilter"/> clause and a <paramref name="pagedSearchAndFilterInfo"/>.
        /// </summary>
        /// <param name="pagedSearchAndFilterInfo">Contains all the information for the dynamic filtering, paging, ordering and sorting of the results.</param>
        /// <param name="extraFilter">Adds an additional expression to filter the results.</param>
        /// <returns><see cref="PagedResult{TDto}"/> containing entity information.</returns>
        public virtual PagedResult<TDto> RetrieveAll(PagedSearchAndFilterInfo pagedSearchAndFilterInfo, Expression<Func<TEntity, bool>> extraFilter = null)
        {
            if (pagedSearchAndFilterInfo == null)
            {
                if (extraFilter == null)
                {
                    return RetrieveAll();
                }
                else
                {
                    return RetrieveAll(extraFilter);
                }
            }

            return RetrieveAll<TEntity, TDto, TId>(pagedSearchAndFilterInfo, extraFilter);
        }

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
        public virtual PagedResult<TDto> RetrieveAll(Expression<Func<TEntity, bool>> where = null, string searchString = null, int skip = 0, int take = 0, string orderBy = null, bool orderAsc = true) => RetrieveAll<TEntity, TDto, TId>(where, searchString, skip, take, orderBy, orderAsc);

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
        public virtual PagedResult<TOtherDto> RetrieveAll<TOtherEntity, TOtherDto, TOtherId>(Expression<Func<TOtherEntity, bool>> where = null, string searchString = null, int skip = 0, int take = 0, string orderBy = null, bool orderAsc = true)
            where TOtherEntity : class, IEntity<TOtherId>
            where TOtherDto : class, IDto<TOtherId>
            where TOtherId : IEquatable<TOtherId>
        {
            var entities = Repository.RetrieveAll<TOtherEntity, TOtherId>(where, searchString, skip, take, orderBy, orderAsc);
            return packIntoPagedResult<TOtherEntity, TOtherDto, TOtherId>(entities, skip, take, Count<TOtherEntity, TOtherId>(where, searchString));
        }
        /// <summary>
        /// Retrieves a <see cref="PagedResult{TDto}"/> containing DTOs representing entities of the type handled by this service from the database, optionally
        /// filtered by a <paramref name="extraFilter"/> clause and a <paramref name="pagedSearchAndFilterInfo"/>. 
        /// </summary>
        /// <typeparam name="TOtherEntity">Type of the entity to retrieve.</typeparam>
        /// <typeparam name="TOtherDto">Type of the DTO to package the results.</typeparam>
        /// <typeparam name="TOtherId">Type of the Id of both <typeparamref name="TOtherEntity"/> and <typeparamref name="TOtherDto"/>.</typeparam>
        /// <param name="pagedSearchAndFilterInfo">Contains all the information for the dynamic filtering, paging, ordering and sorting of the results.</param>
        /// <param name="extraFilter">Adds an additional expression to filter the results.</param>
        /// <returns><see cref="PagedResult{TDto}"/> containing entity information.</returns>
        public virtual PagedResult<TOtherDto> RetrieveAll<TOtherEntity, TOtherDto, TOtherId>(PagedSearchAndFilterInfo pagedSearchAndFilterInfo, Expression<Func<TOtherEntity, bool>> extraFilter)
            where TOtherEntity : class, IEntity<TOtherId>
            where TOtherDto : class, IDto<TOtherId>
            where TOtherId : IEquatable<TOtherId>
        {
            var entities = Repository.RetrieveAll<TOtherEntity, TOtherId>(pagedSearchAndFilterInfo, extraFilter, out int totalOfRecords);
            return packIntoPagedResult<TOtherEntity, TOtherDto, TOtherId>(entities, pagedSearchAndFilterInfo.Offset, pagedSearchAndFilterInfo.Limit, totalOfRecords);
        }

        private PagedResult<TOtherDto> packIntoPagedResult<TOtherEntity, TOtherDto, TOtherId>(IEnumerable<TOtherEntity> entities, int skip, int take, long total)
            where TOtherEntity : class, IEntity<TOtherId>
            where TOtherDto : class, IDto<TOtherId>
            where TOtherId : IEquatable<TOtherId>
        {
            if (entities != null)
            {
                return new PagedResult<TOtherDto>(
                    Mapper.Map<IEnumerable<TOtherEntity>, IEnumerable<TOtherDto>>(entities), skip, take, total);
            }
            return new PagedResult<TOtherDto>(null, skip, take, total);
        }
        #endregion

        /// <summary>
        /// Updates an entity with the data specified in the <paramref name="dto"/>.
        /// </summary>
        /// <param name="dto">DTO that contains the data to update an entity.</param>
        /// <returns>DTO that contains the data of the updated entity.</returns>
        public virtual TDto Update(TDto dto)
        {
            var validationResult = ValidateForUpdate(dto);
            if (!validationResult.IsValid) throw new ValidationException(validationResult.Message);

            var entity = Repository.Retrieve(dto.Id);
            if (entity == null) throw new KeyNotFoundException();

            Mapper.Map(dto, entity);
            if (!Repository.Save()) throw new Exception();
            return Mapper.Map<TEntity, TDto>(entity);
        }

        /// <summary>
        /// Deletes the entity identified by <paramref name="id"/>
        /// </summary>
        /// <param name="id">ID of the entity to delete.</param>
        /// <returns><c>true</c> when the deletion was successful, otherwise false.</returns>
        public virtual bool Delete(TId id) => Repository.Delete(id);

        /// <summary>
        /// Determines wether the Entity with the specified <paramref name="id"/> exists in the database.
        /// </summary>
        /// <param name="id">ID of the entity to find.</param>
        /// <returns><c>true</c> when the entity with the specified <paramref name="id"/> exists in the database, otherwise <c>false</c></returns>
        public virtual bool Exists(TId id) => Repository.Exists(id);

        [ExcludeFromCodeCoverage] // This methods acts as a proxy for repository.Count, thus is excluded from coverage.
        public int Count(Expression<Func<TEntity, bool>> where = null, string searchString = null) => Count<TEntity, TId>(where, searchString);

        public int Count<TOtherEntity, TOtherEntityId>(Expression<Func<TOtherEntity, bool>> where = null, string searchString = null) where TOtherEntity : class, IEntity<TOtherEntityId> => Repository.Count<TOtherEntity, TOtherEntityId>(where, searchString);

        public IEnumerable<TId> GetAllIds(PagedSearchAndFilterInfo filters) => Repository.RetrieveAll<TEntity, TId>(filters, null, out int totalOfRecords).Select(e => e.Id);

        protected virtual ValidationResult ValidateForCreation(TDto dto)
        {
            var result = new ValidationResult();
            if (dto == null) result.AppendMessage("No data was provided.");
            else dto.Id = default(TId);
            return result;
        }

        protected virtual ValidationResult ValidateForUpdate(TDto dto)
        {
            var result = new ValidationResult();
            if (dto == null) result.AppendMessage("No data was provided.");
            else
            {
                if (dto.Id.Equals(default(TId))) result.AppendMessage("Invalid ID");
            }
            return result;
        }
    }
}
