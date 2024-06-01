#nullable disable
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using IMRepo.Areas.Review.Models;
using IMRepo.Models.Domain;
using IMRepo.Models.Reports;

namespace IMRepo.Data
{
    public partial class IMRepoDbContext : IdentityDbContext
    {

        public DbSet<DashboardInfo> DbDashboardInfo { get; set; }
        public DbSet<ItemQty> SectorsQty { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder
                .Entity<DashboardInfo>(
                    eb =>
                    {
                        eb.HasNoKey();
                        eb.ToView("dashboard_info");
                        //eb.Property(v => v.CancelledContractsCount).HasColumnName("CancelledContractsCount");
                    });
            modelBuilder
                .Entity<ItemQty>(
                    eb =>
                    {
                        eb.HasNoKey();
                        eb.ToView("dashboard_ProjectsPerSector");
                    });
            base.OnModelCreating(modelBuilder);

            // avoid cascade
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.FundingSource_info)
                .WithMany()
                .HasForeignKey(p => p.FundingSource)
                .OnDelete(DeleteBehavior.NoAction); // Specify the desired behavior here

        }




    }
}
