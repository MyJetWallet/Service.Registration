using SimpleTrading.SettingsReader;

namespace Service.Registration.Settings
{
    [YamlAttributesOnly]
    public class SettingsModel
    {
        [YamlProperty("Registration.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("Registration.MyNoSqlWriterUrl")]
        public string MyNoSqlWriterUrl { get; set; }

        [YamlProperty("Registration.SpotServiceBusHostPort")]
        public string SpotServiceBusHostPort { get; set; }

        [YamlProperty("Registration.ClientWalletsGrpcServiceUrl")]
        public string ClientWalletsGrpcServiceUrl { get; set; }

        [YamlProperty("Registration.PostgresConnectionString")]
        public string PostgresConnectionString { get; set; }
    }
}