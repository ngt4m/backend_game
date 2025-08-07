using System.Data.Entity;

public class AppDbContext : DbContext
{
#if DEBUG
    public AppDbContext() : base("name=TVQDConnectionString_Local")
    {

        Database.SetInitializer(new CreateDatabaseIfNotExists<AppDbContext>());
    }
#else
    public AppDbContext() : base("name=TVQDConnectionString_Server")
    {

        Database.SetInitializer(new CreateDatabaseIfNotExists<AppDbContext>());
    }
#endif

    public DbSet<User> Users { get; set; }
    public DbSet<Contest> Contests { get; set; }
    public DbSet<UserResult> UserResults { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        // Liên kết 1-n: User -> UserResults
        modelBuilder.Entity<UserResult>()
            .HasRequired(ur => ur.User)
            .WithMany(u => u.UserResults)
            .HasForeignKey(ur => ur.UserId)
            .WillCascadeOnDelete(false);

        // Liên kết 1-n: Contest -> UserResults
        modelBuilder.Entity<UserResult>()
            .HasRequired(ur => ur.Contest)
            .WithMany(c => c.UserResults)
            .HasForeignKey(ur => ur.ContestId)
            .WillCascadeOnDelete(false);

        // Optional: Index unique cho SubmissionToken (nếu dùng)
        modelBuilder.Entity<UserResult>()
            .HasIndex(ur => ur.SubmissionToken)
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}
