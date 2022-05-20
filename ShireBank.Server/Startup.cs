using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ShireBank.Server.Interceptors;
using ShireBank.Server.Services;
using ShireBank.Shared.Data;
using ShireBank.Shared.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShireBank.Server
{
    internal class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<BankDbContext>(o =>
            {
                o.UseSqlite(Configuration.GetConnectionString("BankDatabase"));
            });

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
}
