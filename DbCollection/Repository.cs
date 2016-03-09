using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;

namespace DbListTest
{
    public class Repository<T> where T : IEntity
    {
        private const string IdFieldName = "Id";
        private readonly IMongoCollection<T> _collection;
        private readonly IMongoDatabase _db;

        public Repository()
            : this(ConfigurationManager.AppSettings["MongoConnectionString"])
        {
        }

        public Repository(string connectionString)
        {
            RegisterMaps();

            var mongoUrl = new MongoUrl(connectionString);

            var client = new MongoClient(mongoUrl);

            _db = client.GetDatabase(mongoUrl.DatabaseName);


            _collection = GetCollection();
        }

        public void Insert(T entity)
        {
            var task = _collection.InsertOneAsync(entity);

            task.Wait();

            CheckForDatabaseException(task);
        }

        public void Update(T entity)
        {
            entity.Id = ObjectId.Parse(entity.Id.ToString());

            var task = _collection.ReplaceOneAsync(IdFilter(entity.Id), entity);

            task.Wait();

            CheckForDatabaseException(task);
        }

        public T GetById(object id)
        {
            var task = _collection.Find(IdFilter(id)).FirstOrDefaultAsync();

            task.Wait();

            CheckForDatabaseException(task);

            return task.Result;
        }

        public IList<T> GetOnePageList(Expression<Func<T, bool>> expression, ObjectId lastId,
            int pageSize = 20)
        {
            FilterDefinition<T> filter = expression;
            var idFilter = Builders<T>.Filter.Lt(IdFieldName, lastId);

            if (lastId != ObjectId.Empty)
            {
                filter = Builders<T>.Filter.And(expression, idFilter);
            }

            var resultCursor = _collection.FindSync(filter,
                new FindOptions<T> {Limit = pageSize, Sort = Builders<T>.Sort.Descending(IdFieldName)});

            return resultCursor.ToList();
        }

        public void Delete(object id)
        {
            var filter = IdFilter(id);

            var task = _collection.DeleteOneAsync(filter);

            task.Wait();

            CheckForDatabaseException(task);
        }

        public IList<T> GetList(Expression<Func<T, bool>> expression)
        {
            //LogQuery(expression);

            var task = _collection.FindAsync(expression,
                new FindOptions<T> {Sort = Builders<T>.Sort.Descending(t => t.Id)});

            task.Wait();

            var resultTask = task.Result.ToListAsync();

            resultTask.Wait();

            CheckForDatabaseException(task);

            return resultTask.Result;
        }

        public void DropCollection()
        {
            _db.DropCollection(typeof (T).Name);
        }

        public long Count()
        {
            return _collection.Count(t => true);
        }

        public long Count(Expression<Func<T, bool>> expression)
        {
            return _collection.Count(expression);
        }

        private void RegisterMaps()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof (T))) return;

            BsonSerializer.RegisterSerializationProvider(new CustomDatetimeSerializationProvider());

            BsonClassMap.RegisterClassMap<T>(cm =>
            {
                cm.AutoMap();
                cm.MapIdProperty(c => c.Id)
                    .SetIdGenerator(ObjectIdGenerator.Instance);
            });
        }

        private static void CheckForDatabaseException(Task task)
        {
            if (task.Exception != null)
            {
                throw task.Exception;
            }
        }

        private IMongoCollection<T> GetCollection()
        {
            if (BsonClassMap.GetRegisteredClassMaps().All(x => x.Discriminator != typeof (T).Name))
                BsonClassMap.RegisterClassMap<T>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                    cm.MapIdProperty("Id");
                });

            return _db.GetCollection<T>(typeof (T).Name);
        }

        private static FilterDefinition<T> IdFilter(object id)
        {
            return Builders<T>.Filter.Eq("Id", ObjectId.Parse(id.ToString()));
        }

        private static void LogQuery(Expression<Func<T, bool>> expression)
        {
            var query = Builders<T>.Filter.Where(expression);

            var serializerRegistry = BsonSerializer.SerializerRegistry;
            var documentSerializer = serializerRegistry.GetSerializer<T>();

            Debug.WriteLine(query.Render(documentSerializer, serializerRegistry).ToJson());
        }
    }
}