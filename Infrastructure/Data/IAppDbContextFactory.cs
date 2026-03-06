namespace HGT.EAM.Client.Infrastructure.Data;

public interface IAppDbContextFactory
{
    AppDbContext? CreateDbContext();
}
