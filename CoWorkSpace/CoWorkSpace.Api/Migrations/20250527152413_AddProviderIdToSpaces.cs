using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoWorkSpace.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddProviderIdToSpaces : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProviderId",
                table: "Spaces",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Logs",
                keyColumn: "Id",
                keyValue: 1,
                column: "Timestamp",
                value: new DateTime(2025, 5, 19, 23, 59, 59, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 1,
                column: "Status",
                value: "CREADO");

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Amount", "Status" },
                values: new object[] { 50.00m, "CREADO" });

            migrationBuilder.UpdateData(
                table: "Spaces",
                keyColumn: "Id",
                keyValue: 1,
                column: "ProviderId",
                value: 2);

            migrationBuilder.CreateIndex(
                name: "IX_Spaces_ProviderId",
                table: "Spaces",
                column: "ProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Spaces_Users_ProviderId",
                table: "Spaces",
                column: "ProviderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Spaces_Users_ProviderId",
                table: "Spaces");

            migrationBuilder.DropIndex(
                name: "IX_Spaces_ProviderId",
                table: "Spaces");

            migrationBuilder.DropColumn(
                name: "ProviderId",
                table: "Spaces");

            migrationBuilder.UpdateData(
                table: "Logs",
                keyColumn: "Id",
                keyValue: 1,
                column: "Timestamp",
                value: new DateTime(2025, 5, 19, 20, 23, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 1,
                column: "Status",
                value: "Completed");

            migrationBuilder.UpdateData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Amount", "Status" },
                values: new object[] { 500.00m, "Completed" });
        }
    }
}
