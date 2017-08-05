using System;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace lib.OnlineRunner
{
    public class PortLocker
    {
        private readonly FirebaseClient fb;
        private readonly ChildQuery rootQuery;

        public PortLocker()
        {
            fb = Connect().ConfigureAwait(false).GetAwaiter().GetResult();
            rootQuery = fb.Child("portLocks");
        }

        private static async Task<FirebaseClient> Connect()
        {
            var auth = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyBRYZvtAg1Vm5fZZ-r80vCISm0A8IhA7vM"));
            var link = await auth.SignInWithEmailAndPasswordAsync("pe@kontur.ru", "W1nnerzz");

            return new FirebaseClient(
                "https://icfpc2017.firebaseio.com", new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(link.FirebaseToken)
                });
        }

        public bool TryAcquire(int portNumber, Guid workerId)
        {
            var port = portNumber.ToString();
            var ids = rootQuery.Child(port).OnceAsync<Guid>().ConfigureAwait(false).GetAwaiter().GetResult()
                .Select(o => o.Object).ToList();

            if (ids.Count != 0)
                return false;
            rootQuery.Child(portNumber.ToString()).PostAsync(workerId).GetAwaiter().GetResult();
            return true;
        }

        public void Free(int portNumber)
        {
            var port = portNumber.ToString();
            rootQuery.Child(port).DeleteAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private static string Encode<T>(T data)
        {
            return JsonConvert.SerializeObject(data);
        }

        private static T Decode<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }
    }

    [TestFixture]
    public class PortLocker_Should
    {
        private Guid g = Guid.Parse("d9abfe09-b31a-4dbf-9e8b-fb8dddc77ada");
        private readonly PortLocker pl = new PortLocker();
        private const int portNumber = 1000;

        [Test]
        [Explicit]
        public void TryAcquire()
        {
            pl.TryAcquire(portNumber, g).Should().BeTrue();
            pl.TryAcquire(portNumber, g).Should().BeFalse();
            pl.TryAcquire(portNumber, Guid.NewGuid()).Should().BeFalse();
            pl.Free(portNumber);
            pl.TryAcquire(portNumber, Guid.NewGuid()).Should().BeTrue();
            pl.Free(portNumber);            
        }


        [TearDown]
        public void TearDown()
        {
            pl.Free(portNumber);
        }
        
    }
}