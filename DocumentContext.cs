using Microsoft.EntityFrameworkCore;
using System;

namespace ttn
{
    /// <summary>
    /// actual database connection
    /// </summary>
    class DocumentContext : DbContext
    {
        private string Name; //database path

        //Document table
        public DbSet<Document> Documents { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)

            => options.UseSqlite(String.Format("Data Source={0}", this.Name));


        public DocumentContext(string name)
        {
            Name = name;
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //set composite key Series + Number
            modelBuilder.Entity<Document>()
                .HasKey(p => new { p.Series, p.Number });

            modelBuilder.Entity<Document>()
                .Property(p => p.Series)
                .IsRequired();
            modelBuilder.Entity<Document>()
                .Property(p => p.Number)
                .IsRequired();
            modelBuilder.Entity<Document>()
                .Property(p => p.InDate)
                .IsRequired();
            modelBuilder.Entity<Document>()
                .Property(p => p.Spoiled)
                .IsRequired();
        }
    }
}
