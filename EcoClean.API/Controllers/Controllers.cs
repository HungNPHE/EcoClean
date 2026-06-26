using EcoClean.API.DTOs;
using EcoClean.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcoClean.API.Controllers;

// ── Base ──────────────────────────────────────────────────────
[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    private const int FreeTrialLimit = 10;

    protected int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    protected bool IsPremium => bool.Parse(User.FindFirstValue("IsPremium") ?? "false");
    protected int FreeTrialUsed => int.TryParse(User.FindFirstValue("FreeTrialUsed"), out var v) ? v : 0;

    // True nếu user có quyền dùng premium (đã mua hoặc còn lượt free trial)
    protected bool CanUsePremium => IsPremium || FreeTrialUsed < FreeTrialLimit;

    protected IActionResult PremiumRequired()
        => StatusCode(403, new {
            error = "Premium required",
            freeTrialUsed = FreeTrialUsed,
            freeTrialLimit = FreeTrialLimit
        });
}

// ═══════════════════════════════════════════════════════════════
// AUTH
// ═══════════════════════════════════════════════════════════════
[Route("api/auth")]
public class AuthController : BaseController
{
    private readonly IAuthService _svc;
    public AuthController(IAuthService svc) => _svc = svc;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _svc.RegisterAsync(dto);
        if (result == null) return Conflict(new { error = "Email already exists" });
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _svc.LoginAsync(dto);
        if (result == null) return Unauthorized(new { error = "Invalid credentials" });
        return Ok(result);
    }
}

// ═══════════════════════════════════════════════════════════════
// PROFILE / BMI
// ═══════════════════════════════════════════════════════════════
[Authorize]
[Route("api/profile")]
public class ProfileController : BaseController
{
    private readonly IProfileService _svc;
    public ProfileController(IProfileService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var p = await _svc.GetAsync(UserId);
        return p == null ? NotFound() : Ok(p);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert([FromBody] ProfileInputDto dto)
        => Ok(await _svc.UpsertAsync(UserId, dto));
}

// ═══════════════════════════════════════════════════════════════
// RECIPES
// ═══════════════════════════════════════════════════════════════
[Route("api/recipes")]
public class RecipeController : BaseController
{
    private readonly IRecipeService _svc;
    public RecipeController(IRecipeService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] RecipeFilterDto filter)
        => Ok(await _svc.GetAllAsync(filter));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var r = await _svc.GetByIdAsync(id);
        return r == null ? NotFound() : Ok(r);
    }
}

// ═══════════════════════════════════════════════════════════════
// MEAL PLANS
// ═══════════════════════════════════════════════════════════════
[Authorize]
[Route("api/mealplans")]
public class MealPlanController : BaseController
{
    private readonly IMealPlanService _svc;
    public MealPlanController(IMealPlanService svc) => _svc = svc;

    [HttpGet("date/{date}")]
    public async Task<IActionResult> GetByDate(DateOnly date)
        => Ok(await _svc.GetByDateAsync(UserId, date));

    [HttpGet("range")]
    public async Task<IActionResult> GetByRange([FromQuery] DateOnly from, [FromQuery] DateOnly to)
        => Ok(await _svc.GetByRangeAsync(UserId, from, to));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MealPlanCreateDto dto)
        => Ok(await _svc.CreateAsync(UserId, dto));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
        => await _svc.DeleteAsync(UserId, id) ? NoContent() : NotFound();
}

// ═══════════════════════════════════════════════════════════════
// WEIGHT
// ═══════════════════════════════════════════════════════════════
[Authorize]
[Route("api/weight")]
public class WeightController : BaseController
{
    private readonly IWeightService _svc;
    public WeightController(IWeightService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetHistory([FromQuery] int days = 30)
        => Ok(await _svc.GetHistoryAsync(UserId, days));

    [HttpPost]
    public async Task<IActionResult> Log([FromBody] WeightLogCreateDto dto)
        => Ok(await _svc.LogAsync(UserId, dto));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
        => await _svc.DeleteAsync(UserId, id) ? NoContent() : NotFound();
}

// ═══════════════════════════════════════════════════════════════
// SUBSCRIPTION
// ═══════════════════════════════════════════════════════════════
[Authorize]
[Route("api/subscriptions")]
public class SubscriptionController : BaseController
{
    private readonly ISubscriptionService _svc;
    public SubscriptionController(ISubscriptionService svc) => _svc = svc;

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var sub = await _svc.GetActiveAsync(UserId);
        return sub == null ? NotFound() : Ok(sub);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SubscriptionCreateDto dto)
        => Ok(await _svc.CreateAsync(UserId, dto));

    [HttpPost("{id}/confirm")]
    public async Task<IActionResult> Confirm(int id, [FromQuery] string txRef)
    {
        var result = await _svc.ConfirmPaymentAsync(UserId, id, txRef);
        return result == null ? NotFound() : Ok(result);
    }
}

// ═══════════════════════════════════════════════════════════════
// FOOD SCAN (Premium)
// ═══════════════════════════════════════════════════════════════
[Authorize]
[Route("api/foodscan")]
public class FoodScanController : BaseController
{
    private readonly IFoodScanService _svc;
    private readonly IFreeTrialService _trial;
    public FoodScanController(IFoodScanService svc, IFreeTrialService trial) { _svc = svc; _trial = trial; }

    [HttpPost]
    public async Task<IActionResult> Scan([FromBody] string base64Image)
    {
        if (!IsPremium)
        {
            var (allowed, newToken) = await _trial.ConsumeAsync(UserId);
            if (!allowed) return PremiumRequired();
            var result = await _svc.ScanAsync(UserId, base64Image);
            return Ok(new { data = result, newToken, freeTrialUsed = FreeTrialUsed + 1, freeTrialLimit = FreeTrialService.Limit });
        }
        return Ok(new { data = await _svc.ScanAsync(UserId, base64Image), newToken = (string?)null });
    }

