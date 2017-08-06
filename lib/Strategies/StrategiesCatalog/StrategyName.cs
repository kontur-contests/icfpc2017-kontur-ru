using lib.Strategies.EdgeWeighting;

namespace lib.Strategies.StrategiesCatalog
{
    public class StrategyName
    {
        public static string ForSetup<TStrategy>()
            where TStrategy : ISetupStrategy
            => typeof(TStrategy).Name;

        public static string For<TStrategy>(string prefix = null) 
            where TStrategy : IStrategy 
            => prefix + typeof(TStrategy).Name;

        public static string ForEWStrategy<TMeta, TEdgeWeighter>() 
            where TEdgeWeighter : IEdgeWeighter
            where TMeta : IMetaStrategy
            => $"{typeof(TMeta).Name}_{typeof(TEdgeWeighter).Name}";
    }
}