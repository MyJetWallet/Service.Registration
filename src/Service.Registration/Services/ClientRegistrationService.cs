using System;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using MyJetWallet.Domain;
using MyJetWallet.Domain.ServiceBus.Models;
using MyJetWallet.Sdk.Service;
using MyNoSqlServer.Abstractions;
using Service.ClientWallets.Grpc;
using Service.Registration.Database;
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
        private readonly IRegistrationRepository _registrationRepository;

        public ClientRegistrationService(IMyNoSqlServerDataWriter<ClientRegistrationResponseNoSql> writer,
            IPublisher<ClientRegistrationMessage> publisher,
            IClientWalletService clientWalletService,
            IRegistrationRepository registrationRepository)
        {
            _writer = writer;
            _publisher = publisher;
            _clientWalletService = clientWalletService;
            _registrationRepository = registrationRepository;
        }

        public async Task<ClientRegistrationResponse> GetOrRegisterClientAsync(JetClientIdentity clientId)
        {
            clientId.ClientId.AddToActivityAsTag("clientId");
            clientId.BrokerId.AddToActivityAsTag("brokerId");
            clientId.BrandId.AddToActivityAsTag("brandId");

            try
            {
                return await CreateClientRegistrationAsync(clientId);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Cannot create client {clientId.BrokerId}/{clientId.BrandId}/{clientId.ClientId}, exception: {ex}");
                ex.WriteToActivity();
            }

            try
            {
                return await CreateClientRegistrationAsync(clientId);
            }
            catch (Exception ex)
            {
                ex.FailActivity();
                throw;
            }
        }

        private async Task<ClientRegistrationResponse> CreateClientRegistrationAsync(JetClientIdentity clientId)
        {
            var client = await _writer.GetAsync(
                ClientRegistrationResponseNoSql.GeneratePartitionKey(clientId.BrokerId),
                ClientRegistrationResponseNoSql.GenerateRowKey(clientId.ClientId));

            var response = client?.ClientRegistration ?? await _registrationRepository.GetAsync(clientId.ClientId);

            if (response != null)
            {
                if (response.ClientId.BrokerId != clientId.BrokerId)
                {
                    return new ClientRegistrationResponse()
                        { Result = ClientRegistrationResponse.RegistrationResult.ClientAlreadyRegisterWithOtherBrand };
                }

                if (response.ClientId.BrandId != clientId.BrandId)
                {
                    return new ClientRegistrationResponse()
                        {Result = ClientRegistrationResponse.RegistrationResult.ClientAlreadyRegisterWithOtherBrand};
                }

                await _writer.InsertOrReplaceAsync(ClientRegistrationResponseNoSql.Create(response));

                return response;
            }

            response = new ClientRegistrationResponse()
            {
                Result = ClientRegistrationResponse.RegistrationResult.Ok,
                ClientId = clientId,
                RegisterTime = DateTime.UtcNow
            };

            await _registrationRepository.InsertAsync(response);

            await _clientWalletService.GetWalletsByClient(clientId);
            
            client = ClientRegistrationResponseNoSql.Create(response);

            await _writer.InsertOrReplaceAsync(client);

            await _publisher.PublishAsync(new ClientRegistrationMessage()
            {
                ClientId = client.ClientRegistration.ClientId,
                RegisterTime = client.ClientRegistration.RegisterTime
            });

            return client.ClientRegistration;
        }
    }
}