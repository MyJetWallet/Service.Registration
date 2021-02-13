using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using JetBrains.Annotations;
using MyJetWallet.Domain;
using MyJetWallet.Sdk.GrpcMetrics;
using MyNoSqlServer.DataReader;
using ProtoBuf.Grpc.Client;
using Service.Registration.Grpc;
using Service.Registration.Grpc.Models;
using Service.Registration.NoSql;

namespace Service.Registration.Client
{
    [UsedImplicitly]
    public class ClientRegistrationServiceWithCache: IClientRegistrationService
    {
        private readonly MyNoSqlReadRepository<ClientRegistrationResponseNoSql> _reader;
        private readonly IClientRegistrationService _client;

        public ClientRegistrationServiceWithCache(MyNoSqlReadRepository<ClientRegistrationResponseNoSql> reader,
            string clientRegistrationGrpcServiceUrl)
        {
            _reader = reader;
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress(clientRegistrationGrpcServiceUrl);
            var invoker = channel.Intercept(new PrometheusMetricsInterceptor());
            _client = invoker.CreateGrpcService<IClientRegistrationService>();
        }

        public Task<ClientRegistrationResponse> GetOrRegisterClientAsync(JetClientIdentity clientId)
        {
            var entity = _reader.Get(ClientRegistrationResponseNoSql.GeneratePartitionKey(clientId.BrokerId),
                ClientRegistrationResponseNoSql.GenerateRowKey(clientId.ClientId));

            if (entity != null)
            {
                if (entity.ClientRegistration.ClientId.BrandId != clientId.BrandId)
                {
                    return Task.FromResult(new ClientRegistrationResponse()
                    {
                        Result = ClientRegistrationResponse.RegistrationResult.ClientAlreadyRegisterWithOtherBrand
                    });
                }

                return Task.FromResult(entity.ClientRegistration);
            }

            return _client.GetOrRegisterClientAsync(clientId);
        }
    }
}
