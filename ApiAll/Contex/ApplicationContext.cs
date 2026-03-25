using ApiAll.Model;
using ApiAll.Model.archive;
using ApiAll.Model.settingsmodel;
using ApiAll.Model.water;
using Microsoft.EntityFrameworkCore;

namespace ApiAll.Contex
{
    public class ApplicationContext : DbContext
    {
        public DbSet<WaterMoneyFakeInfo> WaterMoneyFakeInfo { get; set; }
        public DbSet<WaterStatistikaFakeReport> WaterStatistikaFakeReport { get; set; }
        public DbSet<WaterStatistikaFakeReport2> WaterStatistikaFakeReport2 { get; set; }
        public DbSet<WaterCheckFakeModel> WaterCheckFakeModel { get; set; }
        public DbSet<WaterClientAddress> WaterClientAddress { get; set; }
        public DbSet<WaterClientPhoneNumber> WaterClientPhoneNumber { get; set; }
        public DbSet<WaterMessageLog> WaterMessageLog { get; set; }
        public DbSet<WaterOrderedProduct> WaterOrderedProduct { get; set; }

        public DbSet<CustomContragentReport> customContragentReports { get; set; }

        public DbSet<Position> positions { get; set; }
        public DbSet<Province> provinces { get; set; }
        public DbSet<Rooms> rooms { get; set; }
        public DbSet<Authorization> authorizations { get; set; }
        public DbSet<Company> companies { get; set; }
        public DbSet<Contragent> contragents { get; set; }
        public DbSet<Department> departments { get; set; }
        public DbSet<Districts> districts { get; set; }
        public DbSet<ArchiveWorkingTime> ArchiveWorkingTime { get; set; }

        public DbSet<Menu> menus { get; set; }
        public DbSet<MenuItem> menuItems { get; set; }
        public DbSet<AccessMenu> accessMenus { get; set; }
        public DbSet<AccessMenuItem> accessMenuItems { get; set; }
        public DbSet<WaterSelledProductsTemp> WaterSelledProductsTemp { get; set; }

        public DbSet<Users> Users { get; set; }

        public DbSet<ArchiveAccessMenu> ArchiveAccessMenu { get; set; }
        public DbSet<ArchiveAccessMenuItem> ArchiveAccessMenuItem { get; set; }
        public DbSet<ArchiveDepartment> ArchiveDepartment { get; set; }
        public DbSet<ArchiveDatchik> ArchiveDatchik { get; set; }
        public DbSet<ArchiveUser> ArchiveUser { get; set; }
        public DbSet<ArchiveRoom> ArchiveRoom { get; set; }
        public DbSet<ArchiveRoomDatchikDetail> ArchiveRoomDatchikDetail { get; set; }
        public DbSet<ArchiveCompany> ArchiveCompany { get; set; }
        public DbSet<ArchiveRoomDetail> ArchiveRoomDetail { get; set; }
        public DbSet<ArchiveNotificationReciver> ArchiveNotificationReciver { get; set; }
        public DbSet<ArchiveDevice> ArchiveDevice { get; set; }
        public DbSet<ArchiveUsersCheckInOutReport> ArchiveUsersCheckInOutReport { get; set; }
        public DbSet<ArchiveQuti> ArchiveQuti { get; set; }
        public DbSet<ArchiveJavon> ArchiveJavon { get; set; }
        public DbSet<ArchiveTokcha> ArchiveTokcha { get; set; }
        public DbSet<ArchiveStelaj> ArchiveStelaj { get; set; }
        public DbSet<ArchiveXujjat> ArchiveXujjat { get; set; }
        public DbSet<ArchiveXujjatTuri> ArchiveXujjatTuri { get; set; }

        public DbSet<WaterAuth> WaterAuth { get; set; }
        public DbSet<WaterUser> WaterUser { get; set; }
        public DbSet<WaterAdminAuth> WaterAdminAuth { get; set; }
        public DbSet<WaterAdminUser> WaterAdminUser { get; set; }
        public DbSet<WaterCheck> WaterCheck { get; set; }
        public DbSet<WaterClient> WaterClient { get; set; }
        public DbSet<WaterClientBottleInfo> WaterClientBottleInfo { get; set; }
        public DbSet<WaterClientBottleInfoDetail> WaterClientBottleInfoDetail { get; set; }
        public DbSet<WaterClientDolg> WaterClientDolg { get; set; }
        public DbSet<WaterClientType> WaterClientType { get; set; }
        public DbSet<WaterContragent> WaterContragent { get; set; }
        public DbSet<WaterCompany> WaterCompany { get; set; }
        public DbSet<WaterContragentType> WaterContragentType { get; set; }
        public DbSet<WaterInvoice> WaterInvoice { get; set; }
        public DbSet<WaterInvoiceItem> WaterInvoiceItem { get; set; }
        public DbSet<WaterOrder> WaterOrder { get; set; }
        public DbSet<WaterOrderItem> WaterOrderItem { get; set; }
        public DbSet<WaterProduct> WaterProduct { get; set; }
        public DbSet<WaterProductOstatka> WaterProductOstatka { get; set; }
        public DbSet<WaterTuman> WaterTuman { get; set; }
        public DbSet<WaterViloyat> WaterViloyat { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WaterMoneyFakeInfo>().HasNoKey();
            modelBuilder.Entity<WaterStatistikaFakeReport2>().HasNoKey();
            modelBuilder.Entity<CustomContragentReport>().HasNoKey();
            modelBuilder.Entity<WaterSelledProductsTemp>().HasNoKey();
            modelBuilder.Entity<WaterOrderedProduct>().HasNoKey();
            modelBuilder.Entity<ArchiveWorkingTime>().HasNoKey();
            modelBuilder.Entity<WaterCheckFakeModel>().HasNoKey();
            modelBuilder.Entity<WaterStatistikaFakeReport>().HasNoKey();

            base.OnModelCreating(modelBuilder);
        }
    }
}
