using SimpleTrading.SettingsReader;

namespace Service.Registration.Settings
{
    [YamlAttributesOnly]
    public class SettingsModel
    {
        [YamlProperty("Registration.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }
    }
}