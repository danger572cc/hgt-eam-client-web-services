using Microsoft.EntityFrameworkCore;

namespace HGT.EAM.Client.Infrastructure.Data;

public sealed class AppDbContextFactory : IAppDbContextFactory
{
    private readonly string? _connectionString;

    public AppDbContextFactory(string? connectionString)
    {
        _connectionString = connectionString;
    }

    public AppDbContext? CreateDbContext()
    {
        if (string.IsNullOrWhiteSpace(_connectionString))
            return null;

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(_connectionString)
            .Options;

        return new AppDbContext(options);
    }
}
