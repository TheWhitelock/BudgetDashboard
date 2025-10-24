#nullable enable
using BudgetDashboard.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BudgetDashboard.Data;

/// <summary>
/// Represents the Entity Framework Core <see cref="DbContext"/> for the budgeting domain.
/// Provides access to the <see cref="Budget"/> and <see cref="BudgetItem"/> entity sets
/// and configures entity mappings, constraints, and relationships used by the application's data model.
/// </summary>
public class BudgetContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BudgetContext"/> class with the specified options.
    /// </summary>
    /// <param name="options">The options for this context, typically provided by dependency injection and configured with the database provider and connection string.</param>
    public BudgetContext(DbContextOptions<BudgetContext> options) : base(options) {}

    /// <summary>
    /// Gets the <see cref="DbSet{Budget}"/> used to query and save instances of <see cref="Budget"/>.
    /// Use this property to perform LINQ queries, add, update, and remove budget entities.
    /// </summary>
    public DbSet<Budget> Budgets => Set<Budget>();

    /// <summary>
    /// Gets the <see cref="DbSet{BudgetItem}"/> used to query and save instances of <see cref="BudgetItem"/>.
    /// Use this property to perform LINQ queries, add, update, and remove budget item entities.
    /// </summary>
    public DbSet<BudgetItem> BudgetItems => Set<BudgetItem>();

    /// <summary>
    /// Configures the model for the context using the provided <see cref="ModelBuilder"/>.
    /// </summary>
    /// <param name="modelBuilder">
    /// The builder being used to construct the model for this context. This method configures:
    /// - The <see cref="Budget"/> entity: requires <c>Name</c> with a max length of 200, creates an index on <c>Name</c>,
    ///   and configures a one-to-many relationship to <see cref="BudgetItem"/> with cascade delete.
    /// - The <see cref="BudgetItem"/> entity: requires <c>Name</c> with a max length of 200 and maps
    ///   <c>EstimatedAmount</c> to the SQL type <c>decimal(18,2)</c>.
    /// </param>
    /// <remarks>
    /// Override this method to customize the EF Core model. The configuration here centralizes
    /// domain constraints and relationship behavior so they are consistently applied when the database schema is created or validated.
    /// </remarks>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Budget>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.HasIndex(x => x.Name);
            e.HasMany(x => x.Items)
             .WithOne(i => i.Budget!)
             .HasForeignKey(i => i.BudgetId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BudgetItem>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.EstimatedAmount).HasColumnType("decimal(18,2)");
        });
    }
}
