using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.SmartObjects.Services.CEP.Data
{
    public class CEPListenerContext : DbContext
    {
        public CEPListenerContext()
            : base("CEPListenerContext")
        {
        }

        public DbSet<Model.EventListener> EventListener { get; set; }
        public DbSet<Model.EventListenerLog> EventListenerLog { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Model.EventListener>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Model.EventListenerLog>().Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            //modelBuilder.Entity<Data.CaseUser>().Ignore(p => p.FullName);
            //modelBuilder.Entity<Data.CaseInstance>().Ignore(p => p.ExceedsExpected);

            // team - user m:m
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //base.OnModelCreating(modelBuilder);
        }

    }
}