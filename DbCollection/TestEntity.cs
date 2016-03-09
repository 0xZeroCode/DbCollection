using System.Text;

namespace DbListTest
{
    public class TestEntity : IEntity
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public object Id { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("Id: {0}", Id);
            builder.AppendLine();
            builder.AppendFormat("Name: {0}", Name);
            builder.AppendLine();
            builder.AppendFormat("Number: {0}", Number);
            builder.AppendLine();

            return builder.ToString();
        }
    }
}