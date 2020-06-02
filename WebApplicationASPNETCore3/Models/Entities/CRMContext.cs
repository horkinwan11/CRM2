using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

//using MySql.Data.EntityFrameworkCore;
//using MySql.Data.EntityFrameworkCore.Extensions;
using MySql.Data.MySqlClient;

namespace CRM.Models.Entities
{
	public partial class CRMContext : DbContext
	{
	
		public virtual DbSet<Permission> Permission { get; set; }
		public virtual DbSet<User> User { get; set; }
		public virtual DbSet<UserCredential> UserCredential { get; set; }
		public virtual DbSet<UserPermission> UserPermission { get; set; }
		public virtual DbSet<Campaign> Campaign { get; set; }
		public virtual DbSet<CampaignTeam> CampaignTeam { get; set; }
        public virtual DbSet<CampaignTeamMember> CampaignTeamMember { get; set; }
		public virtual DbSet<Package> Package { get; set; }
		public virtual DbSet<Status> Status { get; set; }

		public virtual DbSet<Role> Role { get; set; }

		public virtual DbSet<Customer> Customer { get; set; }
		public CRMContext(DbContextOptions<CRMContext> options)
		: base(options)
		{ } 

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			
			modelBuilder.Entity<Permission>(entity =>
			{
				entity.Property(e => e.Code)
					.IsRequired()
					.HasMaxLength(50);

				entity.Property(e => e.Title)
					.IsRequired()
					.HasMaxLength(100);
			});

			modelBuilder.Entity<User>(entity =>
			{
				entity.Property(e => e.CreatedDate).HasColumnType("datetime");

				entity.Property(e => e.Email)
					.IsRequired()
					.HasMaxLength(100);

				entity.Property(e => e.FirstName)
					.IsRequired()
					.HasMaxLength(50);

				entity.Property(e => e.LastName)
					.IsRequired()
					.HasMaxLength(50);
			});

			modelBuilder.Entity<UserCredential>(entity =>
			{
				entity.Property(e => e.Id).ValueGeneratedNever();

				entity.Property(e => e.HashedPassword)
					.IsRequired()
					.HasMaxLength(100);

				entity.Property(e => e.PasswordSalt)
					.IsRequired()
					.HasMaxLength(50);

				entity.HasOne(d => d.IdNavigation)
					.WithOne(p => p.UserCredential)
					.HasForeignKey<UserCredential>(d => d.Id)
					.OnDelete(DeleteBehavior.Restrict)
					.HasConstraintName("FK_UserCredential_User");
			});

			modelBuilder.Entity<UserPermission>(entity =>
			{
				entity.HasKey(e => new { e.UserId, e.PermissionId })
					.HasName("PK_UserPermissions");

				entity.HasOne(d => d.Permission)
					.WithMany(p => p.UserPermission)
					.HasForeignKey(d => d.PermissionId)
					.OnDelete(DeleteBehavior.Restrict)
					.HasConstraintName("FK_UserPermissions_Permissions");

				entity.HasOne(d => d.User)
					.WithMany(p => p.UserPermission)
					.HasForeignKey(d => d.UserId)
					.OnDelete(DeleteBehavior.Restrict)
					.HasConstraintName("FK_UserPermissions_Users");
			});

			modelBuilder.Entity<CampaignTeam>(entity =>
			{
				entity.HasKey(e => new { e.CampaignId, e.TeamId })
					.HasName("CampaignTeam");

				entity.HasOne(d => d.User)
					.WithMany(p => p.CampaignTeam)
					.HasForeignKey(d => d.TeamId)
					.OnDelete(DeleteBehavior.Restrict)
					.HasConstraintName("FK_CampaignTeam_Teams");

				entity.HasOne(d => d.Campaign)
					.WithMany(p => p.CampaignTeam)
					.HasForeignKey(d => d.CampaignId)
					.OnDelete(DeleteBehavior.Restrict)
					.HasConstraintName("FK_CampaignTeam_Campaigns");
			});

			modelBuilder.Entity<CampaignTeamMember>(entity =>
			{
				entity.HasKey(e => new { e.CampaignId, e.TeamId,  e.MemberId })
					.HasName("PK_UserPermissions");

				entity.HasOne(d => d.User)
					.WithMany(p => p.CampaignTeamMember)
					.HasForeignKey(d => d.MemberId)
					.OnDelete(DeleteBehavior.Restrict)
					.HasConstraintName("FK_CampaignTeamMember_Members");

				entity.HasOne(d => d.User)
					.WithMany(p => p.CampaignTeamMember)
					.HasForeignKey(d => d.TeamId)
					.OnDelete(DeleteBehavior.Restrict)
					.HasConstraintName("FK_UserPermissions_Teams");
			});
		}
	}
}