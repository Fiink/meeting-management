using MeetingManagementSystem.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MeetingManagementSystem.Data.Db
{
    public class MeetingDbContext(DbContextOptions<MeetingDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<MeetingParticipant> MeetingParticipants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure reservation mapping
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.ReservationOwner)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.ReservationOwnerId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Reservation>()
                .HasOne(m => m.MeetingRoom)
                .WithMany(m => m.Reservations)
                .HasForeignKey(r => r.MeetingRoomId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure meeting participant mapping
            modelBuilder.Entity<MeetingParticipant>()
                .HasKey(mp => new { mp.ReservationId, mp.ParticipantId }); // Composite key, many-to-many mapping
            modelBuilder.Entity<MeetingParticipant>()
                .HasOne(mp => mp.Reservation)
                .WithMany(r => r.Participants)
                .HasForeignKey(mp => mp.ReservationId)
                // Due to cycles/multiple cascade paths, we cannot cascade remove this entry when a participant is removed.
                // We can clean up the MeetingParticipant table when we remove a Reservation.
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<MeetingParticipant>()
                .HasOne(mp => mp.Participant)
                .WithMany(u => u.ParticipatingInMeetings)
                .HasForeignKey(mp => mp.ParticipantId)
                .HasConstraintName("FK_MeetingParticipant_User"); // Constraint: User cannot attend the same meeting multiple times
        }
    }
}
