using EcoClean.API.Data;
using EcoClean.API.DTOs;
using EcoClean.API.Helpers;
using EcoClean.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EcoClean.API.Services;

// ═══════════════════════════════════════════════════════════════
// AUTH SERVICE
// ═══════════════════════════════════════════════════════════════
public interface IAuthService
{
    Task<TokenDto?> RegisterAsync(RegisterDto dto);
    Task<TokenDto?> LoginAsync(LoginDto dto);
}

public class AuthService : IAuthService
{
    private readonly EcoCleanDbContext _db;
    private readonly JwtHelper _jwt;

    public AuthService(EcoCleanDbContext db, JwtHelper jwt) { _db = db; _jwt = jwt; }

    public async Task<TokenDto?> RegisterAsync(RegisterDto dto)
    {
        if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
            return null;

        var user = new User
        {
            Email = dto.Email,
            FullName = dto.FullName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return new TokenDto(_jwt.GenerateToken(user), user.Email, user.FullName, user.Role, user.IsPremium);
    }

    public async Task<TokenDto?> LoginAsync(LoginDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return null;

        // Auto-expire premium
        if (user.IsPremium && user.PremiumExpiry < DateTime.UtcNow)
        {
            user.IsPremium = false;
            user.Role = "User";
            await _db.SaveChangesAsync();
        }

        return new TokenDto(_jwt.GenerateToken(user), user.Email, user.FullName, user.Role, user.IsPremium);
    }
}

// ═══════════════════════════════════════════════════════════════
// PROFILE / BMI SERVICE
// ═══════════════════════════════════════════════════════════════
public interface IProfileService
{
    Task<ProfileDto> UpsertAsync(int userId, ProfileInputDto dto);
    Task<ProfileDto?> GetAsync(int userId);
}

public class ProfileService : IProfileService
{
    private readonly EcoCleanDbContext _db;
    public ProfileService(EcoCleanDbContext db) => _db = db;

    public async Task<ProfileDto> UpsertAsync(int userId, ProfileInputDto dto)
    {
        var bmi  = BMICalculator.Calculate(dto.Weight, dto.Height);
        var cat  = BMICalculator.Classify(bmi);
        var bmr  = TDEECalculator.BMR(dto.Weight, dto.Height, dto.Age, dto.Gender);
        var tdee = TDEECalculator.TDEE(bmr, dto.ActivityLevel);
        var goal = TDEECalculator.DailyCalorieGoal(tdee, dto.Goal);

        var profile = await _db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        if (profile == null)
        {
            profile = new UserProfile { UserId = userId };
            _db.UserProfiles.Add(profile);
        }

        profile.Height = dto.Height; profile.Weight = dto.Weight;
        profile.Age = dto.Age; profile.Gender = dto.Gender;
        profile.ActivityLevel = dto.ActivityLevel; profile.Goal = dto.Goal;
        profile.BMI = Math.Round(bmi, 1);
        profile.BMICategory = cat;
        profile.TDEE = Math.Round(tdee, 0);
        profile.DailyCalorieGoal = goal;
        profile.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return ToDto(profile);
    }

    public async Task<ProfileDto?> GetAsync(int userId)
    {
        var p = await _db.UserProfiles.FirstOrDefaultAsync(x => x.UserId == userId);
        return p == null ? null : ToDto(p);
    }

    private static ProfileDto ToDto(UserProfile p) => new(
        p.Height, p.Weight, p.Age, p.Gender, p.ActivityLevel,
        p.BMI, p.BMICategory, p.TDEE, p.DailyCalorieGoal, p.Goal, p.UpdatedAt);
}

// ═══════════════════════════════════════════════════════════════
// RECIPE SERVICE
// ═══════════════════════════════════════════════════════════════
public interface IRecipeService
{
    Task<List<RecipeDto>> GetAllAsync(RecipeFilterDto filter);
    Task<RecipeDto?> GetByIdAsync(int id);
}

public class RecipeService : IRecipeService
{
    private readonly EcoCleanDbContext _db;
    public RecipeService(EcoCleanDbContext db) => _db = db;

    public async Task<List<RecipeDto>> GetAllAsync(RecipeFilterDto filter)
    {
        var q = _db.Recipes.Where(r => r.IsActive).AsQueryable();

        if (!string.IsNullOrEmpty(filter.Category))
            q = q.Where(r => r.Category == filter.Category);
        if (filter.MaxCalories.HasValue)
            q = q.Where(r => r.Calories <= filter.MaxCalories.Value);
        if (filter.MinProtein.HasValue)
            q = q.Where(r => r.Protein >= filter.MinProtein.Value);

        var list = await q.ToListAsync();

        if (!string.IsNullOrEmpty(filter.Tag))
            list = list.Where(r => r.Tags != null && r.Tags.Contains(filter.Tag)).ToList();

        return list.Select(ToDto).ToList();
    }

