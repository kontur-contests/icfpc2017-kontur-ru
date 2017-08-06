using System;
using System.Collections.Generic;
using System.Linq;

namespace lib.StateImpl
{
    public class Services : IServices
    {
        private readonly Dictionary<Type, IService> services = new Dictionary<Type, IService>();

        public T Get<T>(State state) where T : IService, new()
        {
            IService service;
            if (services.TryGetValue(typeof(T), out service))
            {
                if (service == null)
                    throw new InvalidOperationException($"Cyclic dependency near the {typeof(T).Name} service");
                return (T)service;
            }

            services.Add(typeof(T), null);
            var result = new T();
            var isSetupStage = !state.turns.Any();
            if (isSetupStage)
                result.Setup(state, this);
            else
                result.ApplyNextState(state, this);
            services[typeof(T)] = result;
            return result;
        }
    }
}