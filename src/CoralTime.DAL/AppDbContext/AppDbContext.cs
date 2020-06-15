using CoralTime.DAL.Models;
using CoralTime.DAL.Models.LogChanges;
using CoralTime.DAL.Models.Member;
using CoralTime.DAL.Models.ReportsSettings;
using CoralTime.DAL.Models.Vsts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CoralTime.DAL
{
    public partial class AppDbContext : IdentityDbContext<ApplicationUser>, IPersistedGrantDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Member> Members { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<TimeEntry> TimeEntries { get; set; }

        public DbSet<Models.Client> Clients { get; set; }

        public DbSet<ProjectRole> ProjectRoles { get; set; }

        public DbSet<MemberProjectRole> MemberProjectRoles { get; set; }

        public DbSet<TaskType> TaskTypes { get; set; }

        public DbSet<Setting> Settings { get; set; }

        public DbSet<UserForgotPassRequest> UserForgotPassRequests { get; set; }

        public DbSet<MemberImage> MemberImages { get; set; }

        public DbSet<PersistedGrant> PersistedGrants { get; set; }

        public DbSet<ReportsSettings> ReportsSettings { get; set; }

        public DbSet<MemberAction> MemberActions { get; set; }

        public DbSet<VstsUser> VstsUsers { get; set; }

        public DbSet<VstsProject> VstsProjects { get; set; }

        public DbSet<VstsProjectUser> VstsProjectUsers { get; set; }

        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TimeEntry>()
                .HasOne(p => p.Project)
                .WithMany(p => p.TimeEntries).HasForeignKey(k => k.ProjectId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TimeEntry>()
                .HasOne(p => p.Member)
                .WithMany(w => w.TimeEntries).HasForeignKey(k => k.MemberId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TimeEntry>()
                .HasOne(p => p.TaskType);

            builder.Entity<TimeEntry>()
                .HasIndex(t => new { t.MemberId, t.Date });

            builder.Entity<Member>()
                .HasOne(p => p.User);

            builder.Entity<Member>()
                .HasMany(p => p.TimeEntries)
                .WithOne(p => p.Member);

            builder.Entity<Models.Client>()
                .HasMany(p => p.Projects)
                .WithOne(p => p.Client).IsRequired(false);

            builder.Entity<Models.Client>()
                .HasIndex(p => p.Name).IsUnique();

            builder.Entity<TaskType>()
                .HasOne(p => p.Project);

            builder.Entity<MemberImage>()
                .HasOne(p => p.Member);

            builder.Entity<Member>()
                .HasOne(p => p.MemberImage);

            builder.Entity<Project>()
                .HasMany(p => p.TaskTypes)
                .WithOne(p => p.Project).IsRequired(false);

            builder.Entity<Project>()
                .HasOne(p => p.Client);

            builder.Entity<Project>()
                .HasIndex(p => p.Name).IsUnique();

            builder.Entity<Project>()
                .HasMany(p => p.TimeEntries)
                .WithOne(p => p.Project);

            builder.Entity<MemberProjectRole>()
                .HasIndex(t => new
                {
                    MemberProjectRoleId = t.MemberId,
                    t.ProjectId
                }).IsUnique();

            builder.Entity<MemberProjectRole>()
                .HasOne(wp => wp.Member)
                .WithMany(w => w.MemberProjectRoles).HasForeignKey(k => k.MemberId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MemberProjectRole>()
                .HasOne(wp => wp.Project)
                .WithMany(p => p.MemberProjectRoles).HasForeignKey(k => k.ProjectId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PersistedGrant>()
                .HasKey(p => p.Key);

            builder.Entity<ReportsSettings>()
                .HasIndex(t => new { ReportsSettingsId = t.MemberId, t.QueryName }).IsUnique();

            builder.Entity<MemberAction>()
                .HasOne(p => p.Member);

            builder.Entity<VstsProject>()
                .HasOne(p => p.Project);

            builder.Entity<VstsUser>()
                .HasOne(u => u.Member);

            builder.Entity<VstsProjectUser>()
                .HasIndex(t => new
                {
                    t.VstsUserId,
                    t.VstsProjectId
                }).IsUnique();

            builder.Entity<VstsProjectUser>()
                .HasOne(pu=> pu.VstsProject)
                .WithMany(p=> p.VstsProjectUsers).HasForeignKey(k=> k.VstsProjectId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<VstsProjectUser>()
                .HasOne(pu => pu.VstsUser)
                .WithMany(p => p.VstsProjectUsers).HasForeignKey(k => k.VstsUserId).OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(builder);
        }
    }
}