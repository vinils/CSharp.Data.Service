namespace Data.Models
{
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;

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
            : base("name=DataContext")
        {
            //Configuration.LazyLoadingEnabled = false;

            var objectContext = (this as System.Data.Entity.Infrastructure.IObjectContextAdapter).ObjectContext;
            objectContext.CommandTimeout = int.MaxValue;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder
                .Conventions
                .Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Group>()
                .HasOptional(e => e.Parent)
                .WithMany(e => e.Childs)
                .WillCascadeOnDelete(false);

            modelBuilder
                .Entity<DataDecimal>()
                .Property(x => x.DecimalValue)
                .HasPrecision(18, 10);
        }
    }
}
