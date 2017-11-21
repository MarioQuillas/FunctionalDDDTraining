using CustomerManagement.Logic.Model;
using CustomerManagement.Logic.Utils;
using Microsoft.Practices.Unity;
using Unity.WebApi;

namespace CustomerManagement.Api.Utils
{
    public static class DIContainer
    {
        private static UnityContainer container;

        internal static void Init()
        {
            container = new UnityContainer();

            container.RegisterType<UnitOfWork, UnitOfWork>(new PerHttpRequestLifetime("UnitOfWork"));
            container.RegisterType<IEmailGateway, EmailGateway>(new TransientLifetimeManager());
        }

        public static UnityDependencyResolver GetDependencyResolver()
        {
            return new UnityDependencyResolver(container);
        }

        public static UnitOfWork ResolveUnitOfWork()
        {
            return container.Resolve<UnitOfWork>();
        }

        public static bool IsUnitOfWorkInstantiated()
        {
            return new PerHttpRequestLifetime("UnitOfWork").GetValue() != null;
        }
    }
}