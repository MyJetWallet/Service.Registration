using System;
using MyNoSqlServer.Abstractions;
using Service.Registration.Grpc.Models;

namespace Service.Registration.NoSql
{
    public class ClientRegistrationResponseNoSql: MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-client-registration";

        public static string GeneratePartitionKey(string brokerId) => brokerId;
        public static string GenerateRowKey(string clientId) => clientId;

        public ClientRegistrationResponse ClientRegistration { get; set; }

        public static ClientRegistrationResponseNoSql Create(ClientRegistrationResponse response)
        {
            return new ClientRegistrationResponseNoSql()
            {
                PartitionKey = GeneratePartitionKey(response.ClientId.BrokerId),
                RowKey = GenerateRowKey(response.ClientId.ClientId),
                ClientRegistration = response
            };
        }
    }
}
