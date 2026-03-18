using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Ktgk.EntityFrameworkCore;

public static class KtgkDbContextConfigurer
{
    public static void Configure(DbContextOptionsBuilder<KtgkDbContext> builder, string connectionString)
    {
        builder.UseSqlServer(connectionString);
    }

    public static void Configure(DbContextOptionsBuilder<KtgkDbContext> builder, DbConnection connection)
    {
        builder.UseSqlServer(connection);
    }
}