    [HttpGet("history")]
    public async Task<IActionResult> History([FromQuery] int page = 1)
    {
        if (!CanUsePremium) return PremiumRequired();
        return Ok(await _svc.GetHistoryAsync(UserId, page));
    }
}

// ═══════════════════════════════════════════════════════════════
// CHATBOT (Premium)
// ═══════════════════════════════════════════════════════════════
[Authorize]
[Route("api/chat")]
public class ChatController : BaseController
{
    private readonly IChatbotService _svc;
    private readonly IFreeTrialService _trial;
    public ChatController(IChatbotService svc, IFreeTrialService trial) { _svc = svc; _trial = trial; }

    [HttpPost]
    public async Task<IActionResult> Chat([FromBody] ChatMessageDto dto)
    {
        if (!IsPremium)
        {
            var (allowed, newToken) = await _trial.ConsumeAsync(UserId);
            if (!allowed) return PremiumRequired();
            var result = await _svc.ChatAsync(UserId, dto);
            return Ok(new { data = result, newToken, freeTrialUsed = FreeTrialUsed + 1, freeTrialLimit = FreeTrialService.Limit });
        }
        return Ok(new { data = await _svc.ChatAsync(UserId, dto), newToken = (string?)null });
    }

    [HttpGet("session/{sessionId}")]
    public async Task<IActionResult> GetSession(string sessionId)
    {
        if (!CanUsePremium) return PremiumRequired();
        return Ok(await _svc.GetSessionAsync(UserId, sessionId));
    }
}

// ═══════════════════════════════════════════════════════════════
// AI MEAL PLAN (Premium)
// ═══════════════════════════════════════════════════════════════
[Authorize]
[Route("api/aimealplan")]
public class AIMealPlanController : BaseController
{
    private readonly IAIMealPlanService _svc;
    private readonly IFreeTrialService _trial;
    public AIMealPlanController(IAIMealPlanService svc, IFreeTrialService trial) { _svc = svc; _trial = trial; }

    [HttpPost]
    public async Task<IActionResult> Generate([FromBody] AIMealPlanRequestDto dto)
    {
        if (!IsPremium)
        {
            var (allowed, newToken) = await _trial.ConsumeAsync(UserId);
            if (!allowed) return PremiumRequired();
            var result = await _svc.GenerateAsync(UserId, dto);
            return Ok(new { data = result, newToken, freeTrialUsed = FreeTrialUsed + 1, freeTrialLimit = FreeTrialService.Limit });
        }
        return Ok(new { data = await _svc.GenerateAsync(UserId, dto), newToken = (string?)null });
    }

    [HttpGet("history")]
    public async Task<IActionResult> History()
    {
        if (!CanUsePremium) return PremiumRequired();
        return Ok(await _svc.GetHistoryAsync(UserId));
    }
}

// ═══════════════════════════════════════════════════════════════
// RECIPE SUGGESTION (Premium)
// ═══════════════════════════════════════════════════════════════
[Authorize]
[Route("api/recipesuggestion")]
public class RecipeSuggestionController : BaseController
{
    private readonly IRecipeSuggestionService _svc;
    private readonly IFreeTrialService _trial;
    public RecipeSuggestionController(IRecipeSuggestionService svc, IFreeTrialService trial) { _svc = svc; _trial = trial; }

    [HttpGet]
    public async Task<IActionResult> Suggest([FromQuery] string ingredient)
    {
        if (string.IsNullOrWhiteSpace(ingredient))
            return BadRequest(new { error = "Thiếu tên nguyên liệu" });

        if (!IsPremium)
        {
            var (allowed, newToken) = await _trial.ConsumeAsync(UserId);
            if (!allowed) return PremiumRequired();
            var result = await _svc.SuggestFromIngredientAsync(ingredient);
            return Ok(new { data = result, newToken, freeTrialUsed = FreeTrialUsed + 1, freeTrialLimit = FreeTrialService.Limit });
        }
        return Ok(new { data = await _svc.SuggestFromIngredientAsync(ingredient), newToken = (string?)null });
    }
}

// ═══════════════════════════════════════════════════════════════
// SEPAY WEBHOOK  (không cần auth — SePay gọi trực tiếp)
// Endpoint: POST /api/payment/webhook
// ═══════════════════════════════════════════════════════════════
[Route("api/payment")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly ISePayWebhookService _svc;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(ISePayWebhookService svc, ILogger<PaymentController> logger)
    {
        _svc    = svc;
        _logger = logger;
    }

    /// <summary>
    /// SePay gọi endpoint này khi có giao dịch mới.
    /// Cấu hình URL này trong dashboard SePay: https://yourdomain.com/api/payment/webhook
    /// </summary>
    [HttpPost("webhook")]
    public async Task<IActionResult> SePayWebhook(
        [FromBody] SePayWebhookDto payload,
        [FromHeader(Name = "X-SePay-Signature")] string? signature)
    {
        // Đọc raw body để verify signature
        Request.Body.Position = 0;
        using var reader = new System.IO.StreamReader(Request.Body);
        var rawBody = await reader.ReadToEndAsync();

        _logger.LogInformation("SePay webhook received: amount={Amount} content='{Content}'",
            payload.TransferAmount, payload.Content);

        var ok = await _svc.ProcessAsync(payload, signature, rawBody);

        // SePay yêu cầu trả về {"success": true} để xác nhận đã nhận
        return ok
            ? Ok(new { success = true })
            : BadRequest(new { success = false, error = "Invalid signature" });
    }
}
