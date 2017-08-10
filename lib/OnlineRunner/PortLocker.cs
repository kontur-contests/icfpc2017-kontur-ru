using System;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using FluentAssertions;
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
            rootQuery = fb.Child("portLocks_new");
        }

        public static async Task<FirebaseClient> Connect()
        {
            return new FirebaseClient(
                "https://icfpc2017.firebaseio.com", new FirebaseOptions
                {
                });
        }

        public bool CheckIfPortIsFree(int portNumber)
        {
            var portChild = rootQuery.Child(portNumber.ToString());
            var resp = portChild.PostAsync(GetValidToTimestamp()).ConfigureAwait(false)
                .GetAwaiter().GetResult();

            var dateTimes = portChild.OnceAsync<DateTime>().ConfigureAwait(false).GetAwaiter().GetResult();

            var now = DateTime.UtcNow;
            var firebaseObject = dateTimes.First(x => x.Object >= now);
            
            portChild.Child(resp.Key).DeleteAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            return firebaseObject.Key == resp.Key;
        }

        public bool TryAcquire(int portNumber)
        {
            if (!CheckIfPortIsFree((portNumber))) return false;
            
            var portChild = rootQuery.Child(portNumber.ToString());
            portChild.DeleteAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            portChild.PostAsync(GetValidToTimestamp()).ConfigureAwait(false).GetAwaiter().GetResult();
            return true;
        }

        private static DateTime GetValidToTimestamp()
        {
            return DateTime.UtcNow + TimeSpan.FromMinutes(5);
        }

        public void Free(int portNumber)
        {
            var port = portNumber.ToString();
            rootQuery.Child(port).DeleteAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public void Clear()
        {
            rootQuery.DeleteAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }

    [TestFixture]
    public class PortLocker_Should
    {
        private readonly PortLocker pl = new PortLocker();

        [Test]
        [Explicit]
        public void TryAcquire()
        {
            var fb = PortLocker.Connect().ConfigureAwait(false).GetAwaiter().GetResult();
            var root = fb.Child("portLocks_new").Child("1000");
            pl.Clear();
            root.PostAsync(new DateTime(1900)).ConfigureAwait(false).GetAwaiter().GetResult();
            pl.TryAcquire(1000).Should().BeTrue();
            pl.TryAcquire(1000).Should().BeFalse();
            pl.Clear();
        }
        
        [Test, Explicit]
        public void ClearLocks()
        {
            pl.Clear();
        }
    }
}