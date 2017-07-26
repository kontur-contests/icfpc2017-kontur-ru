using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;

namespace lib.Firebase
{
    public class FbClient
    {
        private readonly FirebaseClient fb;

        public FbClient()
        {
            fb = Authenticate().Result;
        }

        public IEnumerable<Problem> GetProblems()
        {
            return fb.Child("problems").OnceAsync<Problem>().ConfigureAwait(false).GetAwaiter().GetResult()
                .Select(f => f.Object);
        }

        private static async Task<FirebaseClient> Authenticate()
        {
            var auth = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyBRYZvtAg1Vm5fZZ-r80vCISm0A8IhA7vM"));
            
            //change apssword in firebase console before repo publishing!
            FirebaseAuthLink authLink = await auth.SignInWithEmailAndPasswordAsync("pe@kontur.ru", "W1nnerzz");
            var fb = new FirebaseClient(
                "https://icfpc2017.firebaseio.com",
                new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(authLink.FirebaseToken)
                });
            return fb;
        }

        public void SaveProblem(Problem problem)
        {
            fb.Child("problems").Child(problem.Id.ToString()).PutAsync(problem).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }

    public class Problem
    {
        public int Id;
        public DateTime Date;

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id} {Date}";
        }
    }
}