    public async Task<RecipeDto?> GetByIdAsync(int id)
    {
        var r = await _db.Recipes.FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
        return r == null ? null : ToDto(r);
    }

    public static RecipeDto ToDto(Recipe r) => new(
        r.Id, r.Name, r.Description, r.Calories, r.Protein, r.Carb, r.Fat, r.Fiber,
        r.Category, r.Tags, r.ImageUrl, r.PrepTimeMin);
}

// ═══════════════════════════════════════════════════════════════
// MEAL PLAN SERVICE
// ═══════════════════════════════════════════════════════════════
public interface IMealPlanService
{
    Task<List<MealPlanDto>> GetByDateAsync(int userId, DateOnly date);
    Task<List<MealPlanDto>> GetByRangeAsync(int userId, DateOnly from, DateOnly to);
    Task<MealPlanDto> CreateAsync(int userId, MealPlanCreateDto dto);
    Task<bool> DeleteAsync(int userId, int id);
}

public class MealPlanService : IMealPlanService
{
    private readonly EcoCleanDbContext _db;
    public MealPlanService(EcoCleanDbContext db) => _db = db;

    public async Task<List<MealPlanDto>> GetByDateAsync(int userId, DateOnly date)
        => await QueryMealPlans(userId).Where(m => m.PlanDate == date).Select(m => ToDto(m)).ToListAsync();

    public async Task<List<MealPlanDto>> GetByRangeAsync(int userId, DateOnly from, DateOnly to)
        => await QueryMealPlans(userId).Where(m => m.PlanDate >= from && m.PlanDate <= to).Select(m => ToDto(m)).ToListAsync();

    public async Task<MealPlanDto> CreateAsync(int userId, MealPlanCreateDto dto)
    {
        var mp = new MealPlan
        {
            UserId = userId, Title = dto.Title, MealType = dto.MealType,
            PlanDate = dto.PlanDate, RecipeId = dto.RecipeId,
            Calories = dto.Calories, Notes = dto.Notes
        };
        _db.MealPlans.Add(mp);
        await _db.SaveChangesAsync();
        await _db.Entry(mp).Reference(x => x.Recipe).LoadAsync();
        return ToDto(mp);
    }

    public async Task<bool> DeleteAsync(int userId, int id)
    {
        var mp = await _db.MealPlans.FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);
        if (mp == null) return false;
        _db.MealPlans.Remove(mp);
        await _db.SaveChangesAsync();
        return true;
    }

    private IQueryable<MealPlan> QueryMealPlans(int userId)
        => _db.MealPlans.Include(m => m.Recipe).Where(m => m.UserId == userId).OrderBy(m => m.MealType);

    private static MealPlanDto ToDto(MealPlan m) => new(
        m.Id, m.Title, m.MealType, m.PlanDate, m.Calories, m.Notes,
        m.Recipe == null ? null : RecipeService.ToDto(m.Recipe));
}

// ═══════════════════════════════════════════════════════════════
// WEIGHT LOG SERVICE
// ═══════════════════════════════════════════════════════════════
public interface IWeightService
{
    Task<List<WeightLogDto>> GetHistoryAsync(int userId, int days);
    Task<WeightLogDto> LogAsync(int userId, WeightLogCreateDto dto);
    Task<bool> DeleteAsync(int userId, int id);
}

public class WeightService : IWeightService
{
    private readonly EcoCleanDbContext _db;
    public WeightService(EcoCleanDbContext db) => _db = db;

    public async Task<List<WeightLogDto>> GetHistoryAsync(int userId, int days)
    {
        var since = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-days));
        return await _db.WeightLogs
            .Where(w => w.UserId == userId && w.LogDate >= since)
            .OrderBy(w => w.LogDate)
            .Select(w => new WeightLogDto(w.Id, w.Weight, w.LogDate, w.Notes))
            .ToListAsync();
    }

    public async Task<WeightLogDto> LogAsync(int userId, WeightLogCreateDto dto)
    {
        var log = new WeightLog { UserId = userId, Weight = dto.Weight, LogDate = dto.LogDate, Notes = dto.Notes };
        _db.WeightLogs.Add(log);
        await _db.SaveChangesAsync();
        return new WeightLogDto(log.Id, log.Weight, log.LogDate, log.Notes);
    }

    public async Task<bool> DeleteAsync(int userId, int id)
    {
        var log = await _db.WeightLogs.FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
        if (log == null) return false;
        _db.WeightLogs.Remove(log);
        await _db.SaveChangesAsync();
        return true;
    }
}

