using System.ServiceModel;
using System.Threading.Tasks;
using Service.Registration.Grpc.Models;

namespace Service.Registration.Grpc
{
    [ServiceContract]
    public interface IHelloService
    {
        [OperationContract]
        Task<HelloMessage> SayHelloAsync(HelloRequest request);
    }
}