namespace BudgetFlow.Application.Common.Utils
{
    public class PaginatedList<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int? PageSize { get; set; } = 30;
        public int? PageNumber { get; set; } = 1;
        public int TotalCount { get; set; }
        public int TotalPages => PageSize.HasValue && PageSize.Value>0 ? (int)Math.Ceiling(TotalCount/)
        public PaginatedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            Items = items;
            TotalCount = count;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
        public PaginatedList<T> Create(List<T> source, int count, int pageNumber, int pageSize)
        {
            return new PaginatedList<T>(source, count, pageNumber, pageSize);
        }

    }
}
