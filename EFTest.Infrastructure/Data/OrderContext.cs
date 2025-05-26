using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MongoDB.EntityFrameworkCore.Extensions;

namespace EFTest.Infrastructure.Data;

public class OrderContext : DbContext
{
    public OrderContext(DbContextOptions<OrderContext> options) : base(options)
    {
        // Disable transactions for standalone MongoDB
        Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
    }

    public DbSet<OrderEntity> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Create value converters
        var guidToStringConverter = new ValueConverter<Guid, string>(
            guid => guid.ToString("N"),
            str => Guid.Parse(str));

        // Configure Order entity for MongoDB
        modelBuilder.Entity<OrderEntity>().ToCollection("orders");
        modelBuilder.Entity<OrderEntity>()
            .HasKey(o => o.Id);
        
        // Configure Id property
        modelBuilder.Entity<OrderEntity>()
            .Property(o => o.Id)
            .HasConversion(guidToStringConverter);

        // Configure embedded OrderLines
        modelBuilder.Entity<OrderEntity>()
            .OwnsMany(o => o.OrderLines, ol =>
            {
                // Configure OrderLine Id property
                ol.Property(line => line.Id)
                  .HasConversion(guidToStringConverter);
                
                // Ignore calculated properties
                ol.Ignore(line => line.LineTotal);
            });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Generate GUIDs for new entities if they are empty
        foreach (var entry in ChangeTracker.Entries<OrderEntity>())
        {
            if (entry.State == EntityState.Added && entry.Entity.Id == Guid.Empty)
            {
                entry.Entity.Id = Guid.NewGuid();
                
                // Generate GUIDs for embedded order lines
                foreach (var orderLine in entry.Entity.OrderLines.Where(ol => ol.Id == Guid.Empty))
                {
                    orderLine.Id = Guid.NewGuid();
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
