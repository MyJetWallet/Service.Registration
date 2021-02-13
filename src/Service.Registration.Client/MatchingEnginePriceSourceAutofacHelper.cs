using Autofac;
using MyNoSqlServer.DataReader;
using Service.Registration.Grpc;
using Service.Registration.NoSql;
// ReSharper disable UnusedMember.Global

namespace Service.Registration.Client
{
    public static class ClientRegistrationAutofacHelper
    {
        /// <summary>
        /// Register interfaces:
        ///   * IClientRegistrationService
        /// </summary>
        public static void RegisterClientRegistrationClient(this ContainerBuilder builder, IMyNoSqlSubscriber myNoSqlSubscriber, string clientRegistrationGrpcServiceUrl)
        {
            var subs = new MyNoSqlReadRepository<ClientRegistrationResponseNoSql>(myNoSqlSubscriber, ClientRegistrationResponseNoSql.TableName);
            builder
                .RegisterInstance(new ClientRegistrationServiceWithCache(subs, clientRegistrationGrpcServiceUrl))
                .As<IClientRegistrationService>()
                .AutoActivate()
                .SingleInstance();

        }
    }
}