using System.Collections.Generic;
using Topshelf;
using worker.Strategies;

namespace worker
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                
                x.Service<WorkerService>(s =>
                {
                    var player = new ExperimentSelector();
                    
                    var config = new Dictionary<string, object>
                    {
                        { "group.id", "icfpc2017-worker" },
                        { "bootstrap.servers", "icfpc-broker.dev.kontur.ru" }
                    };
                    s.ConstructUsing(name => new WorkerService(config, "tasks", "results", player));
                    s.WhenStarted(ws => ws.Start());
                    s.WhenStopped(ws => ws.Stop());
                });
                
                x.RunAsLocalSystem();

                x.SetDescription("ICFPC2017 Worker Service");
                x.SetDisplayName("ICFPC2017 Worker");
                x.SetServiceName("ICFPC2017 Worker");

                x.UseNLog();
            });
        }
    }
}