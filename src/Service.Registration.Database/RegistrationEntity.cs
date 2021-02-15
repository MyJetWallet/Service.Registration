using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Service.Registration.Grpc.Models;

namespace Service.Registration.Database
{
    [Table("registrations")]
    public class RegistrationEntity
    {
        public RegistrationEntity()
        {
        }

        public RegistrationEntity(string brokerId, string clientId, string brandId, DateTime registrationTime)
        {
            BrokerId = brokerId;
            ClientId = clientId;
            BrandId = brandId;
            RegistrationTime = registrationTime;
        }

        [Key]
        public string BrokerId { get; set; }

        [Key]
        public string ClientId { get; set; }

        public string BrandId { get; set; }

        public DateTime RegistrationTime { get; set; }
    }
}