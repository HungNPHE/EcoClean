using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoClean.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFreeTrialUsed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FreeTrialUsed",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 14, 18, 3, 15, 349, DateTimeKind.Utc).AddTicks(1410));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 14, 18, 3, 15, 349, DateTimeKind.Utc).AddTicks(1416));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 14, 18, 3, 15, 349, DateTimeKind.Utc).AddTicks(1418));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 14, 18, 3, 15, 349, DateTimeKind.Utc).AddTicks(1420));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 14, 18, 3, 15, 349, DateTimeKind.Utc).AddTicks(1422));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 14, 18, 3, 15, 349, DateTimeKind.Utc).AddTicks(1423));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 14, 18, 3, 15, 349, DateTimeKind.Utc).AddTicks(1425));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 14, 18, 3, 15, 349, DateTimeKind.Utc).AddTicks(1428));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 14, 18, 3, 15, 349, DateTimeKind.Utc).AddTicks(1430));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 14, 18, 3, 15, 349, DateTimeKind.Utc).AddTicks(1432));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 14, 18, 3, 15, 349, DateTimeKind.Utc).AddTicks(1434));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 14, 18, 3, 15, 349, DateTimeKind.Utc).AddTicks(1435));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 14, 18, 3, 15, 349, DateTimeKind.Utc).AddTicks(1437));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 14, 18, 3, 15, 349, DateTimeKind.Utc).AddTicks(1439));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 14, 18, 3, 15, 349, DateTimeKind.Utc).AddTicks(1441));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 14, 18, 3, 15, 349, DateTimeKind.Utc).AddTicks(1443));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FreeTrialUsed",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 14, 12, 59, 22, 119, DateTimeKind.Utc).AddTicks(1106));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 14, 12, 59, 22, 119, DateTimeKind.Utc).AddTicks(1109));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 14, 12, 59, 22, 119, DateTimeKind.Utc).AddTicks(1111));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 14, 12, 59, 22, 119, DateTimeKind.Utc).AddTicks(1113));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 14, 12, 59, 22, 119, DateTimeKind.Utc).AddTicks(1115));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 14, 12, 59, 22, 119, DateTimeKind.Utc).AddTicks(1117));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 14, 12, 59, 22, 119, DateTimeKind.Utc).AddTicks(1119));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 14, 12, 59, 22, 119, DateTimeKind.Utc).AddTicks(1120));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 14, 12, 59, 22, 119, DateTimeKind.Utc).AddTicks(1122));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 14, 12, 59, 22, 119, DateTimeKind.Utc).AddTicks(1124));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 14, 12, 59, 22, 119, DateTimeKind.Utc).AddTicks(1126));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 14, 12, 59, 22, 119, DateTimeKind.Utc).AddTicks(1128));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 14, 12, 59, 22, 119, DateTimeKind.Utc).AddTicks(1165));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 14, 12, 59, 22, 119, DateTimeKind.Utc).AddTicks(1167));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 14, 12, 59, 22, 119, DateTimeKind.Utc).AddTicks(1169));

            migrationBuilder.UpdateData(
                table: "Recipes",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 14, 12, 59, 22, 119, DateTimeKind.Utc).AddTicks(1171));
        }
    }
}
