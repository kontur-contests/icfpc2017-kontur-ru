using System;
using Newtonsoft.Json;
using NUnit.Framework;

namespace lib
{
    [TestFixture]
    public class Map_Should
    {
        [Test]
        public void Deserialize()
        {
            var json = @"{""sites"":[{""id"":4},{""id"":1},{""id"":3},{""id"":6},{""id"":5},{""id"":0},{""id"":7},{""id"":2}],""rivers"":[{""source"":3,""target"":4,""owner"":-1},{""source"":0,""target"":1,""owner"":-1},{""source"":2,""target"":3,""owner"":-1},{""source"":1,""target"":3,""owner"":-1},{""source"":5,""target"":6,""owner"":-1},{""source"":4,""target"":5,""owner"":-1},{""source"":3,""target"":5,""owner"":-1},{""source"":6,""target"":7,""owner"":-1},{""source"":5,""target"":7,""owner"":-1},{""source"":1,""target"":7,""owner"":-1},{""source"":0,""target"":7,""owner"":-1},{""source"":1,""target"":2,""owner"":-1}],""mines"":[1,5]}";
            var x = JsonConvert.DeserializeObject<Map>(json);

            string deserialized = JsonConvert.SerializeObject(x);
            Console.WriteLine(deserialized);
            Assert.AreEqual(json, deserialized);
        }
    }
}