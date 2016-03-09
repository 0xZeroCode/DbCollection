using MongoDB.Bson;

namespace DbListTest
{
    public class Utils
    {
        public static object ParseObjectId(string objectId)
        {
            return ObjectId.Parse(objectId);
        }

        public static object ConvertStringToObjectId(object id)
        {
            return ParseObjectId(id.ToString());
        }
    }
}