using System.ServiceModel;
using System.Threading.Tasks;
using MyJetWallet.Domain;
using Service.Registration.Grpc.Models;

namespace Service.Registration.Grpc
{
    [ServiceContract]
    public interface IClientRegistrationService
    {
        [OperationContract]
        Task<ClientRegistrationResponse> GetOrRegisterClientAsync(JetClientIdentity clientId);
    }
}