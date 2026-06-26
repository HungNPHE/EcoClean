using EcoClean.API.Data;
using EcoClean.API.DTOs;
using EcoClean.API.Helpers;
using EcoClean.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EcoClean.API.Services;

// ═══════════════════════════════════════════════════════════════
// FREE TRIAL SERVICE
// ═══════════════════════════════════════════════════════════════
public interface IFreeTrialService
{
    /// <summary>Tăng counter FreeTrialUsed. Trả về JWT mới (để client refresh token).</summary>
    Task<(bool allowed, string? newToken)> ConsumeAsync(int userId);
}

public class FreeTrialService : IFreeTrialService
{
    public const int Limit = 10;
    private readonly EcoCleanDbContext _db;
    private readonly JwtHelper _jwt;

    public FreeTrialService(EcoCleanDbContext db, JwtHelper jwt) { _db = db; _jwt = jwt; }

    public async Task<(bool allowed, string? newToken)> ConsumeAsync(int userId)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null) return (false, null);

        // Nếu đã premium thì không tốn lượt
        if (user.IsPremium) return (true, null);

        if (user.FreeTrialUsed >= Limit)
            return (false, null);

        user.FreeTrialUsed++;
        await _db.SaveChangesAsync();

        // Tạo JWT mới với FreeTrialUsed đã cập nhật
        var newToken = _jwt.GenerateToken(user);
        return (true, newToken);
    }
}

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
        r.Category, r.Tags, r.ImageUrl, r.PrepTimeMin, r.Ingredients, r.Instructions);
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
        ["Monthly"] = (49_000m, 1),
        ["Yearly"]  = (499_000m, 12)
    };

    public SubscriptionService(EcoCleanDbContext db, IConfiguration config) { _db = db; _config = config; }

    public async Task<SubscriptionDto> CreateAsync(int userId, SubscriptionCreateDto dto)
    {
        if (!Plans.TryGetValue(dto.Plan, out var plan))
            plan = Plans["Monthly"];

        var txRef = $"ECO{userId}{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";

        // Chọn phương thức thanh toán từ config
        var payMethod   = _config["Payment:Method"] ?? "MoMo"; // "MoMo" | "VietQR"
        var momoPhone   = _config["Payment:MoMoPhone"]   ?? "";
        var momoName    = _config["Payment:MoMoName"]    ?? "ECOCLEAN";
        var bankCode    = _config["Payment:BankCode"]    ?? "TCB";
        var bankAccount = _config["Payment:AccountNumber"] ?? "";

        string qr;
        if (payMethod == "MoMo" && !string.IsNullOrEmpty(momoPhone))
        {
            // MoMo QR tĩnh — format deeplink hiển thị QR trong app MoMo
            // Dùng api.vietqr.io để render QR từ số MoMo (MoMo dùng VietQR standard)
            var note = Uri.EscapeDataString($"ECOCLEAN {txRef}");
            qr = $"https://img.vietqr.io/image/MOMO-{momoPhone}-compact.png" +
                 $"?amount={plan.price}&addInfo={note}&accountName={Uri.EscapeDataString(momoName)}";
        }
        else
        {
            // Fallback VietQR ngân hàng
            var desc = Uri.EscapeDataString($"ECOCLEAN {txRef}");
            qr = $"https://img.vietqr.io/image/{bankCode}-{bankAccount}-compact.png" +
                 $"?amount={plan.price}&addInfo={desc}&accountName=ECOCLEAN";
        }

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

        // Với flow thủ công (MoMo QR tĩnh), user chỉ "thông báo đã chuyển khoản"
        // Không tự confirm — admin phải duyệt qua /api/admin/subscriptions/{id}/confirm
        // Chỉ trả về trạng thái hiện tại (vẫn Pending) để FE hiển thị "Chờ duyệt"
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
// GEMINI AI SERVICE  (vision-capable — dùng cho Food Scan)
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
    private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

    public GeminiService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["GeminiApiKey"] ?? throw new InvalidOperationException("GeminiApiKey not configured");
    }

    public async Task<string> GenerateTextAsync(string prompt)
    {
        var payload = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };
        var res = await SendWithRetryAsync(() => _http.PostAsJsonAsync($"{BaseUrl}?key={_apiKey}", payload));
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
        var res = await SendWithRetryAsync(() => _http.PostAsJsonAsync($"{BaseUrl}?key={_apiKey}", payload));
        return await ExtractText(res);
    }

    // Retry tối đa 3 lần khi gặp 429 (rate limit)
    private static async Task<HttpResponseMessage> SendWithRetryAsync(Func<Task<HttpResponseMessage>> send)
    {
        int[] delays = [5000, 15000, 30000]; // 5s, 15s, 30s
        for (int i = 0; i <= delays.Length; i++)
        {
            var res = await send();
            if ((int)res.StatusCode != 429 || i == delays.Length)
                return res;
            await Task.Delay(delays[i]);
        }
        return await send(); // fallback
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
// GROQ AI SERVICE  (text-only — dùng cho Chatbot & Meal Plan)
// Tốc độ ~500 tokens/s nhờ LPU, free tier 14,400 req/ngày
// ═══════════════════════════════════════════════════════════════
public interface IGroqService
{
    Task<string> GenerateTextAsync(string prompt);
    Task<string> GenerateChatAsync(string systemPrompt, IEnumerable<(string role, string content)> history, string userMessage);
}

public class GroqService : IGroqService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private readonly string _model;
    private const string BaseUrl = "https://api.groq.com/openai/v1/chat/completions";

    public GroqService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["GroqApiKey"] ?? throw new InvalidOperationException("GroqApiKey not configured");
        _model  = config["GroqModel"] ?? "llama-3.3-70b-versatile";

        _http.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
    }

    public Task<string> GenerateTextAsync(string prompt)
        => SendAsync([new { role = "user", content = prompt }]);

    public Task<string> GenerateChatAsync(
        string systemPrompt,
        IEnumerable<(string role, string content)> history,
        string userMessage)
    {
        var messages = new List<object>
        {
            new { role = "system", content = systemPrompt }
        };
        foreach (var (role, content) in history)
            messages.Add(new { role, content });
        messages.Add(new { role = "user", content = userMessage });

        return SendAsync(messages);
    }

    private async Task<string> SendAsync(IEnumerable<object> messages)
    {
        var payload = new
        {
            model    = _model,
            messages = messages,
            temperature = 0.7,
            max_tokens  = 2048
        };

        // Retry tối đa 3 lần khi gặp 429
        int[] delays = [3000, 8000, 20000];
        HttpResponseMessage? res = null;
        for (int i = 0; i <= delays.Length; i++)
        {
            res = await _http.PostAsJsonAsync(BaseUrl, payload);
            if ((int)res.StatusCode != 429 || i == delays.Length) break;
            await Task.Delay(delays[i]);
        }

        res!.EnsureSuccessStatusCode();
        using var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
        return doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
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
// SEPAY WEBHOOK SERVICE
// Tự động xác nhận thanh toán khi SePay gửi webhook
// ═══════════════════════════════════════════════════════════════
public interface ISePayWebhookService
{
    Task<bool> ProcessAsync(SePayWebhookDto payload, string? signature, string rawBody);
}

public class SePayWebhookService : ISePayWebhookService
{
    private readonly EcoCleanDbContext _db;
    private readonly IConfiguration _config;
    private readonly ILogger<SePayWebhookService> _logger;

    private static readonly Dictionary<string, (decimal price, int months)> Plans = new()
    {
        ["Monthly"] = (49_000m, 1),
        ["Yearly"]  = (499_000m, 12)
    };

    public SePayWebhookService(
        EcoCleanDbContext db,
        IConfiguration config,
        ILogger<SePayWebhookService> logger)
    {
        _db     = db;
        _config = config;
        _logger = logger;
    }

    public async Task<bool> ProcessAsync(SePayWebhookDto payload, string? signature, string rawBody)
    {
        // 1. Chỉ xử lý tiền VÀO
        if (payload.TransferType != 1)
            return true;

        // 2. Verify HMAC-SHA256 signature nếu có cấu hình secret
        var secret = _config["SePay:WebhookSecret"];
        if (!string.IsNullOrEmpty(secret) && !string.IsNullOrEmpty(signature))
        {
            if (!VerifySignature(rawBody, signature, secret))
            {
                _logger.LogWarning("SePay webhook: invalid signature");
                return false;
            }
        }

        // 3. Tìm txRef trong nội dung chuyển khoản (format: "ECOCLEAN ECO{userId}{timestamp}")
        var content = payload.Content?.ToUpper() ?? string.Empty;
        var sub = await FindPendingSubscriptionAsync(content, payload.TransferAmount);

        if (sub == null)
        {
            _logger.LogInformation("SePay webhook: no matching pending subscription for content='{Content}' amount={Amount}",
                payload.Content, payload.TransferAmount);
            return true; // Không phải giao dịch của app, bỏ qua
        }

        // 4. Đã paid rồi thì bỏ qua (idempotent)
        if (sub.Status == "Paid")
            return true;

        // 5. Xác nhận thanh toán
        sub.Status = "Paid";
        sub.PaidAt = DateTime.UtcNow;
        sub.ExpiresAt = DateTime.UtcNow.AddMonths(
            Plans.TryGetValue(sub.Plan, out var p) ? p.months : 1);

        var user = await _db.Users.FindAsync(sub.UserId);
        if (user != null)
        {
            user.IsPremium    = true;
            user.PremiumExpiry = sub.ExpiresAt;
            user.Role         = "Premium";
        }

        await _db.SaveChangesAsync();

        _logger.LogInformation("SePay webhook: subscription {SubId} confirmed for user {UserId}, plan={Plan}",
            sub.Id, sub.UserId, sub.Plan);

        return true;
    }

    // Tìm subscription pending khớp với nội dung CK và số tiền
    private async Task<Subscription?> FindPendingSubscriptionAsync(string content, decimal amount)
    {
        // Lấy tất cả pending subscriptions có amount khớp (giới hạn 100 để tránh scan toàn bảng)
        var candidates = await _db.Subscriptions
            .Where(s => s.Status == "Pending" && s.Amount == amount)
            .OrderByDescending(s => s.Id)
            .Take(100)
            .ToListAsync();

        // Match txRef trong nội dung chuyển khoản
        return candidates.FirstOrDefault(s =>
            s.TransactionRef != null &&
            content.Contains(s.TransactionRef.ToUpper()));
    }

    private static bool VerifySignature(string rawBody, string signature, string secret)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA256(
            System.Text.Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(rawBody));
        var expected = Convert.ToHexString(hash).ToLower();
        return string.Equals(expected, signature.ToLower(), StringComparison.OrdinalIgnoreCase);
    }
}
// ═══════════════════════════════════════════════════════════════
public interface IChatbotService
{
    Task<ChatResponseDto> ChatAsync(int userId, ChatMessageDto dto);
    Task<List<ChatHistory>> GetSessionAsync(int userId, string sessionId);
}

