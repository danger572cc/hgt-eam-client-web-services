using HGT.EAM.Client.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace HGT.EAM.Client.Infrastructure.Data;

public sealed class AppDbContext : DbContext
{
    public DbSet<Provision> Provisions => Set<Provision>();
    public DbSet<PurchaseOrderAudit> PurchaseOrderAudits => Set<PurchaseOrderAudit>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProvisionConfiguration());
        modelBuilder.ApplyConfiguration(new PurchaseOrderAuditConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
