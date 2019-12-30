namespace Data.Models
{
    using Microsoft.EntityFrameworkCore;

    public class DataContext : DbContext
    {
        public DbSet<Data> Data { get; set; }
        public DbSet<DataString> DataString { get; set; }
        public DbSet<DataDecimal> DataDecimal { get; set; }
        public DbSet<LimitDecimal> LimitDecimal { get; set; }
        public DbSet<LimitDecimalDenormalized> LimitDecimalDenormalized { get; set; }
        public DbSet<LimitStringDenormalized> LimitStringDenormalized { get; set; }
        public DbSet<Group> Group { get; set; }

        public DataContext() 
            : base()
        {
            //Configuration.LazyLoadingEnabled = false;

            //var objectContext = (this as System.Data.Entity.Infrastructure.IObjectContextAdapter).ObjectContext;
            //objectContext.CommandTimeout = int.MaxValue;
        }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder
            //    .Conventions
            //    .Remove<PluralizingTableNameConvention>();

            modelBuilder
                .Entity<Group>()
                //.HasOptional(e => e.Parent)
                .HasOne(e => e.Parent)
                .WithMany(e => e.Childs);
            //.WillCascadeOnDelete(false);

            modelBuilder
                .Entity<DataDecimal>()
                .Property(x => x.DecimalValue)
                .HasColumnType("decimal(18,10)");
            //.HasPrecision(18, 10);

            modelBuilder
                .Entity<DataDecimal>()
                .HasOne(e => e.LimitDenormalized)
                .WithOne(e => e.Data)
                .HasForeignKey<LimitDecimalDenormalized>(e => new { e.GroupId, e.CollectionDate });

            modelBuilder
                .Entity<DataString>()
                .HasOne(e => e.LimitDenormalized)
                .WithOne(e => e.Data)
                .HasForeignKey<LimitStringDenormalized>(e => new { e.GroupId, e.CollectionDate });

            modelBuilder
                .Entity<Data>()
                .HasKey(e => new { e.GroupId, e.CollectionDate });

            modelBuilder
                .Entity<LimitDecimal>()
                .HasKey(e => new { e.GroupId, e.Priority, e.Max});

            modelBuilder
                .Entity<LimitDecimalDenormalized>()
                .HasKey(e => new { e.GroupId, e.CollectionDate });

            modelBuilder
                .Entity<LimitStringDenormalized>()
                .HasKey(e => new { e.GroupId, e.CollectionDate });

            modelBuilder
                .Entity<LimitDecimal>()
                .Property(e => e.Max)
                .HasColumnType("decimal(18,2)");

            modelBuilder
                .Entity<LimitDecimalDenormalized>()
                .Property(e => e.Max)
                .HasColumnType("decimal(18,2)");

            modelBuilder
                .Entity<LimitDecimalDenormalized>()
                .Property(e => e.Min)
                .HasColumnType("decimal(18,2)");
        }
    }
}
