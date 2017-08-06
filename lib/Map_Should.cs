using System;
using System.Collections.Generic;
using FluentAssertions;
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
            Console.WriteLine(JsonConvert.SerializeObject(x, Formatting.Indented));
            Assert.AreEqual(8, x.Sites.Length);
            Assert.AreEqual(12, x.Rivers.Length);
            Assert.AreEqual(new[]{1,5}, x.Mines);
        }

        [Test]
        public void TestSer()
        {
            var dct = new Dictionary<int, int>
            {
                {1, 2 },
                {10, 20 }
            };
            var serialized = JsonConvert.SerializeObject(dct);
            Console.Out.WriteLine(serialized);

            var newDct = JsonConvert.DeserializeObject<Dictionary<int, int>>(serialized);
            newDct.ShouldBeEquivalentTo(dct);
        }
    }
}