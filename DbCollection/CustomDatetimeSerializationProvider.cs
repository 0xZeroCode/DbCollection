using System;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace DbListTest
{
    public class CustomDatetimeSerializationProvider : IBsonSerializationProvider
    {
        public IBsonSerializer GetSerializer(Type type)
        {
            return type == typeof (DateTime) ? DateTimeSerializer.LocalInstance : null;
        }
    }
}