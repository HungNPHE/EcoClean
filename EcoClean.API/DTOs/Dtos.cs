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
    string Category, string? Tags, string? ImageUrl, int? PrepTimeMin,
    string? Ingredients, string? Instructions);

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
public record AIMealPlanRequestDto(string Goal, int DurationDays, string? FoodContext = null);
public record AIMealPlanDto(
    int Id, string Title, string Goal,
    int TargetCalories, int DurationDays,
    string PlanJson, DateTime CreatedAt);

// ── Recipe Suggestion (Premium) ───────────────────────────────
public record RecipeSuggestionDto(
    string DishName, string Description,
    int Calories, double Protein, double Carb, double Fat,
    string PrepTime, string Difficulty, string[] Steps);

// ── SePay Webhook ─────────────────────────────────────────────
// Payload SePay gửi về khi có giao dịch mới
public record SePayWebhookDto(
    string Gateway,           // Tên ngân hàng, vd: "Techcombank"
    string TransactionDate,   // "2024-01-15 10:30:00"
    string AccountNumber,     // Số tài khoản nhận
    string Code,              // Mã giao dịch ngân hàng
    string Content,           // Nội dung chuyển khoản — chứa txRef
    decimal TransferAmount,   // Số tiền
    string ReferenceCode,     // Mã tham chiếu SePay
    int TransferType,         // 1 = tiền vào, 2 = tiền ra
    string Description);      // Mô tả thêm
