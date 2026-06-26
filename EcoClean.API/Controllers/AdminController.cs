using EcoClean.API.Data;
using EcoClean.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoClean.API.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/admin")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly EcoCleanDbContext _db;
    public AdminController(EcoCleanDbContext db) => _db = db;

    // ── Dashboard stats ────────────────────────────────────────
    [HttpGet("stats")]
    public async Task<IActionResult> Stats()
    {
        var now = DateTime.UtcNow;
        return Ok(new
        {
            TotalUsers    = await _db.Users.CountAsync(),
            PremiumUsers  = await _db.Users.CountAsync(u => u.IsPremium),
            TotalScans    = await _db.FoodScans.CountAsync(),
            TotalChats    = await _db.ChatHistories.CountAsync(),
            Revenue       = await _db.Subscriptions.Where(s => s.Status == "Paid").SumAsync(s => s.Amount),
            NewUsersToday = await _db.Users.CountAsync(u => u.CreatedAt.Date == now.Date),
            ActivePlans   = await _db.Subscriptions.CountAsync(s => s.Status == "Paid" && s.ExpiresAt > now)
        });
    }

    // ── Users list ─────────────────────────────────────────────
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int size = 20)
    {
        var users = await _db.Users
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * size).Take(size)
            .Select(u => new
            {
                u.Id, u.Email, u.FullName, u.Role,
                u.IsPremium, u.PremiumExpiry, u.CreatedAt
            })
            .ToListAsync();

        var total = await _db.Users.CountAsync();
        return Ok(new { total, page, size, users });
    }

    // ── User detail + profile ──────────────────────────────────
    [HttpGet("users/{id:int}")]
    public async Task<IActionResult> GetUserDetail(int id)
    {
        var user = await _db.Users
            .Include(u => u.Profile)
            .Include(u => u.MealPlans)
            .Include(u => u.WeightLogs)
            .Include(u => u.FoodScans)
            .Include(u => u.ChatHistories)
            .Include(u => u.Subscriptions)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) return NotFound(new { error = $"User {id} not found" });

        return Ok(new
        {
            user.Id, user.Email, user.FullName, user.Role,
            user.IsPremium, user.PremiumExpiry, user.CreatedAt,
            Profile = user.Profile == null ? null : new
            {
                user.Profile.Height, user.Profile.Weight, user.Profile.Age,
                user.Profile.Gender, user.Profile.ActivityLevel, user.Profile.Goal,
                user.Profile.BMI, user.Profile.BMICategory,
                user.Profile.TDEE, user.Profile.DailyCalorieGoal, user.Profile.UpdatedAt
            },
            MealPlanCount  = user.MealPlans.Count,
            WeightLogCount = user.WeightLogs.Count,
            ScanCount      = user.FoodScans.Count,
            ChatCount      = user.ChatHistories.Count,
            Subscriptions  = user.Subscriptions
                .OrderByDescending(s => s.CreatedAt)
                .Take(5)
                .Select(s => new { s.Plan, s.Amount, s.Status, s.PaidAt, s.ExpiresAt })
        });
    }

    // ── Delete user ────────────────────────────────────────────
    [HttpDelete("users/{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();
        if (user.Role == "Admin") return BadRequest(new { error = "Cannot delete admin account" });
        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // ── Grant / Revoke premium manually ───────────────────────
    [HttpPost("users/{id:int}/premium")]
    public async Task<IActionResult> GrantPremium(int id, [FromQuery] int months = 1)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();
        user.IsPremium     = true;
        user.PremiumExpiry = DateTime.UtcNow.AddMonths(months);
        user.Role          = "Premium";
        await _db.SaveChangesAsync();
        return Ok(new { message = $"Granted {months} month(s) premium to {user.Email}" });
    }

    [HttpDelete("users/{id:int}/premium")]
    public async Task<IActionResult> RevokePremium(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();
        user.IsPremium     = false;
        user.PremiumExpiry = null;
        user.Role          = "User";
        await _db.SaveChangesAsync();
        return Ok(new { message = "Premium revoked" });
    }

    // ── Recipes management ─────────────────────────────────────
    [HttpPost("recipes")]
    public async Task<IActionResult> CreateRecipe([FromBody] Recipe recipe)
    {
        recipe.CreatedAt = DateTime.UtcNow;
        _db.Recipes.Add(recipe);
        await _db.SaveChangesAsync();
        return Ok(recipe);
    }

    [HttpPut("recipes/{id}")]
    public async Task<IActionResult> UpdateRecipe(int id, [FromBody] Recipe updated)
    {
        var recipe = await _db.Recipes.FindAsync(id);
        if (recipe == null) return NotFound();
        recipe.Name       = updated.Name;
        recipe.Description = updated.Description;
        recipe.Calories   = updated.Calories;
        recipe.Protein    = updated.Protein;
        recipe.Carb       = updated.Carb;
        recipe.Fat        = updated.Fat;
        recipe.Fiber      = updated.Fiber;
        recipe.Category   = updated.Category;
        recipe.Tags       = updated.Tags;
        recipe.ImageUrl   = updated.ImageUrl;
        recipe.PrepTimeMin = updated.PrepTimeMin;
        recipe.Ingredients  = updated.Ingredients;
        recipe.Instructions = updated.Instructions;
        recipe.IsActive   = updated.IsActive;
        await _db.SaveChangesAsync();
        return Ok(recipe);
    }

    [HttpDelete("recipes/{id}")]
    public async Task<IActionResult> DeleteRecipe(int id)
    {
        var recipe = await _db.Recipes.FindAsync(id);
        if (recipe == null) return NotFound();
        recipe.IsActive = false; // Soft delete
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // ── Subscriptions ──────────────────────────────────────────
    [HttpGet("subscriptions")]
    public async Task<IActionResult> GetSubscriptions([FromQuery] string? status)
    {
        var q = _db.Subscriptions.Include(s => s.User).AsQueryable();
        if (!string.IsNullOrEmpty(status)) q = q.Where(s => s.Status == status);
        var subs = await q.OrderByDescending(s => s.CreatedAt)
            .Select(s => new
            {
                s.Id, s.Plan, s.Amount, s.Status,
                s.TransactionRef, s.PaidAt, s.ExpiresAt,
                User = new { s.User.Id, s.User.Email, s.User.FullName }
            })
            .Take(100)
            .ToListAsync();
        return Ok(subs);
    }

    // Manually confirm a pending payment
    [HttpPost("subscriptions/{id}/confirm")]
    public async Task<IActionResult> AdminConfirm(int id)
    {
        var sub = await _db.Subscriptions.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == id);
        if (sub == null) return NotFound();

        sub.Status = "Paid";
        sub.PaidAt = DateTime.UtcNow;
        sub.ExpiresAt = DateTime.UtcNow.AddMonths(sub.Plan == "Yearly" ? 12 : 1);

        if (sub.User != null)
        {
            sub.User.IsPremium = true;
            sub.User.PremiumExpiry = sub.ExpiresAt;
            sub.User.Role = "Premium";
        }

        await _db.SaveChangesAsync();
        return Ok(new { message = "Payment confirmed", sub.ExpiresAt });
    }
}
