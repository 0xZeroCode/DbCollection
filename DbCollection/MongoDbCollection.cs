using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DbListTest
{
    public class MongoDbCollection<T> : IEntityCollection<T> where T : IEntity
    {
        private readonly string _connectionString;
        private readonly Repository<T> _repo;

        public MongoDbCollection(string connectionString)
        {
            _connectionString = connectionString;
            _repo = new Repository<T>(connectionString);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new MongoDbEnumerator<T>(_connectionString);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            _repo.Insert(item);
        }

        public void Clear()
        {
            _repo.DropCollection();
        }

        public bool Contains(T item)
        {
            return ContainsId(item.Id);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _repo.GetList(t => true).CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return RemoveById(item.Id);
        }

        int ICollection<T>.Count => (int) Count;

        public long Count => _repo.Count();

        public bool IsReadOnly => false;

        public T GetById(object id)
        {
            return _repo.GetById(id);
        }

        public bool ContainsId(object id)
        {
            var result = _repo.GetById(id);

            return result != null && !result.Equals(default(T));
        }

        public bool RemoveById(object id)
        {
            try
            {
                _repo.Delete(id);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IFilteredCollection<T> Filter(Expression<Func<T, bool>> predicate)
        {
            return new MongoDbFilteredCollection<T>(_connectionString, predicate);
        }

        public IPagedCollection<T> ToPagedCollection()
        {
            return new MongoDbPagedCollection<T>(_connectionString, t => true);
        }
    }
}