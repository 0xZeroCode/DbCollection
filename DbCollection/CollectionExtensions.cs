using System.Collections.Generic;

namespace DbListTest
{
    public static class CollectionExtensions
    {
        public static long Count<T>(this ICollection<T> colletion) where T : IEntity
        {
            // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
            if (colletion is IEntityCollection<T>)
            {
                return ((IEntityCollection<T>) colletion).Count;
            }

            return colletion.Count;
        }
    }
}