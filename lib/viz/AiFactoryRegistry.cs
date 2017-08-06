using System;
using System.Linq;
using lib.Ai;

namespace lib.viz
{
    public class AiFactoryRegistry
    {
        public static readonly AiFactory[] Factories;
        private static readonly AiFactory[] ForOnlineRunsFactories;

        static AiFactoryRegistry()
        {
            var aiTypes = typeof(AiFactoryRegistry).Assembly.GetTypes()
                .Where(x => typeof(IAi).IsAssignableFrom(x) && x.GetConstructor(Type.EmptyTypes) != null)
                .ToArray();
            Factories = aiTypes.Select(CreateFactory).ToArray();
            ForOnlineRunsFactories = aiTypes
                .Where(x => Attribute.GetCustomAttribute(x, typeof(ShouldNotRunOnlineAttribute)) == null)
                .Select(CreateFactory)
                .ToArray();
        }

        public static AiFactory CreateFactory(Type type)
        {
            return new AiFactory(type.Name, () => (IAi) Activator.CreateInstance(type));
        }

        public static AiFactory CreateFactory<TAi>() where TAi : IAi
        {
            return CreateFactory(typeof(TAi));
        }

        public static IAi GetNextAi(bool forOnlineRuns)
        {
            return (forOnlineRuns ? ForOnlineRunsFactories : Factories)
                .OrderBy(x => Guid.NewGuid())
                .First()
                .Create();
        }
    }

    public class ShouldNotRunOnlineAttribute : Attribute
    {
    }
}