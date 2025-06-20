using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegularizadorPolizas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddModeToApiKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Mode",
                table: "ApiKeys",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 21,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 22,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 23,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 24,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 25,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 26,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 27,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 28,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 29,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 30,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 31,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 32,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 33,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 34,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 35,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 36,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 37,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 38,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 5,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 6,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 7,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 8,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 9,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 10,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 11,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 12,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 13,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 14,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 15,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 16,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 17,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 18,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 19,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 20,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 21,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 22,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 23,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 24,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 25,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 26,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 27,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 28,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 29,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 30,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 31,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 32,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 33,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 34,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 35,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 36,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 37,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 38,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 39,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 40,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 41,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 42,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 43,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 44,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 45,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 46,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 47,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 48,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 49,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 50,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 51,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 52,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 53,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 54,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 55,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 56,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 57,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 58,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 59,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 60,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 61,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 62,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 63,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 64,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 65,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 66,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 67,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 68,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 69,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 70,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 71,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 72,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 73,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 74,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 75,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 76,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 77,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 78,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 79,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 80,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 81,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 82,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 83,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 84,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 85,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 86,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 87,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 88,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 89,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 90,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 91,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 92,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 93,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 94,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 95,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 96,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 97,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 98,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 99,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 100,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 101,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 102,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 103,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 104,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 105,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 106,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 107,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 108,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 109,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 110,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 111,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 112,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 113,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 114,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 115,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 116,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 117,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 118,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 119,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 120,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 121,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 122,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 123,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 124,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 125,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 126,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 127,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 128,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087), new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087), new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087), new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087), new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087), new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087), new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "AssignedAt",
                value: new DateTime(2025, 6, 20, 17, 38, 12, 345, DateTimeKind.Utc).AddTicks(6087));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mode",
                table: "ApiKeys");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 21,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 22,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 23,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 24,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 25,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 26,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 27,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 28,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 29,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 30,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 31,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 32,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 33,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 34,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 35,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 36,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 37,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 38,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 5,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 6,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 7,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 8,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 9,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 10,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 11,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 12,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 13,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 14,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 15,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 16,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 17,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 18,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 19,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 20,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 21,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 22,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 23,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 24,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 25,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 26,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 27,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 28,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 29,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 30,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 31,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 32,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 33,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 34,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 35,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 36,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 37,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 38,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 39,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 40,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 41,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 42,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 43,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 44,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 45,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 46,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 47,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 48,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 49,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 50,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 51,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 52,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 53,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 54,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 55,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 56,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 57,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 58,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 59,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 60,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 61,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 62,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 63,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 64,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 65,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 66,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 67,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 68,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 69,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 70,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 71,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 72,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 73,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 74,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 75,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 76,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 77,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 78,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 79,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 80,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 81,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 82,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 83,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 84,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 85,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 86,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 87,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 88,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 89,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 90,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 91,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 92,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 93,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 94,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 95,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 96,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 97,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 98,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 99,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 100,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 101,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 102,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 103,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 104,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 105,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 106,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 107,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 108,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 109,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 110,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 111,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 112,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 113,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 114,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 115,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 116,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 117,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 118,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 119,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 120,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 121,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 122,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 123,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 124,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 125,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 126,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 127,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 128,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824), new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824), new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824), new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824), new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824), new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824), new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "AssignedAt",
                value: new DateTime(2025, 6, 19, 15, 59, 13, 394, DateTimeKind.Utc).AddTicks(1824));
        }
    }
}
