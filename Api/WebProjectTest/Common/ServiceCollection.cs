using System.Reflection;
using WebIServices.IBase;

namespace WebProjectTest.Common
{
    public static class ServiceCollection
    {
        /// <summary>
        /// 批量注册Service服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly"></param>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterAllImplementationsOfInterface(this IServiceCollection services, Assembly assembly, ServiceLifetime lifetime)
        {
            var typesToRegister = assembly.GetTypes()
                .Where(type => type.IsAbstract == true && type.IsInterface == true)
                .Where(type => type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IBaseService)));

            foreach (var type in typesToRegister)
            {
                var serviceType = type.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IBaseService));
                services.Add(new ServiceDescriptor(serviceType, type, lifetime));
            }

            return services;
        }
    }
}
