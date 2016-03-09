using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DbListTest
{
    public interface IEntityCollection<T> : ICollection<T> where T : IEntity
    {
        new long Count { get; }
        T GetById(object id);
        bool ContainsId(object id);
        bool RemoveById(object id);
        IFilteredCollection<T> Filter(Expression<Func<T, bool>> predicate);
        IPagedCollection<T> ToPagedCollection();
    }
}