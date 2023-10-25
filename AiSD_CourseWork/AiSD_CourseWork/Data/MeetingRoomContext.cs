using System;
using System.Collections.Generic;
using AiSD_CourseWork.Models;
using Microsoft.EntityFrameworkCore;

namespace AiSD_CourseWork.Data;

public partial class MeetingRoomContext : DbContext
{
    public MeetingRoomContext()
    {
    }

    public MeetingRoomContext(DbContextOptions<MeetingRoomContext> options)
        : base(options)
    {
    }

    public virtual DbSet<MeetingRoom> MeetingRooms { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-IR0N4NT4\\SQLEXPRESS; initial catalog='MeetingRoom'; trusted_connection=yes; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MeetingRoom>(entity =>
        {
            entity.HasOne(d => d.IdNavigation).WithMany(p => p.MeetingRooms)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MeetingRoom_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
