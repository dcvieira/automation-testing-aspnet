using CarvedRock.Data;
using CarvedRock.InnerLoop.Tests.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;

namespace CarvedRock.InnerLoop.Tests.Utils
{
    public class CustomApiFactory(SharedFixture fixture) : WebApplicationFactory<Program>
    {
        public SharedFixture SharedFixture => fixture;
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("innerloop-test");

            builder.ConfigureTestServices(services => services
              .AddAuthentication(TestAuthHandler.SchemeName)
              .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { }));

            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                      d => d.ServiceType == typeof(DbContextOptions<LocalContext>));
                      services.Remove(dbContextDescriptor!);

                var ctx = services.SingleOrDefault(d => d.ServiceType == typeof(LocalContext));
                services.Remove(ctx!);

                // SQLite --------------------------------
                services.AddDbContext<LocalContext>(opts =>
                    opts.UseSqlite($"Data Source={SharedFixture.DatabaseName}")
                        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
            });
        }
    }
}
