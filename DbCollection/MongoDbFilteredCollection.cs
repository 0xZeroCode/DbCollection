using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DbListTest
{
    public class MongoDbFilteredCollection<T> : IFilteredCollection<T> where T : IEntity
    {
        private readonly string _connectionString;
        private readonly Expression<Func<T, bool>> _filterExpression;
        private readonly Repository<T> _repository;

        public MongoDbFilteredCollection(string connectionString, Expression<Func<T, bool>> filterExpression)
        {
            _connectionString = connectionString;
            _filterExpression = filterExpression;
            _repository = new Repository<T>(_connectionString);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new MongoDbEnumerator<T>(_connectionString, _filterExpression);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        int IReadOnlyCollection<T>.Count => (int) Count;

        public long Count => _repository.Count(_filterExpression);

        public IPagedCollection<T> ToPagedCollection()
        {
            return new MongoDbPagedCollection<T>(_connectionString, _filterExpression);
        }

        public IList<T> ToList()
        {
            return _repository.GetList(_filterExpression);
        }
    }
}