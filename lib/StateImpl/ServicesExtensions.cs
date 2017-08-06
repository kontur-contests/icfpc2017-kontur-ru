namespace lib.StateImpl
{
    public static class ServicesExtensions
    {
        public static void Setup<T>(this IServices services) where T : IService
        {
            services.Get<T>();
        }
    }
}