// ═══════════════════════════════════════════════════════════════
// SUBSCRIPTION SERVICE
// ═══════════════════════════════════════════════════════════════
public interface ISubscriptionService
{
    Task<SubscriptionDto> CreateAsync(int userId, SubscriptionCreateDto dto);
    Task<SubscriptionDto?> ConfirmPaymentAsync(int userId, int subId, string txRef);
    Task<SubscriptionDto?> GetActiveAsync(int userId);
}

public class SubscriptionService : ISubscriptionService
{
    private readonly EcoCleanDbContext _db;
    private readonly IConfiguration _config;

    private static readonly Dictionary<string, (decimal price, int months)> Plans = new()
    {
        ["Monthly"] = (99_000m, 1),
        ["Yearly"]  = (799_000m, 12)
    };

    public SubscriptionService(EcoCleanDbContext db, IConfiguration config) { _db = db; _config = config; }

    public async Task<SubscriptionDto> CreateAsync(int userId, SubscriptionCreateDto dto)
    {
        if (!Plans.TryGetValue(dto.Plan, out var plan))
            plan = Plans["Monthly"];

        var txRef = $"ECO{userId}{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
        var bankCode    = _config["Payment:BankCode"] ?? "MB";
        var bankAccount = _config["Payment:AccountNumber"] ?? "0000000000";
        var desc = Uri.EscapeDataString($"ECOCLEAN {txRef}");
        var qr = $"https://img.vietqr.io/image/{bankCode}-{bankAccount}-compact.png?amount={plan.price}&addInfo={desc}&accountName=ECOCLEAN";

        var sub = new Subscription
        {
            UserId = userId, Plan = dto.Plan,
            Amount = plan.price, TransactionRef = txRef, QRCode = qr
        };
        _db.Subscriptions.Add(sub);
        await _db.SaveChangesAsync();
        return ToDto(sub);
    }

    public async Task<SubscriptionDto?> ConfirmPaymentAsync(int userId, int subId, string txRef)
    {
        var sub = await _db.Subscriptions
            .FirstOrDefaultAsync(s => s.Id == subId && s.UserId == userId && s.TransactionRef == txRef);
        if (sub == null) return null;

        // Only confirm if still pending
        if (sub.Status == "Paid") return ToDto(sub);

        sub.Status = "Paid";
        sub.PaidAt = DateTime.UtcNow;
        sub.ExpiresAt = DateTime.UtcNow.AddMonths(Plans.TryGetValue(sub.Plan, out var p) ? p.months : 1);

        var user = await _db.Users.FindAsync(userId);
        if (user != null)
        {
            user.IsPremium = true;
            user.PremiumExpiry = sub.ExpiresAt;
            user.Role = "Premium";
        }

        await _db.SaveChangesAsync();
        return ToDto(sub);
    }

    public async Task<SubscriptionDto?> GetActiveAsync(int userId)
    {
        var sub = await _db.Subscriptions
            .Where(s => s.UserId == userId && s.Status == "Paid" && s.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(s => s.ExpiresAt)
            .FirstOrDefaultAsync();
        return sub == null ? null : ToDto(sub);
    }

    private static SubscriptionDto ToDto(Subscription s) =>
        new(s.Id, s.Plan, s.Amount, s.Status, s.QRCode, s.TransactionRef, s.PaidAt, s.ExpiresAt);
}

// ═══════════════════════════════════════════════════════════════
// GEMINI AI SERVICE
// ═══════════════════════════════════════════════════════════════
public interface IGeminiService
{
    Task<string> GenerateTextAsync(string prompt);
    Task<string> AnalyzeImageAsync(string base64Image, string prompt);
}

public class GeminiService : IGeminiService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";