public class ChatbotService : IChatbotService
{
    private readonly EcoCleanDbContext _db;
    private readonly IGroqService _ai;

    private const string SystemContext = """
        Bạn là chuyên gia dinh dưỡng EcoClean AI. Chuyên tư vấn về:
        - Chế độ ăn Eat Clean
        - Tính toán calories và dinh dưỡng
        - Thực đơn lành mạnh
        - Mục tiêu giảm/tăng cân
        Trả lời ngắn gọn, thực tế, bằng tiếng Việt.
        """;

    public ChatbotService(EcoCleanDbContext db, IGroqService ai) { _db = db; _ai = ai; }

    public async Task<ChatResponseDto> ChatAsync(int userId, ChatMessageDto dto)
    {
        var sessionId = dto.SessionId ?? Guid.NewGuid().ToString("N");

        var history = await _db.ChatHistories
            .Where(c => c.UserId == userId && c.SessionId == sessionId)
            .OrderByDescending(c => c.CreatedAt)
            .Take(10)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();

        // Dùng GenerateChatAsync để tận dụng native chat format của Groq (nhanh hơn)
        var historyTuples = history.Select(h => (h.Role, h.Content));
        var reply = await _ai.GenerateChatAsync(SystemContext, historyTuples, dto.Message);

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
// RECIPE SUGGESTION SERVICE (Premium)
// ═══════════════════════════════════════════════════════════════
public interface IRecipeSuggestionService
{
    Task<List<RecipeSuggestionDto>> SuggestFromIngredientAsync(string ingredient);
}

public class RecipeSuggestionService : IRecipeSuggestionService
{
    private readonly IGroqService _ai;

    public RecipeSuggestionService(IGroqService ai) => _ai = ai;

    public async Task<List<RecipeSuggestionDto>> SuggestFromIngredientAsync(string ingredient)
    {
        var prompt = string.Format(
            "Gợi ý 4 món ăn Eat Clean lành mạnh có thể làm từ nguyên liệu chính: \"{0}\".\n\n" +
            "Trả về JSON duy nhất (không markdown, không giải thích):\n" +
            "[{{\"dishName\":\"string\",\"description\":\"string\",\"calories\":0,\"protein\":0,\"carb\":0,\"fat\":0,\"prepTime\":\"string\",\"difficulty\":\"Dễ|Trung bình|Khó\",\"steps\":[\"string\"]}}]\n\n" +
            "Yêu cầu: món Việt Nam hoặc phổ biến tại Việt Nam, đơn giản, ít dầu mỡ, steps tối đa 4 bước ngắn gọn.",
            ingredient);

        var raw = await _ai.GenerateTextAsync(prompt);

        try
        {
            var clean = raw.Trim();
            // Strip markdown code block nếu có
            if (clean.StartsWith("```")) clean = System.Text.RegularExpressions.Regex.Replace(clean, @"^```[a-z]*\s*", "").TrimEnd('`').Trim();

            using var doc = JsonDocument.Parse(clean);
            var list = new List<RecipeSuggestionDto>();

            foreach (var item in doc.RootElement.EnumerateArray())
            {
                list.Add(new RecipeSuggestionDto(
                    DishName:    item.TryGetProperty("dishName",    out var dn) ? dn.GetString() ?? "" : "",
                    Description: item.TryGetProperty("description", out var ds) ? ds.GetString() ?? "" : "",
                    Calories:    item.TryGetProperty("calories",    out var c)  ? (int)c.GetDouble()   : 0,
                    Protein:     item.TryGetProperty("protein",     out var p)  ? p.GetDouble()        : 0,
                    Carb:        item.TryGetProperty("carb",        out var ca) ? ca.GetDouble()       : 0,
                    Fat:         item.TryGetProperty("fat",         out var f)  ? f.GetDouble()        : 0,
                    PrepTime:    item.TryGetProperty("prepTime",    out var pt) ? pt.GetString() ?? "" : "",
                    Difficulty:  item.TryGetProperty("difficulty",  out var d)  ? d.GetString()  ?? "" : "",
                    Steps:       item.TryGetProperty("steps",       out var st)
                                    ? st.EnumerateArray().Select(s => s.GetString() ?? "").ToArray()
                                    : []
                ));
            }
            return list;
        }
        catch
        {
            return [];
        }
    }
}

// ═══════════════════════════════════════════════════════════════
// AI MEAL PLAN SERVICE (Premium) — dùng Groq để tăng tốc độ
// ═══════════════════════════════════════════════════════════════
public interface IAIMealPlanService
{
    Task<AIMealPlanDto> GenerateAsync(int userId, AIMealPlanRequestDto dto);
    Task<List<AIMealPlanDto>> GetHistoryAsync(int userId);
}

public class AIMealPlanService : IAIMealPlanService
{
    private readonly EcoCleanDbContext _db;
    private readonly IGroqService _ai;

    private const string MealPlanSystem = """
        Bạn là chuyên gia dinh dưỡng Eat Clean. Khi được yêu cầu tạo thực đơn,
        hãy trả về JSON THUẦN TÚY (không markdown, không giải thích, không ```json).
        Chỉ trả về object JSON hợp lệ theo đúng format được yêu cầu.
        Ưu tiên món ăn Việt Nam, đơn giản, lành mạnh.
        """;

    public AIMealPlanService(EcoCleanDbContext db, IGroqService ai) { _db = db; _ai = ai; }

    public async Task<AIMealPlanDto> GenerateAsync(int userId, AIMealPlanRequestDto req)
    {
        var profile = await _db.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        var targetCal = profile == null ? 1800 : profile.DailyCalorieGoal;

        var foodNote = string.IsNullOrEmpty(req.FoodContext)
            ? string.Empty
            : $"\nGợi ý: người dùng quan tâm đến món \"{req.FoodContext}\", hãy tích hợp hoặc gợi ý các món tương tự.";

        var userPrompt = string.Format(
            "Tạo thực đơn Eat Clean {0} ngày cho mục tiêu: {1}. Mục tiêu calories: {2} kcal/ngày.{3}\n\n" +
            "Format JSON yêu cầu:\n" +
            "{{\"title\":\"string\",\"days\":[{{\"day\":1,\"breakfast\":{{\"name\":\"string\",\"calories\":0,\"protein\":0,\"carb\":0,\"fat\":0}},\"lunch\":{{\"name\":\"string\",\"calories\":0,\"protein\":0,\"carb\":0,\"fat\":0}},\"dinner\":{{\"name\":\"string\",\"calories\":0,\"protein\":0,\"carb\":0,\"fat\":0}},\"snack\":{{\"name\":\"string\",\"calories\":0,\"protein\":0,\"carb\":0,\"fat\":0}},\"totalCalories\":0}}]}}",
            req.DurationDays, req.Goal, targetCal, foodNote);

        var raw = await _ai.GenerateChatAsync(MealPlanSystem, [], userPrompt);

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
