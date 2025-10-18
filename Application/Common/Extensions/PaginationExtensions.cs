using Application.Common.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Extensions;

public static class PaginationExtensions
{
    public static Task<PagedList<TDestination>> ToPaginatedListAsync<TDestination>(
        this IQueryable<TDestination> queryable, int pageNumber, int pageSize) where TDestination : class
    {
        return PagedList<TDestination>.CreateAsync(queryable.AsNoTracking(), pageNumber, pageSize);
    }
}