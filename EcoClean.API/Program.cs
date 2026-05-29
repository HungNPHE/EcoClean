using EcoClean.API.Data;
using EcoClean.API.Helpers;
using EcoClean.API.Middleware;
using EcoClean.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ── Database ──────────────────────────────────────────────────
builder.Services.AddDbContext<EcoCleanDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── JWT Auth ──────────────────────────────────────────────────
var jwtSecret = builder.Configuration["JwtSettings:Secret"]
    ?? throw new InvalidOperationException("JWT Secret not configured");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer   = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();

// ── Services ──────────────────────────────────────────────────
builder.Services.AddScoped<JwtHelper>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<IMealPlanService, MealPlanService>();
builder.Services.AddScoped<IWeightService, WeightService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IFoodScanService, FoodScanService>();
builder.Services.AddScoped<IChatbotService, ChatbotService>();
builder.Services.AddScoped<IAIMealPlanService, AIMealPlanService>();
builder.Services.AddHttpClient<IGeminiService, GeminiService>();

// ── CORS ──────────────────────────────────────────────────────
builder.Services.AddCors(opt => opt.AddPolicy("AllowMVC", p =>
    p.WithOrigins(
        builder.Configuration["MvcBaseUrl"] ?? "http://localhost:5001",
        "http://localhost:5001",
        "https://localhost:7001"
    )
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()));

// ── Swagger ───────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "EcoClean API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization", Type = SecuritySchemeType.Http,
        Scheme = "bearer", BearerFormat = "JWT", In = ParameterLocation.Header
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [new OpenApiSecurityScheme { Reference = new() { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }] = []
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

var app = builder.Build();

// ── Migrate & Seed ────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db  = scope.ServiceProvider.GetRequiredService<EcoCleanDbContext>();
    var cfg = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    await db.Database.MigrateAsync();

    // Seed admin account from config (safe to run on every startup)
    var adminEmail = cfg["AdminSeed:Email"];
    var adminPass  = cfg["AdminSeed:Password"];
    var adminName  = cfg["AdminSeed:FullName"] ?? "System Admin";

    if (!string.IsNullOrEmpty(adminEmail) && !string.IsNullOrEmpty(adminPass))
    {
        var existing = await db.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);
        if (existing == null)
        {
            db.Users.Add(new EcoClean.API.Models.User
            {
                Email        = adminEmail,
                FullName     = adminName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPass),
                Role         = "Admin",
                IsPremium    = true
            });
            await db.SaveChangesAsync();
        }
        else if (existing.Role != "Admin")
        {
            existing.Role = "Admin";
            await db.SaveChangesAsync();
        }
    }
}

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowMVC");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
