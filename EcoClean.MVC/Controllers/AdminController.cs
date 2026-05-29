using EcoClean.MVC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EcoClean.MVC.Controllers;

public class AdminController : Controller
{
    private readonly ApiClient _api;

    public AdminController(ApiClient api) => _api = api;

    // Guard: chỉ cho role Admin
    private bool IsAdmin => Request.Cookies["ecoclean_role"] == "Admin"
                         && Request.Cookies.ContainsKey("ecoclean_token");

    private IActionResult RequireAdmin()
    {
        if (!Request.Cookies.ContainsKey("ecoclean_token"))
            return RedirectToAction("Login", "Account");
        if (Request.Cookies["ecoclean_role"] != "Admin")
            return RedirectToAction("Index", "Home");
        return null!;
    }

    // ── Dashboard ──────────────────────────────────────────────
    public async Task<IActionResult> Index()
    {
        var guard = RequireAdmin(); if (guard != null) return guard;
        ViewBag.Stats = await _api.GetAsync<JsonElement?>("admin/stats");
        return View();
    }

    // ── Users ──────────────────────────────────────────────────
    public async Task<IActionResult> Users(int page = 1)
    {
        var guard = RequireAdmin(); if (guard != null) return guard;
        ViewBag.Data = await _api.GetAsync<JsonElement?>($"admin/users?page={page}&size=20");
        ViewBag.Page = page;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> GrantPremium(int id, int months = 1)
    {
        var guard = RequireAdmin(); if (guard != null) return guard;
        await _api.PostAsync<JsonElement>($"admin/users/{id}/premium?months={months}", new { });
        return RedirectToAction("Users");
    }

    [HttpPost]
    public async Task<IActionResult> RevokePremium(int id)
    {
        var guard = RequireAdmin(); if (guard != null) return guard;
        await _api.DeleteAsync($"admin/users/{id}/premium");
        return RedirectToAction("Users");
    }

    public async Task<IActionResult> UserDetail(int id)
    {
        var guard = RequireAdmin(); if (guard != null) return guard;
        var user = await _api.GetAsync<JsonElement?>($"admin/users/{id}");
        if (user == null || user.Value.ValueKind == System.Text.Json.JsonValueKind.Null)
        {
            TempData["Error"] = $"Không tìm thấy người dùng ID={id}. Kiểm tra API logs.";
            return RedirectToAction("Users");
        }
        ViewBag.User = user;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var guard = RequireAdmin(); if (guard != null) return guard;
        await _api.DeleteAsync($"admin/users/{id}");
        TempData["Success"] = "Đã xóa người dùng.";
        return RedirectToAction("Users");
    }

    // ── Recipes ────────────────────────────────────────────────
    public async Task<IActionResult> Recipes()
    {
        var guard = RequireAdmin(); if (guard != null) return guard;
        ViewBag.Recipes = await _api.GetAsync<JsonElement?>("recipes");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateRecipe(string name, string? description, int calories,
        double protein, double carb, double fat, double fiber,
        string category, int? prepTimeMin, string? tags, string? imageUrl)
    {
        var guard = RequireAdmin(); if (guard != null) return guard;
        await _api.PostAsync<JsonElement>("admin/recipes", new
        {
            name, description, calories, protein, carb, fat, fiber,
            category, prepTimeMin, tags, imageUrl, isActive = true
        });
        return RedirectToAction("Recipes");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteRecipe(int id)
    {
        var guard = RequireAdmin(); if (guard != null) return guard;
        await _api.DeleteAsync($"admin/recipes/{id}");
        return RedirectToAction("Recipes");
    }

    [HttpPost]
    public async Task<IActionResult> UpdateRecipe(int id, string name, string? description, int calories,
        double protein, double carb, double fat, double fiber,
        string category, int? prepTimeMin, string? tags, string? imageUrl, bool isActive = true)
    {
        var guard = RequireAdmin(); if (guard != null) return guard;
        await _api.PutAsync<JsonElement>($"admin/recipes/{id}", new
        {
            name, description, calories, protein, carb, fat, fiber,
            category, prepTimeMin, tags, imageUrl, isActive
        });
        return RedirectToAction("Recipes");
    }

    // ── Subscriptions ──────────────────────────────────────────
    public async Task<IActionResult> Subscriptions(string? status)
    {
        var guard = RequireAdmin(); if (guard != null) return guard;
        var q = string.IsNullOrEmpty(status) ? "admin/subscriptions" : $"admin/subscriptions?status={status}";
        ViewBag.Subs   = await _api.GetAsync<JsonElement?>(q);
        ViewBag.Status = status;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmSubscription(int id)
    {
        var guard = RequireAdmin(); if (guard != null) return guard;
        await _api.PostAsync<JsonElement>($"admin/subscriptions/{id}/confirm", new { });
        return RedirectToAction("Subscriptions");
    }
}
