using Microsoft.EntityFrameworkCore;

namespace WireMock.Pages.WireMockService
{
    public class PaginatedList<T> : List<T>
    {
        public int Page { get; private set; }
        public int TotalPages { get; private set; }

        public bool HasPreviousPage => Page > 1;

        public bool HasNextPage => Page < TotalPages;

        public PaginatedList(List<T> items, int count, int page, int pageSize)
        {
            Page = page;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            AddRange(items);
        }

        
        public static PaginatedList<T> CreatePage(
            IList<T> source, int page, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip(
                    (page - 1) * pageSize)
                .Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, page, pageSize);
        }
    }
}