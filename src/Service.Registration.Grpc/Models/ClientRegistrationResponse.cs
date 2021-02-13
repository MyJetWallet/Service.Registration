using System;
using System.Runtime.Serialization;
using MyJetWallet.Domain;

namespace Service.Registration.Grpc.Models
{
    [DataContract]
    public class ClientRegistrationResponse
    {
        [DataMember(Order = 1)] public RegistrationResult Result { get; set; }
        [DataMember(Order = 2)] public JetClientIdentity ClientId { get; set; }
        [DataMember(Order = 3)] public DateTime RegisterTime { get; set; }


        public enum RegistrationResult
        {
            Ok,
            ClientAlreadyRegisterWithOtherBrand
        }
    }
}