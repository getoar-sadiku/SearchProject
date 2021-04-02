using System.Collections.Generic;

namespace SearchProject.Infrastructure
{
    public class PagedResponse<T> : IPagedResponse<T> where T : class
    {
        public PagedResponse()
        {

        }
        public IList<T> Records { get; set; }
        public long TotalCount { get; set; }
    }
}
