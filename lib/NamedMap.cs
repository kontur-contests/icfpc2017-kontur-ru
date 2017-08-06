using System;
using JetBrains.Annotations;

namespace lib
{
    public class NamedMap
    {
        public NamedMap([NotNull] string name, [NotNull] Map map)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Map name is empty");
            Name = name;
            Map = map;
        }

        public bool IsOnline { get; set; }

        public int PlayersCount { get; set; }

        [NotNull]
        public string Name { get; }

        [NotNull]
        public Map Map { get; }

        public override string ToString()
        {
            return Name;
        }
    }

}