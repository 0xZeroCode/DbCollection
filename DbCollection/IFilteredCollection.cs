using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DbListTest
{
    public interface IFilteredCollection<T> : IReadOnlyCollection<T>
    {
        IPagedCollection<T> ToPagedCollection();
        IList<T> ToList();
        new long Count { get; }
    }
}