using EcoClean.MVC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EcoClean.MVC.Controllers;

// ── Base ──────────────────────────────────────────────────────
public abstract class BaseController : Controller
{
    protected bool IsAuthenticated => Request.Cookies.ContainsKey("ecoclean_token");

    // Admin cũng có quyền premium
    protected bool IsPremium
    {
        get
        {
            var fromJwt  = HttpContext.Items["IsPremium"]?.ToString() == "True";
            var role     = HttpContext.Items["Role"]?.ToString()
                        ?? Request.Cookies["ecoclean_role"] ?? "";
            return fromJwt || role == "Admin";
        }
    }

    protected IActionResult RequireAuth()
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");
        return null!;
    }
}

// ═══════════════════════════════════════════════════════════════
// HOME
// ═══════════════════════════════════════════════════════════════
public class HomeController : BaseController
{
    public IActionResult Index() => View();
    public IActionResult Privacy() => View();
}

// ═══════════════════════════════════════════════════════════════
// ACCOUNT
// ═══════════════════════════════════════════════════════════════
public class AccountController : BaseController
{
    private readonly ApiClient _api;
    private readonly IWebHostEnvironment _env;
    public AccountController(ApiClient api, IWebHostEnvironment env) { _api = api; _env = env; }

    [HttpGet] public IActionResult Login() => View();
    [HttpGet] public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        JsonElement result;
        try
        {
            result = await _api.PostAsync<JsonElement>("auth/login", new { email, password });
        }
        catch (ApiException)
        {
            ViewBag.Error = "Email hoặc mật khẩu không đúng";
            return View();
        }
        catch
        {
            ViewBag.Error = "Không thể kết nối đến máy chủ. Vui lòng thử lại.";
            return View();
        }

        if (result.ValueKind == JsonValueKind.Undefined ||
            !result.TryGetProperty("accessToken", out var tokenProp))
        {
            ViewBag.Error = "Email hoặc mật khẩu không đúng";
            return View();
        }

        // Secure=true chỉ khi chạy HTTPS (production), false khi dev HTTP
        var isHttps = _env.IsProduction();

        Response.Cookies.Append("ecoclean_token", tokenProp.GetString()!, new CookieOptions
        {
            HttpOnly = true,
            Secure   = isHttps,
            SameSite = isHttps ? SameSiteMode.Strict : SameSiteMode.Lax,
            Expires  = DateTimeOffset.UtcNow.AddDays(7)
        });

        if (result.TryGetProperty("fullName", out var name))
        {
            Response.Cookies.Append(
                "ecoclean_name",
                name.GetString()!,
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                });
        }

        if (result.TryGetProperty("role", out var role))
        {
            Response.Cookies.Append(
                "ecoclean_role",
                role.GetString()!,
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                });
        }

        // Admin đi thẳng vào Admin Dashboard
        var roleValue = result.TryGetProperty("role", out var rv) ? rv.GetString() : "User";
        return roleValue == "Admin"
            ? RedirectToAction("Index", "Admin")
            : RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    public async Task<IActionResult> Register(string email, string password, string fullName)
    {
        var result = await _api.PostAsync<JsonElement>(
            "auth/register",
            new { email, password, fullName });

        if (result.ValueKind == JsonValueKind.Undefined)
        {
            ViewBag.Error = "Email đã tồn tại hoặc có lỗi xảy ra";
            return View();
        }

        return RedirectToAction("Login");
    }

    public IActionResult Logout()
    {
        Response.Cookies.Delete("ecoclean_token");
        Response.Cookies.Delete("ecoclean_name");
        Response.Cookies.Delete("ecoclean_role");
        return RedirectToAction("Index", "Home");
    }
}

// ═══════════════════════════════════════════════════════════════
// DASHBOARD
// ═══════════════════════════════════════════════════════════════
public class DashboardController : BaseController
{
    private readonly ApiClient _api;
    public DashboardController(ApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");

        ViewBag.Profile    = await _api.GetAsync<JsonElement?>("profile");
        ViewBag.WeightLogs = await _api.GetAsync<JsonElement?>("weight?days=30");
        ViewBag.TodayMeals = await _api.GetAsync<JsonElement?>($"mealplans/date/{DateOnly.FromDateTime(DateTime.Today):yyyy-MM-dd}");
        return View();
    }
}