    public GeminiService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["GeminiApiKey"] ?? throw new InvalidOperationException("GeminiApiKey not configured");
    }

    public async Task<string> GenerateTextAsync(string prompt)
    {
        var payload = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };
        var res = await _http.PostAsJsonAsync($"{BaseUrl}?key={_apiKey}", payload);
        return await ExtractText(res);
    }

    public async Task<string> AnalyzeImageAsync(string base64Image, string prompt)
    {
        var payload = new
        {
            contents = new[] {
                new {
                    parts = new object[] {
                        new { inline_data = new { mime_type = "image/jpeg", data = base64Image } },
                        new { text = prompt }
                    }
                }
            }
        };
        var res = await _http.PostAsJsonAsync($"{BaseUrl}?key={_apiKey}", payload);
        return await ExtractText(res);
    }

    private static async Task<string> ExtractText(HttpResponseMessage res)
    {
        res.EnsureSuccessStatusCode();
        using var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
        return doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? string.Empty;
    }
}

// ═══════════════════════════════════════════════════════════════
// FOOD SCAN SERVICE (Premium)
// ═══════════════════════════════════════════════════════════════
public interface IFoodScanService
{
    Task<FoodScanResultDto> ScanAsync(int userId, string base64Image);
    Task<List<FoodScanResultDto>> GetHistoryAsync(int userId, int page);
}

public class FoodScanService : IFoodScanService
{
    private readonly EcoCleanDbContext _db;
    private readonly IGeminiService _ai;

    private const string ScanPrompt = """
        Analyze this food image and return JSON ONLY (no markdown, no explanation):
        {"foodName":"string","calories":number,"protein":number,"carb":number,"fat":number,"confidence":number}
        Use grams for macros, kcal for calories, 0-1 for confidence. Vietnamese food names preferred.
        """;

    public FoodScanService(EcoCleanDbContext db, IGeminiService ai) { _db = db; _ai = ai; }

    public async Task<FoodScanResultDto> ScanAsync(int userId, string base64Image)
    {
        var raw = await _ai.AnalyzeImageAsync(base64Image, ScanPrompt);

        try
        {
            var clean = raw.Trim().TrimStart('`').TrimEnd('`');
            if (clean.StartsWith("json")) clean = clean[4..].Trim();
            var json = JsonDocument.Parse(clean).RootElement;

            var scan = new FoodScan
            {
                UserId    = userId,
                ImageUrl  = $"data:image/jpeg;base64,{base64Image[..Math.Min(50, base64Image.Length)]}...",
                FoodName  = json.TryGetProperty("foodName",   out var fn) ? fn.GetString() : null,
                Calories  = json.TryGetProperty("calories",   out var c)  ? (int?)c.GetDouble() : null,
                Protein   = json.TryGetProperty("protein",    out var p)  ? (float?)p.GetDouble() : null,
                Carb      = json.TryGetProperty("carb",       out var ca) ? (float?)ca.GetDouble() : null,
                Fat       = json.TryGetProperty("fat",        out var f)  ? (float?)f.GetDouble() : null,
                Confidence = json.TryGetProperty("confidence", out var cf) ? (float?)cf.GetDouble() : null,
                RawAIResponse = raw
            };
            _db.FoodScans.Add(scan);
            await _db.SaveChangesAsync();

            return new FoodScanResultDto(scan.Id, scan.FoodName, scan.Calories,
                scan.Protein, scan.Carb, scan.Fat, scan.Confidence, scan.ImageUrl, scan.ScannedAt);
        }
        catch
        {
            return new FoodScanResultDto(0, "Không nhận diện được", null, null, null, null, 0f, string.Empty, DateTime.UtcNow);
        }
    }

    public async Task<List<FoodScanResultDto>> GetHistoryAsync(int userId, int page)
    {
        const int pageSize = 10;
        return await _db.FoodScans
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.ScannedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(f => new FoodScanResultDto(f.Id, f.FoodName, f.Calories,
                f.Protein, f.Carb, f.Fat, f.Confidence, f.ImageUrl, f.ScannedAt))
            .ToListAsync();
    }
}

// ═══════════════════════════════════════════════════════════════
// CHATBOT SERVICE (Premium)
// ═══════════════════════════════════════════════════════════════
public interface IChatbotService
{
    Task<ChatResponseDto> ChatAsync(int userId, ChatMessageDto dto);
    Task<List<ChatHistory>> GetSessionAsync(int userId, string sessionId);
}

public class ChatbotService : IChatbotService
{
    private readonly EcoCleanDbContext _db;
    private readonly IGeminiService _ai;

    private const string SystemContext = """
        Bạn là chuyên gia dinh dưỡng EcoClean AI. Chuyên tư vấn về:
        - Chế độ ăn Eat Clean
        - Tính toán calories và dinh dưỡng
        - Thực đơn lành mạnh
        - Mục tiêu giảm/tăng cân
        Trả lời ngắn gọn, thực tế, bằng tiếng Việt.
        """;

