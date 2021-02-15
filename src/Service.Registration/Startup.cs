using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Autofac;
using MyJetWallet.Domain.ServiceBus;
using MyJetWallet.Sdk.GrpcMetrics;
using MyJetWallet.Sdk.GrpcSchema;
using MyJetWallet.Sdk.Postgres;
using MyJetWallet.Sdk.Service;
using MyNoSqlServer.Abstractions;
using MyServiceBus.TcpClient;
using Prometheus;
using ProtoBuf.Grpc.Server;
using Service.ClientWallets.Client;
using Service.ClientWallets.Grpc;
using Service.Registration.Database;
using Service.Registration.Grpc;
using Service.Registration.Modules;
using Service.Registration.NoSql;
using Service.Registration.Services;
using SimpleTrading.BaseMetrics;
using SimpleTrading.ServiceStatusReporterConnector;

namespace Service.Registration
{
    public class Startup
    {
        private readonly MyServiceBusTcpClient _serviceBusClient;

        public Startup()
        {
            _serviceBusClient = new MyServiceBusTcpClient(Program.ReloadedSettings(model => model.SpotServiceBusHostPort), ApplicationEnvironment.HostName);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCodeFirstGrpc(options =>
            {
                options.Interceptors.Add<PrometheusMetricsInterceptor>();
                options.BindMetricsInterceptors();
            });

            services.AddHostedService<ApplicationLifetimeManager>();

            services.AddDatabase(RegistrationContext.Schema, Program.Settings.PostgresConnectionString, o => new RegistrationContext(o));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseMetricServer();

            app.BindServicesTree(Assembly.GetExecutingAssembly());

            app.BindIsAlive();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcSchema<ClientRegistrationService, IClientRegistrationService>();

                endpoints.MapGrpcSchemaRegistry();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });

            _serviceBusClient.Start();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<SettingsModule>();
            builder.RegisterModule<ServiceModule>();

            builder.Register(ctx => new MyNoSqlServer.DataWriter.MyNoSqlServerDataWriter<ClientRegistrationResponseNoSql>(
                    Program.ReloadedSettings(model => model.MyNoSqlWriterUrl), ClientRegistrationResponseNoSql.TableName, true))
                .As<IMyNoSqlServerDataWriter<ClientRegistrationResponseNoSql>>()
                .SingleInstance();

            builder.RegisterClientRegistrationPublisher(_serviceBusClient);

            builder.RegisterClientWalletsClientsWithoutCache(Program.Settings.ClientWalletsGrpcServiceUrl);
        }
    }
}
