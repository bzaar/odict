using System.Data.Entity;

namespace odict.ru
{
    public class DbContext : System.Data.Entity.DbContext
    {
        public DbContext ()
        {
            Database.SetInitializer (new CreateDatabaseIfNotExists <DbContext> ());
        }

        public DbSet <Reply> Replies { get; set; }
    }
}