    public ChatbotService(EcoCleanDbContext db, IGeminiService ai) { _db = db; _ai = ai; }

    public async Task<ChatResponseDto> ChatAsync(int userId, ChatMessageDto dto)
    {
        var sessionId = dto.SessionId ?? Guid.NewGuid().ToString("N");

        var history = await _db.ChatHistories
            .Where(c => c.UserId == userId && c.SessionId == sessionId)
            .OrderBy(c => c.CreatedAt).TakeLast(10).ToListAsync();

        var sb = new System.Text.StringBuilder(SystemContext);
        sb.AppendLine("\n\nLịch sử hội thoại:");
        foreach (var h in history)
            sb.AppendLine($"{(h.Role == "user" ? "Người dùng" : "AI")}: {h.Content}");
        sb.AppendLine($"\nNgười dùng: {dto.Message}");
        sb.AppendLine("AI:");

        var reply = await _ai.GenerateTextAsync(sb.ToString());

        _db.ChatHistories.AddRange(
            new ChatHistory { UserId = userId, SessionId = sessionId, Role = "user",      Content = dto.Message },
            new ChatHistory { UserId = userId, SessionId = sessionId, Role = "assistant", Content = reply }
        );
        await _db.SaveChangesAsync();

        return new ChatResponseDto(reply, sessionId);
    }

    public async Task<List<ChatHistory>> GetSessionAsync(int userId, string sessionId)
        => await _db.ChatHistories
            .Where(c => c.UserId == userId && c.SessionId == sessionId)
            .OrderBy(c => c.CreatedAt).ToListAsync();
}

// ═══════════════════════════════════════════════════════════════
// AI MEAL PLAN SERVICE (Premium)
// ═══════════════════════════════════════════════════════════════
public interface IAIMealPlanService
{
    Task<AIMealPlanDto> GenerateAsync(int userId, AIMealPlanRequestDto dto);
    Task<List<AIMealPlanDto>> GetHistoryAsync(int userId);
}

public class AIMealPlanService : IAIMealPlanService
{
    private readonly EcoCleanDbContext _db;
    private readonly IGeminiService _ai;

    public AIMealPlanService(EcoCleanDbContext db, IGeminiService ai) { _db = db; _ai = ai; }

    public async Task<AIMealPlanDto> GenerateAsync(int userId, AIMealPlanRequestDto req)
    {
        var profile = await _db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        var targetCal = profile == null ? 1800 : profile.DailyCalorieGoal;

        // Use string.Format to avoid interpolation conflicts with JSON braces
        var prompt = string.Format(
            "Tạo thực đơn Eat Clean {0} ngày cho mục tiêu: {1}.\n" +
            "Thông tin: {2} kcal/ngày.\n\n" +
            "Trả về JSON duy nhất theo format (không có markdown, không giải thích):\n" +
            "{{\"title\":\"string\",\"days\":[{{\"day\":1,\"breakfast\":{{\"name\":\"string\",\"calories\":0,\"protein\":0,\"carb\":0,\"fat\":0}},\"lunch\":{{\"name\":\"string\",\"calories\":0,\"protein\":0,\"carb\":0,\"fat\":0}},\"dinner\":{{\"name\":\"string\",\"calories\":0,\"protein\":0,\"carb\":0,\"fat\":0}},\"snack\":{{\"name\":\"string\",\"calories\":0,\"protein\":0,\"carb\":0,\"fat\":0}},\"totalCalories\":0}}]}}\n\n" +
            "Món ăn Việt Nam, đơn giản, lành mạnh.",
            req.DurationDays, req.Goal, targetCal);

        var raw = await _ai.GenerateTextAsync(prompt);

        var plan = new AIMealPlan
        {
            UserId = userId,
            Goal = req.Goal,
            TargetCalories = targetCal,
            DurationDays = req.DurationDays,
            Title = $"Thực đơn {req.Goal} {req.DurationDays} ngày",
            PlanJson = raw
        };

        _db.AIMealPlans.Add(plan);
        await _db.SaveChangesAsync();

        return ToDto(plan);
    }

    public async Task<List<AIMealPlanDto>> GetHistoryAsync(int userId)
        => await _db.AIMealPlans
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => ToDto(a))
            .ToListAsync();

    private static AIMealPlanDto ToDto(AIMealPlan a) =>
        new(a.Id, a.Title, a.Goal, a.TargetCalories, a.DurationDays, a.PlanJson, a.CreatedAt);
}
