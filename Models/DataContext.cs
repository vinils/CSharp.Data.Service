namespace Data.Models
{
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;

    public class DataContext : DbContext
    {
        public DbSet<Exam> Exam { get; set; }
        public DbSet<ExamString> ExamString { get; set; }
        public DbSet<ExamDecimal> ExamDecimal { get; set; }
        public DbSet<LimitDecimal> LimitDecimal { get; set; }
        public DbSet<LimitDecimalDenormalized> LimitDecimalDenormalized { get; set; }
        public DbSet<LimitStringDenormalized> LimitStringDenormalized { get; set; }
        public DbSet<Group> Group { get; set; }

        public DataContext() 
            : base("name=DataContext")
        {
            //Configuration.LazyLoadingEnabled = false;
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
        }
    }
}
