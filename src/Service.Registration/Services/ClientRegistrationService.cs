using System;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using MyJetWallet.Domain;
using MyJetWallet.Domain.ServiceBus.Models;
using MyNoSqlServer.Abstractions;
using Service.ClientWallets.Grpc;
using Service.Registration.Grpc;
using Service.Registration.Grpc.Models;
using Service.Registration.NoSql;

namespace Service.Registration.Services
{
    public class ClientRegistrationService: IClientRegistrationService
    {
        private readonly IMyNoSqlServerDataWriter<ClientRegistrationResponseNoSql> _writer;
        private readonly IPublisher<ClientRegistrationMessage> _publisher;
        private readonly IClientWalletService _clientWalletService;

        public ClientRegistrationService(IMyNoSqlServerDataWriter<ClientRegistrationResponseNoSql> writer,
            IPublisher<ClientRegistrationMessage> publisher,
            IClientWalletService clientWalletService)
        {
            _writer = writer;
            _publisher = publisher;
            _clientWalletService = clientWalletService;
        }

        public async Task<ClientRegistrationResponse> GetOrRegisterClientAsync(JetClientIdentity clientId)
        {
            try
            {
                return await CreateClientRegistrationAsync(clientId);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Cannot create client {clientId.BrokerId}/{clientId.BrandId}/{clientId.ClientId}, exception: {ex}");
            }

            return await CreateClientRegistrationAsync(clientId);
        }

        private async Task<ClientRegistrationResponse> CreateClientRegistrationAsync(JetClientIdentity clientId)
        {
            var client = await _writer.GetAsync(
                ClientRegistrationResponseNoSql.GeneratePartitionKey(clientId.BrokerId),
                ClientRegistrationResponseNoSql.GenerateRowKey(clientId.ClientId));

            if (client != null)
            {
                if (client.ClientRegistration.ClientId.BrandId != clientId.BrandId)
                {
                    return new ClientRegistrationResponse()
                        {Result = ClientRegistrationResponse.RegistrationResult.ClientAlreadyRegisterWithOtherBrand};
                }

                return client.ClientRegistration;
            }

            client = ClientRegistrationResponseNoSql.Create(new ClientRegistrationResponse()
            {
                Result = ClientRegistrationResponse.RegistrationResult.Ok,
                ClientId = clientId,
                RegisterTime = DateTime.UtcNow
            });

            await _clientWalletService.GetWalletsByClient(clientId);

            await _writer.InsertAsync(client);

            await _publisher.PublishAsync(new ClientRegistrationMessage()
            {
                ClientId = client.ClientRegistration.ClientId,
                RegisterTime = client.ClientRegistration.RegisterTime
            });

            return client.ClientRegistration;
        }
    }
}