namespace BudgetFlow.Application.Common.Utils;
public class PaginatedList<T>
{
    public IEnumerable<T> Items { get; }
    public int PageSize { get; }
    public int PageNumber { get; }
    public int TotalCount { get; }
    public int TotalPages { get; }
    public PaginatedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = count;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = PageSize > 0 ? ( int )Math.Ceiling(( double )TotalCount / PageSize) : 0;
    }
}
