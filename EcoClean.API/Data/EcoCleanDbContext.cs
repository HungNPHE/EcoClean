using EcoClean.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EcoClean.API.Data;

public class EcoCleanDbContext : DbContext
{
    public EcoCleanDbContext(DbContextOptions<EcoCleanDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<MealPlan> MealPlans => Set<MealPlan>();
    public DbSet<WeightLog> WeightLogs => Set<WeightLog>();
    public DbSet<FoodScan> FoodScans => Set<FoodScan>();
    public DbSet<ChatHistory> ChatHistories => Set<ChatHistory>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<AIMealPlan> AIMealPlans => Set<AIMealPlan>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // User
        mb.Entity<User>(e => {
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Role).HasMaxLength(20);
        });

        // UserProfile 1-1
        mb.Entity<UserProfile>(e => {
            e.HasOne(p => p.User).WithOne(u => u.Profile)
             .HasForeignKey<UserProfile>(p => p.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // Recipe
        mb.Entity<Recipe>(e => {
            e.Property(r => r.Category).HasMaxLength(50);
        });

        // MealPlan
        mb.Entity<MealPlan>(e => {
            e.HasIndex(m => new { m.UserId, m.PlanDate });
            e.HasOne(m => m.User).WithMany(u => u.MealPlans)
             .HasForeignKey(m => m.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(m => m.Recipe).WithMany()
             .HasForeignKey(m => m.RecipeId).OnDelete(DeleteBehavior.SetNull);
        });

        // WeightLog
        mb.Entity<WeightLog>(e => {
            e.HasIndex(w => new { w.UserId, w.LogDate });
            e.HasOne(w => w.User).WithMany(u => u.WeightLogs)
             .HasForeignKey(w => w.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // FoodScan
        mb.Entity<FoodScan>(e => {
            e.HasOne(f => f.User).WithMany(u => u.FoodScans)
             .HasForeignKey(f => f.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // ChatHistory — bảng trong DB tên là "ChatHistory" (số ít)
        mb.Entity<ChatHistory>(e => {
            e.ToTable("ChatHistory");
            e.HasIndex(c => new { c.UserId, c.SessionId });
            e.HasOne(c => c.User).WithMany(u => u.ChatHistories)
             .HasForeignKey(c => c.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // Subscription
        mb.Entity<Subscription>(e => {
            e.Property(s => s.Amount).HasColumnType("decimal(10,2)");
            e.HasOne(s => s.User).WithMany(u => u.Subscriptions)
             .HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // AIMealPlan
        mb.Entity<AIMealPlan>(e => {
            e.HasOne(a => a.User).WithMany(u => u.AIMealPlans)
             .HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // Seed Recipes
        mb.Entity<Recipe>().HasData(
            new Recipe { Id=1, Name="Salad ức gà nướng",   Calories=320, Protein=35.0, Carb=12.0, Fat=14.0, Fiber=4.0, Category="Lunch",     Tags="[\"high-protein\",\"low-carb\"]",  IsActive=true },
            new Recipe { Id=2, Name="Yến mạch trái cây",   Calories=280, Protein=8.0,  Carb=52.0, Fat=5.0,  Fiber=6.0, Category="Breakfast", Tags="[\"fiber\",\"energy\"]",           IsActive=true },
            new Recipe { Id=3, Name="Cơm gạo lứt cá hồi",  Calories=450, Protein=42.0, Carb=48.0, Fat=12.0, Fiber=5.0, Category="Lunch",     Tags="[\"omega3\",\"balanced\"]",        IsActive=true },
            new Recipe { Id=4, Name="Smoothie bơ chuối",   Calories=380, Protein=12.0, Carb=38.0, Fat=20.0, Fiber=6.0, Category="Snack",     Tags="[\"smoothie\",\"healthy-fat\"]",   IsActive=true },
            new Recipe { Id=5, Name="Trứng bác rau cải",   Calories=210, Protein=18.0, Carb=6.0,  Fat=13.0, Fiber=2.0, Category="Breakfast", Tags="[\"low-carb\",\"quick\"]",         IsActive=true },
            new Recipe { Id=6, Name="Gà luộc rau hấp",     Calories=290, Protein=40.0, Carb=14.0, Fat=6.0,  Fiber=5.0, Category="Dinner",    Tags="[\"lean\",\"clean\"]",             IsActive=true },
            new Recipe { Id=7, Name="Overnight oats",      Calories=310, Protein=14.0, Carb=44.0, Fat=8.0,  Fiber=8.0, Category="Breakfast", Tags="[\"prep-ahead\",\"fiber\"]",       IsActive=true },
            new Recipe { Id=8, Name="Bowl cá ngừ quinoa",  Calories=400, Protein=38.0, Carb=42.0, Fat=10.0, Fiber=5.0, Category="Dinner",    Tags="[\"complete-protein\",\"grain\"]", IsActive=true }
        );
    }
}
