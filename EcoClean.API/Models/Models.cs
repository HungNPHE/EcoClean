namespace EcoClean.API.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Role { get; set; } = "User";
    public bool IsPremium { get; set; } = false;
    public DateTime? PremiumExpiry { get; set; }
    public int FreeTrialUsed { get; set; } = 0;   // Số lần đã dùng thử premium
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public UserProfile? Profile { get; set; }
    public ICollection<MealPlan> MealPlans { get; set; } = new List<MealPlan>();
    public ICollection<WeightLog> WeightLogs { get; set; } = new List<WeightLog>();
    public ICollection<FoodScan> FoodScans { get; set; } = new List<FoodScan>();
    public ICollection<ChatHistory> ChatHistories { get; set; } = new List<ChatHistory>();
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    public ICollection<AIMealPlan> AIMealPlans { get; set; } = new List<AIMealPlan>();
}

public class UserProfile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public double Height { get; set; }       // cm
    public double Weight { get; set; }       // kg
    public int Age { get; set; }
    public string Gender { get; set; } = "Male";
    public string ActivityLevel { get; set; } = "Sedentary";
    public double BMI { get; set; }
    public string BMICategory { get; set; } = string.Empty;
    public double TDEE { get; set; }
    public int DailyCalorieGoal { get; set; }
    public string Goal { get; set; } = "Maintain";
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}

public class Recipe
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int Calories { get; set; }
    public double Protein { get; set; }
    public double Carb { get; set; }
    public double Fat { get; set; }
    public double Fiber { get; set; }
    public string Category { get; set; } = null!;
    public string? Tags { get; set; }
    public string? ImageUrl { get; set; }
    public int? PrepTimeMin { get; set; }
    public string? Ingredients { get; set; }   // JSON array: ["200g ức gà","1 bát rau xà lách"...]
    public string? Instructions { get; set; }  // JSON array: ["Bước 1: ...","Bước 2: ..."...]
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class MealPlan
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? RecipeId { get; set; }
    public string Title { get; set; } = null!;
    public string MealType { get; set; } = null!;
    public DateOnly PlanDate { get; set; }
    public int Calories { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Recipe? Recipe { get; set; }
}

public class WeightLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public double Weight { get; set; }
    public DateOnly LogDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}

public class FoodScan
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string ImageUrl { get; set; } = null!;
    public string? FoodName { get; set; }
    public int? Calories { get; set; }
    public double? Protein { get; set; }
    public double? Carb { get; set; }
    public double? Fat { get; set; }
    public double? Confidence { get; set; }
    public string? RawAIResponse { get; set; }
    public DateTime ScannedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}

public class ChatHistory
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string SessionId { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}

public class Subscription
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Plan { get; set; } = "Monthly";
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Pending";
    public string? TransactionRef { get; set; }
    public string? QRCode { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}

public class AIMealPlan
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = null!;
    public string Goal { get; set; } = null!;
    public int TargetCalories { get; set; }
    public int DurationDays { get; set; } = 7;
    public string PlanJson { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}
