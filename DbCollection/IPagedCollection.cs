using System.Collections.Generic;

namespace DbListTest
{
    public interface IPagedCollection<T> : IReadOnlyCollection<T>
    {
        int PageSize { get; }
        ICollection<T> CurrentPage { get; }
        new long Count { get; }
        bool MoveNext();
    }
}