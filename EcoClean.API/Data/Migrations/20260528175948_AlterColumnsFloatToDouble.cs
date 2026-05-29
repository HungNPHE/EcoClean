using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoClean.API.Migrations
{
    /// <inheritdoc />
    public partial class AlterColumnsFloatToDouble : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Weight",
                table: "WeightLogs",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<double>(
                name: "Weight",
                table: "UserProfiles",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<double>(
                name: "TDEE",
                table: "UserProfiles",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<double>(
                name: "Height",
                table: "UserProfiles",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<double>(
                name: "BMI",
                table: "UserProfiles",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<double>(
                name: "Protein",
                table: "Recipes",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<double>(
                name: "Fiber",
                table: "Recipes",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<double>(
                name: "Fat",
                table: "Recipes",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<double>(
                name: "Carb",
                table: "Recipes",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<double>(
                name: "Protein",
                table: "FoodScans",
                type: "float",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Fat",
                table: "FoodScans",
                type: "float",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Confidence",
                table: "FoodScans",
                type: "float",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Carb",
                table: "FoodScans",
                type: "float",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Carb", "CreatedAt", "Fat", "Fiber", "Protein" },
                values: new object[] { 12.0, new DateTime(2026, 5, 28, 17, 59, 47, 812, DateTimeKind.Utc).AddTicks(7760), 14.0, 4.0, 35.0 });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Carb", "CreatedAt", "Fat", "Fiber", "Protein" },
                values: new object[] { 52.0, new DateTime(2026, 5, 28, 17, 59, 47, 812, DateTimeKind.Utc).AddTicks(7762), 5.0, 6.0, 8.0 });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Carb", "CreatedAt", "Fat", "Fiber", "Protein" },
                values: new object[] { 48.0, new DateTime(2026, 5, 28, 17, 59, 47, 812, DateTimeKind.Utc).AddTicks(7763), 12.0, 5.0, 42.0 });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Carb", "CreatedAt", "Fat", "Fiber", "Protein" },
                values: new object[] { 38.0, new DateTime(2026, 5, 28, 17, 59, 47, 812, DateTimeKind.Utc).AddTicks(7765), 20.0, 6.0, 12.0 });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Carb", "CreatedAt", "Fat", "Fiber", "Protein" },
                values: new object[] { 6.0, new DateTime(2026, 5, 28, 17, 59, 47, 812, DateTimeKind.Utc).AddTicks(7766), 13.0, 2.0, 18.0 });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Carb", "CreatedAt", "Fat", "Fiber", "Protein" },
                values: new object[] { 14.0, new DateTime(2026, 5, 28, 17, 59, 47, 812, DateTimeKind.Utc).AddTicks(7768), 6.0, 5.0, 40.0 });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Carb", "CreatedAt", "Fat", "Fiber", "Protein" },
                values: new object[] { 44.0, new DateTime(2026, 5, 28, 17, 59, 47, 812, DateTimeKind.Utc).AddTicks(7769), 8.0, 8.0, 14.0 });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Carb", "CreatedAt", "Fat", "Fiber", "Protein" },
                values: new object[] { 42.0, new DateTime(2026, 5, 28, 17, 59, 47, 812, DateTimeKind.Utc).AddTicks(7770), 10.0, 5.0, 38.0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "Weight",
                table: "WeightLogs",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<float>(
                name: "Weight",
                table: "UserProfiles",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<float>(
                name: "TDEE",
                table: "UserProfiles",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<float>(
                name: "Height",
                table: "UserProfiles",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<float>(
                name: "BMI",
                table: "UserProfiles",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<float>(
                name: "Protein",
                table: "Recipes",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<float>(
                name: "Fiber",
                table: "Recipes",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<float>(
                name: "Fat",
                table: "Recipes",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<float>(
                name: "Carb",
                table: "Recipes",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<float>(
                name: "Protein",
                table: "FoodScans",
                type: "real",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Fat",
                table: "FoodScans",
                type: "real",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Confidence",
                table: "FoodScans",
                type: "real",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Carb",
                table: "FoodScans",
                type: "real",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Carb", "CreatedAt", "Fat", "Fiber", "Protein" },
                values: new object[] { 12f, new DateTime(2026, 5, 28, 12, 9, 30, 76, DateTimeKind.Utc).AddTicks(6062), 14f, 4f, 35f });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Carb", "CreatedAt", "Fat", "Fiber", "Protein" },
                values: new object[] { 52f, new DateTime(2026, 5, 28, 12, 9, 30, 76, DateTimeKind.Utc).AddTicks(6065), 5f, 6f, 8f });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Carb", "CreatedAt", "Fat", "Fiber", "Protein" },
                values: new object[] { 48f, new DateTime(2026, 5, 28, 12, 9, 30, 76, DateTimeKind.Utc).AddTicks(6067), 12f, 5f, 42f });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Carb", "CreatedAt", "Fat", "Fiber", "Protein" },
                values: new object[] { 38f, new DateTime(2026, 5, 28, 12, 9, 30, 76, DateTimeKind.Utc).AddTicks(6068), 20f, 6f, 12f });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Carb", "CreatedAt", "Fat", "Fiber", "Protein" },
                values: new object[] { 6f, new DateTime(2026, 5, 28, 12, 9, 30, 76, DateTimeKind.Utc).AddTicks(6069), 13f, 2f, 18f });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Carb", "CreatedAt", "Fat", "Fiber", "Protein" },
                values: new object[] { 14f, new DateTime(2026, 5, 28, 12, 9, 30, 76, DateTimeKind.Utc).AddTicks(6071), 6f, 5f, 40f });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Carb", "CreatedAt", "Fat", "Fiber", "Protein" },
                values: new object[] { 44f, new DateTime(2026, 5, 28, 12, 9, 30, 76, DateTimeKind.Utc).AddTicks(6072), 8f, 8f, 14f });

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Carb", "CreatedAt", "Fat", "Fiber", "Protein" },
                values: new object[] { 42f, new DateTime(2026, 5, 28, 12, 9, 30, 76, DateTimeKind.Utc).AddTicks(6074), 10f, 5f, 38f });
        }
    }
}
