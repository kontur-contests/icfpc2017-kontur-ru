using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace worker
{
    public class TaskPlayer
    {
        public string Name { get; set; }
        public List<int> Values { get; set; }
    }
    
    public class Task
    {
        public List<TaskPlayer> Players { get; set; }
    }

    public class ResultPlayer
    {
        public string Name { get; set; }
        public int Rank { get; set; }
    }

    public class Result
    {
        public IEnumerable<ResultPlayer> Players { get; set; }
    }

    public interface IPlayerStrategy
    {
        IEnumerable<TaskPlayer> Play(List<TaskPlayer> players);
    }
    
    public class DummySumPlayerStrategy : IPlayerStrategy
    {
        public IEnumerable<TaskPlayer> Play(List<TaskPlayer> players)
        {
            return players.OrderByDescending(player => player.Values.Sum());
        }
    }
    
    public interface IPlayer
    {
        Result Play(Task task);
    }
    
    public class Player : IPlayer
    {
        private readonly IPlayerStrategy playerStrategy;
        
        public Player(IPlayerStrategy playerStrategy)
        {
            this.playerStrategy = playerStrategy;
        }
        
        public Result Play(Task task)
        {
            var players = playerStrategy
                .Play(task.Players)
                .Select((player, i) => new ResultPlayer
                {
                    Name = player.Name,
                    Rank = i
                });

            return new Result
            {
                Players = players
            };
        }
    }
}