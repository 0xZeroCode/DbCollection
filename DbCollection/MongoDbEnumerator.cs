using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using MongoDB.Bson;

namespace DbListTest
{
    public class MongoDbEnumerator<T> : IEnumerator<T> where T : IEntity
    {
        private readonly Expression<Func<T, bool>> _filterExpression;
        private IEnumerator<T> _currentEnumerator;
        private IList<T> _currentList;
        private ObjectId _lastId;
        private Repository<T> _repository;


        public MongoDbEnumerator(string connectionString) : this(connectionString, t => true)
        {
        }

        public MongoDbEnumerator(string connectionString, Expression<Func<T, bool>> filterExpression)
        {
            _filterExpression = filterExpression;
            _repository = new Repository<T>(connectionString);
            _lastId = ObjectId.Empty;
            _currentList = _repository.GetOnePageList(filterExpression, _lastId);
            _currentEnumerator = _currentList.GetEnumerator();
        }

        public void Dispose()
        {
            _repository = null;
            _currentEnumerator.Dispose();
            _currentList = null;
            _lastId = ObjectId.Empty;
        }

        public bool MoveNext()
        {
            var hasMoved = _currentEnumerator.MoveNext();

            if (hasMoved)
            {
                return true;
            }

            _lastId = ObjectId.Parse(_currentList[_currentList.Count - 1].Id.ToString());
            _currentList = _repository.GetOnePageList(_filterExpression, _lastId);
            _currentEnumerator = _currentList.GetEnumerator();

            return _currentList.Count != 0;
        }

        public void Reset()
        {
            _lastId = ObjectId.Empty;
            _currentList = _repository.GetOnePageList(_filterExpression, _lastId);
            _currentEnumerator = _currentList.GetEnumerator();
            _currentEnumerator.Reset();
        }

        public T Current => _currentEnumerator.Current;

        object IEnumerator.Current => Current;
    }
}