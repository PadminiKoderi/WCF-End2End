using GeoLib.Core;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.OracleClient;
using System.Configuration;

namespace GeoLib.Data
{
    public class GeoLibDbContext : DbContext
    {
        public GeoLibDbContext()
            : base("name=main")
        {
            Database.SetInitializer<GeoLibDbContext>(null);
        }

        public DbSet<ZipCode> ZipCodeSet { get; set; }
        public DbSet<State> StateSet { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Ignore<IIdentifiableEntity>();

            modelBuilder.Entity<ZipCode>().HasKey<int>(e => e.ZipCodeId).Ignore(e => e.EntityId)
                .HasRequired(e => e.State).WithMany().HasForeignKey(e => e.StateId);

            modelBuilder.Entity<State>().HasKey<int>(e => e.StateId).Ignore(e => e.EntityId);
        }

        public static DataTable GetOracleData(string Sql)
        {
            DataTable dtData = new DataTable();
            OracleConnection cn = new OracleConnection();
            OracleDataAdapter OracleDataAdapter = default(OracleDataAdapter);            
             cn.ConnectionString = ConfigurationManager.ConnectionStrings["OracleConn"].ConnectionString;
                cn.Open();
                OracleDataAdapter = new OracleDataAdapter(Sql, cn);
                OracleDataAdapter.Fill(dtData);
                cn.Close();
                return dtData;

            
        }
        public static void UpdateOracleData(string sql)
        {
            
            OracleConnection cn = new OracleConnection();
            OracleCommand cm = new OracleCommand(sql, cn);
            
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["OracleConn"].ConnectionString;
            cn.Open();
            cm.ExecuteNonQuery();   
            cn.Close();
            
        }
    }
}
