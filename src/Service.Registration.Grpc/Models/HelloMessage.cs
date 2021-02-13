using System.Runtime.Serialization;
using Service.Registration.Domain.Models;

namespace Service.Registration.Grpc.Models
{
    [DataContract]
    public class HelloMessage : IHelloMessage
    {
        [DataMember(Order = 1)]
        public string Message { get; set; }
    }
}