using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Service.Registration.Database;

namespace Service.Registration.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<RegistrationRepository>()
                .As<IRegistrationRepository>();
        }
    }
}