// ═══════════════════════════════════════════════════════════════
// PROFILE
// ═══════════════════════════════════════════════════════════════
public class ProfileController : BaseController
{
    private readonly ApiClient _api;
    public ProfileController(ApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");
        ViewBag.Profile = await _api.GetAsync<JsonElement?>("profile");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Save(double height, double weight, int age,
        string gender, string activityLevel, string goal)
    {
        await _api.PostAsync<JsonElement>("profile", new { height, weight, age, gender, activityLevel, goal });
        return RedirectToAction("Index", "Dashboard");
    }
}

// ═══════════════════════════════════════════════════════════════
// RECIPE
// ═══════════════════════════════════════════════════════════════
public class RecipeController : BaseController
{
    private readonly ApiClient _api;
    public RecipeController(ApiClient api) => _api = api;

    public async Task<IActionResult> Index(string? category, int? maxCalories, string? tag)
    {
        var q = $"recipes?category={category}&maxCalories={maxCalories}&tag={tag}";
        ViewBag.Recipes  = await _api.GetAsync<JsonElement?>(q);
        ViewBag.Category = category;
        return View();
    }

    public async Task<IActionResult> Detail(int id)
    {
        var recipe = await _api.GetAsync<JsonElement?>($"recipes/{id}");
        if (recipe == null) return NotFound();
        return View(recipe);
    }
}

// ═══════════════════════════════════════════════════════════════
// MEAL PLAN
// ═══════════════════════════════════════════════════════════════
public class MealPlanController : BaseController
{
    private readonly ApiClient _api;
    public MealPlanController(ApiClient api) => _api = api;

    public async Task<IActionResult> Index(string? date)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");
        var d = string.IsNullOrEmpty(date) ? DateOnly.FromDateTime(DateTime.Today) : DateOnly.Parse(date);
        ViewBag.Meals = await _api.GetAsync<JsonElement?>($"mealplans/date/{d:yyyy-MM-dd}");
        ViewBag.Date  = d;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Add(string title, string mealType,
        string planDate, int? recipeId, int calories, string? notes)
    {
        await _api.PostAsync<JsonElement>("mealplans", new
        {
            title,
            mealType,
            planDate,   // gửi string "yyyy-MM-dd", API tự parse DateOnly
            recipeId,
            calories,
            notes
        });
        return RedirectToAction("Index", new { date = planDate });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id, string date)
    {
        await _api.DeleteAsync($"mealplans/{id}");
        return RedirectToAction("Index", new { date });
    }
}

// ═══════════════════════════════════════════════════════════════
// WEIGHT
// ═══════════════════════════════════════════════════════════════
public class WeightController : BaseController
{
    private readonly ApiClient _api;
    public WeightController(ApiClient api) => _api = api;

    public async Task<IActionResult> Index()
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");
        ViewBag.Logs = await _api.GetAsync<JsonElement?>("weight?days=90");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Log(double weight, string logDate, string? notes)
    {
        await _api.PostAsync<JsonElement>("weight", new
        {
            weight,
            logDate,    // gửi string "yyyy-MM-dd", API tự parse DateOnly
            notes
        });
        return RedirectToAction("Index");
    }
}

// ═══════════════════════════════════════════════════════════════
// PREMIUM
// ═══════════════════════════════════════════════════════════════
public class PremiumController : BaseController
{
    private readonly ApiClient _api;
    public PremiumController(ApiClient api) => _api = api;

    // Helper: check auth + premium, redirect nếu thiếu
    private IActionResult? RequirePremium()
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");
        if (!IsPremium)       return RedirectToAction("Payment", "Premium");
        return null;
    }

    public IActionResult Index() => View();

    // ── Food Scan ──────────────────────────────────────────────
    public IActionResult Scan()
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ScanSubmit([FromBody] string base64Image)
    {
        if (!IsAuthenticated) return Unauthorized();
        if (string.IsNullOrEmpty(base64Image))
            return BadRequest(new { error = "Không có dữ liệu ảnh" });
        try
        {
            var result = await _api.PostRawAsync<JsonElement>("foodscan", $"\"{base64Image}\"");
            RefreshTokenIfNeeded(result);
            // Trả về data thật (có thể wrapped hoặc không)
            var data = result.TryGetProperty("data", out var d) ? d : result;
            var freeTrialUsed = result.TryGetProperty("freeTrialUsed", out var ft) ? ft.GetInt32() : (int?)null;
            var freeTrialLimit = result.TryGetProperty("freeTrialLimit", out var fl) ? fl.GetInt32() : 10;
            return Json(new { data, freeTrialUsed, freeTrialLimit });
        }
        catch (ApiException ex) when (ex.StatusCode == 403)
        {
            return StatusCode(403, new { error = "Bạn đã dùng hết 10 lượt miễn phí. Vui lòng nâng cấp Premium." });
        }
        catch (ApiException ex) { return StatusCode(ex.StatusCode, new { error = ex.Message }); }
        catch (Exception ex)    { return StatusCode(500, new { error = ex.Message }); }
    }

