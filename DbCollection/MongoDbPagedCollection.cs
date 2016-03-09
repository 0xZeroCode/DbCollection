using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Bson;

namespace DbListTest
{
    public class MongoDbPagedCollection<T> : IPagedCollection<T> where T : IEntity
    {
        private const int DefaultPageSize = 3;
        private readonly string _connectionString;
        private readonly Expression<Func<T, bool>> _filterExpression;
        private readonly Repository<T> _repository;

        public MongoDbPagedCollection(string connectionString, Expression<Func<T, bool>> filterExpression)
        {
            _connectionString = connectionString;
            _filterExpression = filterExpression;
            _repository = new Repository<T>(connectionString);
            CurrentPage = _repository.GetOnePageList(_filterExpression, ObjectId.Empty, DefaultPageSize);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new MongoDbEnumerator<T>(_connectionString, _filterExpression);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public long Count => _repository.Count(_filterExpression);
        public int PageSize => DefaultPageSize;
        public ICollection<T> CurrentPage { get; private set; }

        public bool MoveNext()
        {
            var last = CurrentPage.LastOrDefault();

            if (last == null)
            {
                return false;
            }

            var result = _repository.GetOnePageList(_filterExpression, ObjectId.Parse(last.Id.ToString()), DefaultPageSize);

            if (result.Count == 0)
            {
                return false;
            }

            CurrentPage = result;

            return true;
        }

        int IReadOnlyCollection<T>.Count => (int) Count;

        public bool Contains(T item)
        {
            var result = _repository.GetById(item.Id);

            var filterFunc = _filterExpression.Compile();

            return result != null && filterFunc(result);
        }
    }
}