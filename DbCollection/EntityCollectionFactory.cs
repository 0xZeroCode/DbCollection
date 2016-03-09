using System.Configuration;

namespace DbListTest
{
    public class TestCollectionFactory
    {
        public static IEntityCollection<TestEntity> Create()
        {
            return new MongoDbCollection<TestEntity>(
                ConfigurationManager.ConnectionStrings["MongoConnectionString"].ConnectionString);
        }
    }
}