    // ── Chatbot ────────────────────────────────────────────────
    public IActionResult Chat()
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChatSend(string message, string? sessionId)
    {
        if (!IsAuthenticated) return Unauthorized();
        try
        {
            var result = await _api.PostAsync<JsonElement>("chat", new { message, sessionId });
            RefreshTokenIfNeeded(result);
            var data = result.TryGetProperty("data", out var d) ? d : result;
            var freeTrialUsed = result.TryGetProperty("freeTrialUsed", out var ft) ? ft.GetInt32() : (int?)null;
            var freeTrialLimit = result.TryGetProperty("freeTrialLimit", out var fl) ? fl.GetInt32() : 10;
            return Json(new { data, freeTrialUsed, freeTrialLimit });
        }
        catch (ApiException ex) when (ex.StatusCode == 403)
        {
            return StatusCode(403, new { error = "Bạn đã dùng hết 10 lượt miễn phí. Vui lòng nâng cấp Premium." });
        }
        catch (ApiException ex) { return StatusCode(ex.StatusCode, new { error = ex.Message }); }
        catch (Exception ex)    { return StatusCode(500, new { error = ex.Message }); }
    }

    // ── AI Meal Plan ───────────────────────────────────────────
    public IActionResult AIPlan()
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AIPlanGenerate(string goal, int durationDays, string? foodContext = null)
    {
        if (!IsAuthenticated) return Unauthorized();
        try
        {
            var result = await _api.PostAsync<JsonElement>("aimealplan", new { goal, durationDays, foodContext });
            RefreshTokenIfNeeded(result);
            var data = result.TryGetProperty("data", out var d) ? d : result;
            var freeTrialUsed = result.TryGetProperty("freeTrialUsed", out var ft) ? ft.GetInt32() : (int?)null;
            var freeTrialLimit = result.TryGetProperty("freeTrialLimit", out var fl) ? fl.GetInt32() : 10;
            return Json(new { data, freeTrialUsed, freeTrialLimit });
        }
        catch (ApiException ex) when (ex.StatusCode == 403)
        {
            return StatusCode(403, new { error = "Bạn đã dùng hết 10 lượt miễn phí. Vui lòng nâng cấp Premium." });
        }
        catch (ApiException ex) { return StatusCode(ex.StatusCode, new { error = ex.Message }); }
        catch (Exception ex)    { return StatusCode(500, new { error = ex.Message }); }
    }

    [HttpGet]
    public async Task<IActionResult> RecipeSuggest(string ingredient)
    {
        if (!IsAuthenticated) return Unauthorized();
        try
        {
            var result = await _api.GetAsync<JsonElement>($"recipesuggestion?ingredient={Uri.EscapeDataString(ingredient)}");
            RefreshTokenIfNeeded(result);
            var data = result.TryGetProperty("data", out var d) ? d : result;
            return Json(new { data });
        }
        catch (ApiException ex) when (ex.StatusCode == 403)
        {
            return StatusCode(403, new { error = "Bạn đã dùng hết 10 lượt miễn phí. Vui lòng nâng cấp Premium." });
        }
        catch (ApiException ex) { return StatusCode(ex.StatusCode, new { error = ex.Message }); }
        catch (Exception ex)    { return StatusCode(500, new { error = ex.Message }); }
    }

    // Helper: nếu API trả về newToken thì cập nhật cookie để JWT có FreeTrialUsed mới
    private void RefreshTokenIfNeeded(JsonElement result)
    {
        if (result.TryGetProperty("newToken", out var nt)
            && nt.ValueKind == JsonValueKind.String
            && nt.GetString() is { Length: > 0 } newToken)
        {
            var isHttps = HttpContext.RequestServices
                .GetRequiredService<IWebHostEnvironment>().IsProduction();
            Response.Cookies.Append("ecoclean_token", newToken, new CookieOptions
            {
                HttpOnly = true,
                Secure   = isHttps,
                SameSite = isHttps ? SameSiteMode.Strict : SameSiteMode.Lax,
                Expires  = DateTimeOffset.UtcNow.AddDays(7)
            });
        }
    }

    // ── Payment (ai cũng vào được) ─────────────────────────────
    public IActionResult Payment()
    {
        // Nếu đã premium thì redirect về trang premium index
        if (IsAuthenticated && IsPremium)
            return RedirectToAction("Index", "Premium");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Subscribe(string plan)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Account");
        var sub = await _api.PostAsync<JsonElement>("subscriptions", new { plan });
        return Json(sub);
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmPayment(int subId, string txRef)
    {
        if (!IsAuthenticated) return Unauthorized();
        var result = await _api.PostAsync<JsonElement>($"subscriptions/{subId}/confirm?txRef={txRef}", new { });
        return Json(result);
    }
}
