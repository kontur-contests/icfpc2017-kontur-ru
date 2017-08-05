using System;
using System.Linq;
using lib.Ai;

namespace lib.viz
{
    public class AiFactoryRegistry
    {
        public static readonly AiFactory[] Factories;

        static AiFactoryRegistry()
        {
            var types = typeof(AiFactoryRegistry).Assembly.GetTypes()
                .Where(x => typeof(IAi).IsAssignableFrom(x) && x.GetConstructor(Type.EmptyTypes) != null);
            Factories = types.Select(type => new AiFactory(type.Name, () => (IAi) Activator.CreateInstance(type)))
                .ToArray();
        }

        public static IAi GetNextAi()
        {
            return Factories
                .Where(x => x.Name.Equals("GreedyAi") || x.Name.Equals("ConnectClosestMinesAi"))
                .OrderBy(x => Guid.NewGuid()).First().Create();
        }
    }
}