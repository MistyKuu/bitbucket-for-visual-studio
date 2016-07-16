using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitBucket.REST.API.Serializers;
using NUnit.Framework;

namespace Bitbucket.REST.API.Integration.Tests.SerializerTests
{
    [TestFixture]
    class BitBucketSerializerTests
    {
        [Test]
        public void shouldThrowUnauthorizedException()
        {
            var testInstance = new TestObject() {Age = 5, Name = "Kamil"};
            var serializer = new NewtonsoftJsonSerializer();

            var result = serializer.Serialize(testInstance);

            var test2 = "{\r\n  \"name\": \"Kamil\",\r\n  \"age\": 5\r\n}";
            var des = serializer.Deserialize<TestObject>(result);

            var des2 = serializer.Deserialize<TestObject>(test2);
            Console.WriteLine();
            var test = 0;

        }


        public class TestObject
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
    }

  
}
