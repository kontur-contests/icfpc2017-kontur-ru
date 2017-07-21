using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using NUnit.Framework;

namespace lib
{
    public class FbClient
    {
        private FirebaseClient fb;

        public FbClient()
        {
            fb = Authenticate().Result;
        }

        public IEnumerable<Problem> GetProblems()
        {
            return fb.Child("problems").OnceAsync<Problem>().ConfigureAwait(false).GetAwaiter().GetResult().Select(f => f.Object);
        }

        private static async Task<FirebaseClient> Authenticate()
        {
            var auth = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyBRYZvtAg1Vm5fZZ-r80vCISm0A8IhA7vM"));
            FirebaseAuthLink authLink = await auth.SignInWithEmailAndPasswordAsync("pe@kontur.ru", "W1nnerzz");
            var fb = new FirebaseClient(
                "https://icfpc2017.firebaseio.com",
                new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(authLink.FirebaseToken)
                });
            return fb;
        }
    }

    public class Problem
    {
        public int k;

        public override string ToString()
        {
            return $"{nameof(k)}: {k}";
        }
    }

    [TestFixture]
    public class FbClient_Should
    {
        [Test]
        public void DoSomething_WhenSomething()
        {
            var ps = new FbClient().GetProblems();
            Console.WriteLine(string.Join(" ", ps));
        }
    }
}