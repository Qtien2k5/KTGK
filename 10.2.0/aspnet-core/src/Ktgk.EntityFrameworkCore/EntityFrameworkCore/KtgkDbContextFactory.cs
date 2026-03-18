using Ktgk.Configuration;
using Ktgk.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Ktgk.EntityFrameworkCore;

/* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
public class KtgkDbContextFactory : IDesignTimeDbContextFactory<KtgkDbContext>
{
    public KtgkDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<KtgkDbContext>();

        /*
         You can provide an environmentName parameter to the AppConfigurations.Get method. 
         In this case, AppConfigurations will try to read appsettings.{environmentName}.json.
         Use Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") method or from string[] args to get environment if necessary.
         https://docs.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli#args
         */
        var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

        KtgkDbContextConfigurer.Configure(builder, configuration.GetConnectionString(KtgkConsts.ConnectionStringName));

        return new KtgkDbContext(builder.Options);
    }
}
