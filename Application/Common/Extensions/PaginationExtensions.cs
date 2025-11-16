using Application.Common.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Extensions;

public static class PaginationExtensions
{
    extension<TDestination>(IQueryable<TDestination> queryable) where TDestination : class
    {
        public Task<PagedList<TDestination>> ToPaginatedListAsync(int pageNumber, int pageSize)
        {
            return PagedList<TDestination>.CreateAsync(queryable.AsNoTracking(), pageNumber, pageSize);
        }
    }
}