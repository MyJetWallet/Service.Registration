using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyJetWallet.Domain;
using Service.Registration.Grpc.Models;

namespace Service.Registration.Database
{
    public interface IRegistrationRepository
    {
        Task InsertAsync(ClientRegistrationResponse response);
        Task<ClientRegistrationResponse> GetAsync(string clientId);
    }

    public class RegistrationRepository: IRegistrationRepository
    {
        private readonly DbContextOptionsBuilder<RegistrationContext> _dbContextOptionsBuilder;

        public RegistrationRepository(DbContextOptionsBuilder<RegistrationContext> dbContextOptionsBuilder)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
        }

        public async Task InsertAsync(ClientRegistrationResponse response)
        {
            await using var ctx = new RegistrationContext(_dbContextOptionsBuilder.Options);

            var entity = new RegistrationEntity(response.ClientId.BrokerId, response.ClientId.ClientId,
                response.ClientId.BrandId, response.RegisterTime);

            await ctx.Registrations.AddAsync(entity);
            await ctx.SaveChangesAsync();
        }

        public async Task<ClientRegistrationResponse> GetAsync(string clientId)
        {
            await using var ctx = new RegistrationContext(_dbContextOptionsBuilder.Options);

            var entity = await ctx.Registrations.FirstOrDefaultAsync(e => e.ClientId == clientId);

            if (entity == null)
                return null;

            return new ClientRegistrationResponse()
            {
                ClientId = new JetClientIdentity(entity.BrokerId, entity.BrandId, entity.ClientId),
                RegisterTime = entity.RegistrationTime,
                Result = ClientRegistrationResponse.RegistrationResult.Ok
            };
        }
    }
}