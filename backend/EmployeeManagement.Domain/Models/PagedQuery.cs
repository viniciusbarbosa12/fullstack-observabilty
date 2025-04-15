namespace EmployeeManagement.Domain.Models
{
    public class PagedQuery<TFilter>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public TFilter? Filter { get; set; }

        public void Normalize()
        {
            if (Page <= 0) Page = 1;
            if (PageSize <= 0) PageSize = 10;
            if (PageSize > 100) PageSize = 100;
        }
    }
}
