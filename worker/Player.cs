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
        public string Map { get; set; }
        public string Experiment { get; set; }
        public int Token { get; set; }
        public int Part { get; set; }
        public List<PlayerWithParams> Players { get; set; }
    }

    public class PlayerResult
    {
        public long Scores { get; set; }
        public string ServerName { get; set; }
    }
    
    public class Result
    {
        public List<PlayerResult> Results { get; set; }
        public string Error { get; set; }
        public Task Task { get; set; }
        public int RiversCount { get; set; }
        public int SitesCount { get; set; }
    }
}