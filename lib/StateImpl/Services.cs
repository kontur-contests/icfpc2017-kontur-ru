using System;
using System.Collections.Generic;
using System.Linq;

namespace lib.StateImpl
{
    public class Services : IServices
    {
        private class ServiceInfo
        {
            public IService instance;
            public int turn;
            public bool used;
        }

        private readonly Dictionary<Type, ServiceInfo> services = new Dictionary<Type, ServiceInfo>();

        public T Get<T>(State state) where T : IService, new()
        {
            var currentTurn = state.turns.Count;

            var result = (T)TryGetAlreadyInitialized(typeof(T), state);
            if (result == null)
            {
                var serviceInfo = new ServiceInfo {turn = currentTurn, used = true};
                services.Add(typeof(T), serviceInfo);
                result = new T();
                var isSetupStage = !state.turns.Any();
                if (isSetupStage)
                    result.Setup(state, this);
                else
                    result.ApplyNextState(state, this);
                serviceInfo.instance = result;
                serviceInfo.used = false;
            }
            return result;
        }

        private IService TryGetAlreadyInitialized(Type type, State state)
        {
            var currentTurn = state.turns.Count;
            ServiceInfo serviceInfo;
            if (!services.TryGetValue(type, out serviceInfo))
                return null;
            if (serviceInfo.used)
                throw new InvalidOperationException($"Cyclic dependency near the {type.Name} service");
            if (serviceInfo.turn != currentTurn)
            {
                serviceInfo.used = true;
                serviceInfo.turn = currentTurn;
                serviceInfo.instance.ApplyNextState(state, this);
                serviceInfo.used = false;
            }
            return serviceInfo.instance;
        }

        public void ApplyNextState(State state)
        {
            foreach (var type in services.Keys)
                TryGetAlreadyInitialized(type, state);
        }
    }
}