using MyJetWallet.Sdk.Postgres;

namespace Service.Registration.Database.DesignTime
{
    public class ContextFactory : MyDesignTimeContextFactory<RegistrationContext>
    {
        public ContextFactory() : base(options => new RegistrationContext(options))
        {
        }
    }
}