using MatchFinder.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MatchFinder.Infrastructure.DataAccess
{
    public class MatchFinderContext : DbContext
    {
        public MatchFinderContext()
        {
        }

        public MatchFinderContext(DbContextOptions<MatchFinderContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var ConnectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(ConnectionString);
            }
        }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<FieldRevenue> FieldRevenues { get; set; }
        public DbSet<PartialField> PartialFields { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<Fund> Funds { get; set; }
        public DbSet<OpponentFinding> OpponentFindings { get; set; }
        public DbSet<OpponentFindingRequest> OpponentFindingRequests { get; set; }
        public DbSet<PreOrder> PreOrders { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Verification> Verifications { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TournamentRegistration> TournamentRegistrations { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<MatchingTeam> MatchingTeams { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationUser> NotificationUsers { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<InactiveTime> InactiveTimes { get; set; }
        public DbSet<Slot> Slots { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Rate> Rates { get; set; }
        public DbSet<FavoriteField> FavoriteFields { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<InvertedIndex> InvertedIndexes { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Staff>()
                .HasKey(s => new { s.FieldId, s.UserId });

            modelBuilder.Entity<TournamentRegistration>()
                .HasKey(tr => new { tr.TournamentId, tr.TeamId });

            modelBuilder.Entity<TeamMember>()
                .HasKey(tm => new { tm.TeamId, tm.UserId });

            modelBuilder.Entity<NotificationUser>()
                .HasKey(au => new { au.NotificationId, au.UserId });

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.PartialField)
                .WithMany(pf => pf.Bookings)
                .HasForeignKey(b => b.PartialFieldId);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Team)
                .WithMany(t => t.Bookings)
                .HasForeignKey(b => b.TeamId);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FieldRevenue>()
                .HasOne(fr => fr.PartialField)
                .WithMany(pf => pf.FieldRevenues)
                .HasForeignKey(fr => fr.PartialFieldId);

            modelBuilder.Entity<PartialField>()
                .HasOne(pf => pf.Field)
                .WithMany(f => f.PartialFields)
                .HasForeignKey(pf => pf.FieldId);

            modelBuilder.Entity<Field>()
                .HasOne(f => f.Owner)
                .WithMany(u => u.Fields)
                .HasForeignKey(f => f.OwnerId)
                .OnDelete(DeleteBehavior.Restrict); // Avoid cascade paths

            modelBuilder.Entity<Fund>()
                .HasOne(f => f.Team)
                .WithMany(t => t.Funds)
                .HasForeignKey(f => f.TeamId);

            modelBuilder.Entity<OpponentFinding>()
                .HasOne(p => p.Field)
                .WithMany(f => f.OpponentFindings)
                .HasForeignKey(p => p.FieldId)
                .OnDelete(DeleteBehavior.Restrict); // Avoid cascade paths

            modelBuilder.Entity<OpponentFinding>()
                .HasOne(p => p.UserFinding)
                .WithMany(u => u.OpponentFindings)
                .HasForeignKey(p => p.UserFindingId)
                .OnDelete(DeleteBehavior.Restrict); // Avoid cascade paths

            modelBuilder.Entity<OpponentFinding>()
                .HasOne(p => p.Booking)
                .WithMany(u => u.OpponentFindings)
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Restrict); // Avoid cascade paths

            modelBuilder.Entity<OpponentFindingRequest>()
                .HasOne(p => p.OpponentFinding)
                .WithMany(u => u.OpponentFindingRequests)
                .HasForeignKey(p => p.OpponentFindingId)
                .OnDelete(DeleteBehavior.Restrict); // Avoid cascade paths

            modelBuilder.Entity<OpponentFindingRequest>()
                .HasOne(p => p.UserRequesting)
                .WithMany(u => u.OpponentFindingRequests)
                .HasForeignKey(p => p.UserRequestingId)
                .OnDelete(DeleteBehavior.Restrict); // Avoid cascade paths

            modelBuilder.Entity<PreOrder>()
                .HasOne(po => po.Booking)
                .WithMany(b => b.PreOrders)
                .HasForeignKey(po => po.BookingId)
                .OnDelete(DeleteBehavior.Restrict); // Avoid cascade paths

            modelBuilder.Entity<PreOrder>()
                .HasOne(po => po.Item)
                .WithMany(m => m.PreOrders)
                .HasForeignKey(po => po.ItemId)
                .OnDelete(DeleteBehavior.Restrict); // Avoid cascade paths

            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId);

            modelBuilder.Entity<Verification>()
                .HasOne(v => v.User)
                .WithMany(u => u.Verifications)
                .HasForeignKey(v => v.UserId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            modelBuilder.Entity<MatchingTeam>()
                .HasOne(mt => mt.Team1)
                .WithMany(t => t.MatchingTeams1)
                .HasForeignKey(mt => mt.Team1Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MatchingTeam>()
                .HasOne(mt => mt.Team2)
                .WithMany(t => t.MatchingTeams2)
                .HasForeignKey(mt => mt.Team2Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MatchingTeam>()
                .HasOne(mt => mt.PartialField)
                .WithMany(pf => pf.MatchingTeams)
                .HasForeignKey(mt => mt.PartialFieldId);

            modelBuilder.Entity<MatchingTeam>()
                .HasOne(mt => mt.Tournament)
                .WithMany(t => t.MatchingTeams)
                .HasForeignKey(mt => mt.TournamentId);

            modelBuilder.Entity<NotificationUser>()
                .HasOne(au => au.Notification)
                .WithMany(a => a.NotificationUsers)
                .HasForeignKey(au => au.NotificationId);

            modelBuilder.Entity<NotificationUser>()
                .HasOne(au => au.User)
                .WithMany(u => u.NotificationUsers)
                .HasForeignKey(au => au.UserId);

            modelBuilder.Entity<Menu>()
                .HasOne(m => m.Field)
                .WithMany(f => f.Menus)
                .HasForeignKey(m => m.FieldId);

            modelBuilder.Entity<InactiveTime>()
                .HasOne(it => it.Field)
                .WithMany(f => f.InactiveTimes)
                .HasForeignKey(it => it.FieldId);

            modelBuilder.Entity<Slot>()
                .HasOne(s => s.Field)
                .WithMany(f => f.Slots)
                .HasForeignKey(s => s.FieldId);

            modelBuilder.Entity<TeamMember>()
                .HasOne(tm => tm.Team)
                .WithMany(t => t.TeamMembers)
                .HasForeignKey(tm => tm.TeamId)
                .OnDelete(DeleteBehavior.Restrict); // Avoid cascade paths

            modelBuilder.Entity<TeamMember>()
                .HasOne(tm => tm.User)
                .WithMany(u => u.TeamMembers)
                .HasForeignKey(tm => tm.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Avoid cascade paths

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Reciver)
                .WithMany()
                .HasForeignKey(t => t.ReciverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Booking)
                .WithMany(b => b.Transactions)
                .HasForeignKey(t => t.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rate>(entity =>
            {
                entity.HasKey(r => new { r.UserId, r.BookingId });

                entity.HasOne(r => r.Rater)
                    .WithMany(u => u.Rates)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Booking)
                    .WithMany(f => f.Rates)
                    .HasForeignKey(r => r.BookingId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Field)
                    .WithMany(f => f.Rates)
                    .HasForeignKey(r => r.FieldId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(r => r.Star)
                    .HasAnnotation("Range", new[] { 1, 5 });
            });

            modelBuilder.Entity<FavoriteField>(entity =>
            {
                entity.HasKey(r => new { r.UserId, r.FieldId });

                entity.HasOne(r => r.User)
                    .WithMany(u => u.FavoriteFields)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Field)
                    .WithMany(f => f.FavoriteFields)
                    .HasForeignKey(r => r.FieldId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasOne(r => r.Reporter)
                    .WithMany(u => u.Reports)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Field)
                    .WithMany(f => f.Reports)
                    .HasForeignKey(r => r.FieldId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<BlogPost>(entity =>
            {
                entity.HasOne(r => r.Author)
                    .WithMany(u => u.BlogPosts)
                    .HasForeignKey(r => r.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Field)
                    .WithMany(f => f.BlogPosts)
                    .HasForeignKey(r => r.FieldId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.HasOne(r => r.Field)
                    .WithMany(u => u.Images)
                    .HasForeignKey(r => r.FieldId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}