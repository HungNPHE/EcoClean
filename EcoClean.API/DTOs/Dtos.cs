namespace EcoClean.API.DTOs;

// ── Auth ──────────────────────────────────────────────────────
public record RegisterDto(string Email, string Password, string FullName);
public record LoginDto(string Email, string Password);
public record TokenDto(string AccessToken, string Email, string FullName, string Role, bool IsPremium);

// ── Profile / BMI ─────────────────────────────────────────────
public record ProfileInputDto(
    double Height, double Weight, int Age,
    string Gender, string ActivityLevel, string Goal);

public record ProfileDto(
    double Height, double Weight, int Age,
    string Gender, string ActivityLevel,
    double BMI, string BMICategory,
    double TDEE, int DailyCalorieGoal, string Goal,
    DateTime UpdatedAt);

public record BMIResultDto(
    float BMI, string Category,
    float TDEE, int DailyCalorieGoal);

// ── Recipe ────────────────────────────────────────────────────
public record RecipeDto(
    int Id, string Name, string? Description,
    int Calories, double Protein, double Carb, double Fat, double Fiber,
    string Category, string? Tags, string? ImageUrl, int? PrepTimeMin);

public record RecipeFilterDto(
    string? Category, int? MaxCalories, int? MinProtein, string? Tag);

// ── MealPlan ──────────────────────────────────────────────────
public record MealPlanCreateDto(
    string Title, string MealType,
    DateOnly PlanDate, int? RecipeId,
    int Calories, string? Notes);

public record MealPlanDto(
    int Id, string Title, string MealType,
    DateOnly PlanDate, int Calories,
    string? Notes, RecipeDto? Recipe);

// ── WeightLog ─────────────────────────────────────────────────
public record WeightLogCreateDto(double Weight, DateOnly LogDate, string? Notes);
public record WeightLogDto(int Id, double Weight, DateOnly LogDate, string? Notes);

// ── FoodScan (Premium) ────────────────────────────────────────
public record FoodScanResultDto(
    int Id, string? FoodName,
    int? Calories, double? Protein, double? Carb, double? Fat,
    double? Confidence, string ImageUrl, DateTime ScannedAt);

// ── Chat (Premium) ────────────────────────────────────────────
public record ChatMessageDto(string Message, string? SessionId);
public record ChatResponseDto(string Reply, string SessionId);

// ── Subscription ──────────────────────────────────────────────
public record SubscriptionCreateDto(string Plan); // Monthly | Yearly
public record SubscriptionDto(
    int Id, string Plan, decimal Amount, string Status,
    string? QRCode, string? TransactionRef,
    DateTime? PaidAt, DateTime? ExpiresAt);

// ── AI Meal Plan (Premium) ────────────────────────────────────
public record AIMealPlanRequestDto(string Goal, int DurationDays);
public record AIMealPlanDto(
    int Id, string Title, string Goal,
    int TargetCalories, int DurationDays,
    string PlanJson, DateTime CreatedAt);
