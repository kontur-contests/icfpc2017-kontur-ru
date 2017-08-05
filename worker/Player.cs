using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace worker
{
    public class PlayerWithParams
    {
        public string Name { get; set; }
        public Dictionary<string, double> Params { get; set; }
        public long Rank { get; set; } = 0;
    }
    
    public class Task
    {
        public List<PlayerWithParams> Players { get; set; }
    }

    public class Result
    {
        public IEnumerable<PlayerWithParams> Players { get; set; }
    }

    public interface IPlayer
    {
        Result Play(Task task);
    }
    
    public class Player : IPlayer
    {
        private readonly IExperiment playerStrategy;
        
        public Player(IExperiment playerStrategy)
        {
            this.playerStrategy = playerStrategy;
        }
        
        public Result Play(Task task)
        {
            var players = playerStrategy
                .Play(task)
                .Select(pair => new PlayerWithParams
                {
                    Name = pair.Item1.Name,
                    Params = pair.Item1.Params,
                    Rank = pair.Item2
                });

            return new Result
            {
                Players = players
            };
        }
    }
}