namespace lib.StateImpl
{
    public static class ServicesExtensions
    {
        public static void Setup<T>(this IServices services, State state) where T : IService, new()
        {
            services.Get<T>(state);
        }
    }
}