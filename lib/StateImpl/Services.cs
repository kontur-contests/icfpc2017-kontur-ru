using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Shouldly;

namespace lib.StateImpl
{
    public class Services : IServices
    {
        private readonly State state;
        private readonly Dictionary<Type, IService> services = new Dictionary<Type, IService>();

        public Services(State state)
        {
            this.state = state;
        }

        public T Get<T>() where T : IService
        {
            return (T) Get(typeof(T));
        }

        private IService Get(Type type)
        {
            IService service;
            if (services.TryGetValue(type, out service))
            {
                if (service == null)
                    throw new InvalidOperationException($"Cyclic dependency near the {type.Name} service");
                return service;
            }

            services.Add(type, null);
            var constructor = GetConstructor(type);
            service = (IService)Activator.CreateInstance(type, CreateArguments(constructor.GetParameters()));
            services[type] = service;
            return service;
        }

        private object[] CreateArguments(ParameterInfo[] parameterInfos)
        {
            var result = new List<object>();
            foreach (var parameterInfo in parameterInfos)
            {
                result.Add(CreateArgument(parameterInfo));
            }
            return result.ToArray();
        }

        private object CreateArgument(ParameterInfo parameterInfo)
        {
            if (typeof(IServices).IsAssignableFrom(parameterInfo.ParameterType))
                return this;
            if (typeof(IService).IsAssignableFrom(parameterInfo.ParameterType))
                return Get(parameterInfo.ParameterType);
            if (parameterInfo.ParameterType == typeof(State))
                return state;
            if (parameterInfo.Name.Equals("isSetupStage", StringComparison.OrdinalIgnoreCase))
                return state.IsSetupStage();
            var stateFields = typeof(State).GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var stateField in stateFields)
            {
                if (!stateField.FieldType.IsValueType && stateField.FieldType == parameterInfo.ParameterType)
                    return stateField.GetValue(state);
                if (stateField.FieldType.IsValueType && stateField.FieldType == parameterInfo.ParameterType && stateField.Name.Equals(parameterInfo.Name, StringComparison.OrdinalIgnoreCase))
                    return stateField.GetValue(state);
            }
            throw new InvalidOperationException($"Couldn't create parameter {parameterInfo} for service {parameterInfo.Member.DeclaringType}");
        }

        private static ConstructorInfo GetConstructor(Type type)
        {
            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            if (constructors.Length == 0)
                throw new InvalidOperationException($"Type {type} has no public instance constructors");
            if (constructors.Length == 1)
                return constructors[0];
            constructors = constructors.Where(x => x.GetParameters().Length != 0).ToArray();
            if (constructors.Length == 1)
                return constructors[0];
            throw new InvalidOperationException($"Type {type} has no too many public instance constructors");
        }
    }

    [TestFixture]
    public class Services_Test
    {
        private class ParameterlessService : IService
        {
        }

        private class IsSetupStageService : IService
        {
            public bool IsSetupStage { get; }

            public IsSetupStageService(bool isSetupStage)
            {
                IsSetupStage = isSetupStage;
            }
        }

        private class ParameterfulService : IService
        {
            public ParameterlessService Service { get; }
            public State State { get; }
            public Map Map { get; }
            public int Punter { get; }

            public ParameterfulService(ParameterlessService service, State state, Map map, int punter)
            {
                Service = service;
                State = state;
                Map = map;
                Punter = punter;
            }
        }

        [Test]
        public void Parameterless()
        {
            var services = new Services(new State());
            var service = services.Get<ParameterlessService>();
            service.ShouldNotBeNull();
        }

        [Test]
        public void IsSetupStage_True()
        {
            var services = new Services(new State());
            var service = services.Get<IsSetupStageService>();
            service.IsSetupStage.ShouldBe(true);
        }

        [Test]
        public void IsSetupStage_False()
        {
            var services = new Services(new State { turns = { new TurnState() } });
            var service = services.Get<IsSetupStageService>();
            service.IsSetupStage.ShouldBe(false);
        }

        [Test]
        public void Parameterful()
        {
            var map = new Map();
            var state = new State
            {
                punter = 42,
                map = map
            };
            var services = new Services(state);
            var service = services.Get<ParameterfulService>();
            service.State.ShouldBeSameAs(state);
            service.Map.ShouldBeSameAs(map);
            service.Punter.ShouldBe(42);
            service.Service.ShouldNotBeNull();
        }

        [Test]
        public void GetTwice_ReturnsSameInstance()
        {
            var services = new Services(new State());
            var service1 = services.Get<ParameterlessService>();
            var service2 = services.Get<ParameterlessService>();
            service2.ShouldBeSameAs(service1);
        }
    }
}