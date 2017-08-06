using System;
using System.Linq;
using System.Reflection;
using lib.Ai;

namespace lib.viz
{
    public class AiFactoryRegistry
    {
        public static readonly AiFactory[] Factories;
        public static readonly AiFactory[] ForOnlineRunsFactories;

        static AiFactoryRegistry()
        {
            var aiTypes = typeof(AiFactoryRegistry).Assembly.GetTypes()
                .Where(x => typeof(IAi).IsAssignableFrom(x) && x.GetConstructor(Type.EmptyTypes) != null)
                .Select(type => new {type, attr = type.GetCustomAttribute<ShouldNotRunOnlineAttribute>()})
                .Where(x => x.attr == null || !x.attr.DisableCompletely)
                .ToArray();
            Factories = aiTypes.Select(x => CreateFactory(x.type)).ToArray();
            ForOnlineRunsFactories = aiTypes
                .Where(x => x.attr == null)
                .Select(x => CreateFactory(x.type))
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

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ShouldNotRunOnlineAttribute : Attribute
    {
        public bool DisableCompletely { get; set; }
    }
}