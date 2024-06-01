#nullable disable
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using IMRepo.Models.Domain;

namespace IMRepo.Data
{
    public partial class IMRepoDbContext : IdentityDbContext
    {
        public IMRepoDbContext(DbContextOptions<IMRepoDbContext>  options) : base(options)
        {
        }


        public DbSet<Addition> Addition { get; set; }
        public DbSet<AdditionAttachment> AdditionAttachment { get; set; }
        public DbSet<ExtensionAttachment> ExtensionAttachment { get; set; }
        public DbSet<PaymentAttachment> PaymentAttachment { get; set; }
        public DbSet<ProjectAttachment> ProjectAttachment { get; set; }
        public DbSet<Agency> Agency { get; set; }
        public DbSet<FundingAgency> FundingAgency { get; set; }
        public DbSet<PaymentStage> PaymentStage { get; set; }
        public DbSet<TaskStage> TaskStage { get; set; }
        public DbSet<ProjectStage> ProjectStage { get; set; }
        public DbSet<Extension> Extension { get; set; }
        public DbSet<ProjectFunding> ProjectFunding { get; set; }
        public DbSet<ProjectImage> ProjectImage { get; set; }
        public DbSet<Sdg> Sdg { get; set; }
        public DbSet<Office> Office { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<UserProfile> UserProfile { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<Sector> Sector { get; set; }
        public DbSet<Subsector> Subsector { get; set; }
        public DbSet<FundingType> FundingType { get; set; }
        public DbSet<ProjectVideo> ProjectVideo { get; set; }
    }
}
