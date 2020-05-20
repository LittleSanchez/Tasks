namespace Server
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class PhoneListModel : DbContext
    {
        public PhoneListModel()
            : base("name=PhoneListContext")
        {
        }

        public virtual DbSet<Phones> Phones { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
