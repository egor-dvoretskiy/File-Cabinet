using Microsoft.EntityFrameworkCore;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace FileCabinetApp.ServiceTools
{
    /// <summary>
    /// Context for Entity framework.
    /// </summary>
    internal class EntityService : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityService"/> class.
        /// </summary>
        internal EntityService()
        {
            _ = this.Database.EnsureCreated();
        }

        /// <summary>
        /// Gets or sets stored records.
        /// </summary>
        /// <value>
        /// Stored records.
        /// </value>
        internal DbSet<FileCabinetRecord> FileCabinetRecords { get; set; }

        /// <summary>
        /// Configure connection to database.
        /// </summary>
        /// <param name="optionsBuilder">Settings of connection.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ADOService.ConnectionString);

            // optionsBuilder.LogTo(System.Console.WriteLine);
        }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<FileCabinetRecord>()
                .Property(p => p.Salary)
                .HasColumnType("decimal(18,3)");
        }
    }
}
