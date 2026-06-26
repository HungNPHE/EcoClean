using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EcoClean.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Recipes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Calories = table.Column<int>(type: "int", nullable: false),
                    Protein = table.Column<double>(type: "float", nullable: false),
                    Carb = table.Column<double>(type: "float", nullable: false),
                    Fat = table.Column<double>(type: "float", nullable: false),
                    Fiber = table.Column<double>(type: "float", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrepTimeMin = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsPremium = table.Column<bool>(type: "bit", nullable: false),
                    PremiumExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AIMealPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Goal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetCalories = table.Column<int>(type: "int", nullable: false),
                    DurationDays = table.Column<int>(type: "int", nullable: false),
                    PlanJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIMealPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AIMealPlans_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SessionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatHistory_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FoodScans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FoodName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Calories = table.Column<int>(type: "int", nullable: true),
                    Protein = table.Column<double>(type: "float", nullable: true),
                    Carb = table.Column<double>(type: "float", nullable: true),
                    Fat = table.Column<double>(type: "float", nullable: true),
                    Confidence = table.Column<double>(type: "float", nullable: true),
                    RawAIResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScannedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodScans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodScans_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MealPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RecipeId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MealType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlanDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Calories = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealPlans_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MealPlans_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Plan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QRCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Height = table.Column<double>(type: "float", nullable: false),
                    Weight = table.Column<double>(type: "float", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActivityLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BMI = table.Column<double>(type: "float", nullable: false),
                    BMICategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TDEE = table.Column<double>(type: "float", nullable: false),
                    DailyCalorieGoal = table.Column<int>(type: "int", nullable: false),
                    Goal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeightLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<double>(type: "float", nullable: false),
                    LogDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeightLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeightLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Recipes",
                columns: new[] { "Id", "Calories", "Carb", "Category", "CreatedAt", "Description", "Fat", "Fiber", "ImageUrl", "IsActive", "Name", "PrepTimeMin", "Protein", "Tags" },
                values: new object[,]
                {
                    { 1, 320, 12.0, "Lunch", new DateTime(2026, 6, 12, 1, 46, 16, 84, DateTimeKind.Utc).AddTicks(1293), null, 14.0, 4.0, null, true, "Salad ức gà nướng", null, 35.0, "[\"high-protein\",\"low-carb\"]" },
                    { 2, 280, 52.0, "Breakfast", new DateTime(2026, 6, 12, 1, 46, 16, 84, DateTimeKind.Utc).AddTicks(1295), null, 5.0, 6.0, null, true, "Yến mạch trái cây", null, 8.0, "[\"fiber\",\"energy\"]" },
                    { 3, 450, 48.0, "Lunch", new DateTime(2026, 6, 12, 1, 46, 16, 84, DateTimeKind.Utc).AddTicks(1297), null, 12.0, 5.0, null, true, "Cơm gạo lứt cá hồi", null, 42.0, "[\"omega3\",\"balanced\"]" },
                    { 4, 380, 38.0, "Snack", new DateTime(2026, 6, 12, 1, 46, 16, 84, DateTimeKind.Utc).AddTicks(1298), null, 20.0, 6.0, null, true, "Smoothie bơ chuối", null, 12.0, "[\"smoothie\",\"healthy-fat\"]" },
                    { 5, 210, 6.0, "Breakfast", new DateTime(2026, 6, 12, 1, 46, 16, 84, DateTimeKind.Utc).AddTicks(1299), null, 13.0, 2.0, null, true, "Trứng bác rau cải", null, 18.0, "[\"low-carb\",\"quick\"]" },
                    { 6, 290, 14.0, "Dinner", new DateTime(2026, 6, 12, 1, 46, 16, 84, DateTimeKind.Utc).AddTicks(1301), null, 6.0, 5.0, null, true, "Gà luộc rau hấp", null, 40.0, "[\"lean\",\"clean\"]" },
                    { 7, 310, 44.0, "Breakfast", new DateTime(2026, 6, 12, 1, 46, 16, 84, DateTimeKind.Utc).AddTicks(1302), null, 8.0, 8.0, null, true, "Overnight oats", null, 14.0, "[\"prep-ahead\",\"fiber\"]" },
                    { 8, 400, 42.0, "Dinner", new DateTime(2026, 6, 12, 1, 46, 16, 84, DateTimeKind.Utc).AddTicks(1303), null, 10.0, 5.0, null, true, "Bowl cá ngừ quinoa", null, 38.0, "[\"complete-protein\",\"grain\"]" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AIMealPlans_UserId",
                table: "AIMealPlans",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatHistory_UserId_SessionId",
                table: "ChatHistory",
                columns: new[] { "UserId", "SessionId" });

            migrationBuilder.CreateIndex(
                name: "IX_FoodScans_UserId",
                table: "FoodScans",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MealPlans_RecipeId",
                table: "MealPlans",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_MealPlans_UserId_PlanDate",
                table: "MealPlans",
                columns: new[] { "UserId", "PlanDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_UserId",
                table: "Subscriptions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_UserId",
                table: "UserProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WeightLogs_UserId_LogDate",
                table: "WeightLogs",
                columns: new[] { "UserId", "LogDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AIMealPlans");

            migrationBuilder.DropTable(
                name: "ChatHistory");

            migrationBuilder.DropTable(
                name: "FoodScans");

            migrationBuilder.DropTable(
                name: "MealPlans");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "UserProfiles");

            migrationBuilder.DropTable(
                name: "WeightLogs");

            migrationBuilder.DropTable(
                name: "Recipes");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
