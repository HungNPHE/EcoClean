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
    protected int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    protected bool IsPremium => bool.Parse(User.FindFirstValue("IsPremium") ?? "false");

    protected IActionResult PremiumRequired()
        => Forbid("Premium subscription required");
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
    public FoodScanController(IFoodScanService svc) => _svc = svc;

    [HttpPost]
    public async Task<IActionResult> Scan([FromBody] string base64Image)
    {
        if (!IsPremium) return PremiumRequired();
        return Ok(await _svc.ScanAsync(UserId, base64Image));
    }

    [HttpGet("history")]
    public async Task<IActionResult> History([FromQuery] int page = 1)
    {
        if (!IsPremium) return PremiumRequired();
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
    public ChatController(IChatbotService svc) => _svc = svc;

    [HttpPost]
    public async Task<IActionResult> Chat([FromBody] ChatMessageDto dto)
    {
        if (!IsPremium) return PremiumRequired();
        return Ok(await _svc.ChatAsync(UserId, dto));
    }

    [HttpGet("session/{sessionId}")]
    public async Task<IActionResult> GetSession(string sessionId)
    {
        if (!IsPremium) return PremiumRequired();
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
    public AIMealPlanController(IAIMealPlanService svc) => _svc = svc;

    [HttpPost]
    public async Task<IActionResult> Generate([FromBody] AIMealPlanRequestDto dto)
    {
        if (!IsPremium) return PremiumRequired();
        return Ok(await _svc.GenerateAsync(UserId, dto));
    }

    [HttpGet("history")]
    public async Task<IActionResult> History()
    {
        if (!IsPremium) return PremiumRequired();
        return Ok(await _svc.GetHistoryAsync(UserId));
    }
}
