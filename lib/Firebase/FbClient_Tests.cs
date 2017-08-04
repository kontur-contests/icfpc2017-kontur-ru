// pack: exclude
using System;
using System.Linq;
using MoreLinq;
using NUnit.Framework;
using Shouldly;

namespace lib.Firebase
{
    [TestFixture]
    public class FbClient_Tests
    {
        [Test, Explicit]
        public void FirebaseSmokeTest()
        {
            var client = new FbClient();
            var problem = new Problem { Id = 42, Date = DateTime.Now };
            client.SaveProblem(problem);
            var ps = client.GetProblems().ToList();
            ps.ShouldContain(p => p.Id == 42);
            Console.WriteLine(ps.ToDelimitedString(","));
            Console.WriteLine(problem);
        }
    }
}