using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShireBank.Server.Interceptors;
using ShireBank.Server.Services;
using ShireBank.Shared.Data;
using ShireBank.Shared.Data.Repositories;

namespace ShireBank.Server;

internal class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; set; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<BankDbContext>(o => { o.UseSqlite(Configuration.GetConnectionString("BankDatabase")); });

        services.AddTransient<IBankAccountRepository, BankAccountRepository>();
        services.AddTransient<IBankTransactionRepository, BankTransactionRepository>();
        services.AddSingleton<InspectionStateService>();

        services.AddGrpc(o =>
        {
            o.EnableDetailedErrors = true;
            o.Interceptors.Add<InspectionInterceptor>();
        });
    }

    public void Configure(IApplicationBuilder app, BankDbContext bankDbContext)
    {
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<InspectorService>();
            endpoints.MapGrpcService<CustomerService>();
        });

        bankDbContext.Database.EnsureDeleted();
        bankDbContext.Database.Migrate();
    }
}