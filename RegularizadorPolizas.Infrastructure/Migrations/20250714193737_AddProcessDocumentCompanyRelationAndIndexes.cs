using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegularizadorPolizas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProcessDocumentCompanyRelationAndIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoCompania",
                table: "ProcessDocuments",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "CompaniaId",
                table: "ProcessDocuments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CostoProcessamiento",
                table: "ProcessDocuments",
                type: "decimal(10,4)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EnviadoVelneo",
                table: "ProcessDocuments",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEnvioVelneo",
                table: "ProcessDocuments",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaFinProcesamiento",
                table: "ProcessDocuments",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaInicioProcesamiento",
                table: "ProcessDocuments",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HashArchivo",
                table: "ProcessDocuments",
                type: "varchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "IntentosProcesamiento",
                table: "ProcessDocuments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxIntentos",
                table: "ProcessDocuments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MensajeError",
                table: "ProcessDocuments",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "MetadatosJson",
                table: "ProcessDocuments",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "NivelConfianza",
                table: "ProcessDocuments",
                type: "decimal(5,4)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumeroPaginas",
                table: "ProcessDocuments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Prioridad",
                table: "ProcessDocuments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RespuestaVelneo",
                table: "ProcessDocuments",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<long>(
                name: "TamanoArchivo",
                table: "ProcessDocuments",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TiempoProcessamiento",
                table: "ProcessDocuments",
                type: "decimal(8,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoMime",
                table: "ProcessDocuments",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 21,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 22,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 23,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 24,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 25,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 26,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 27,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 28,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 29,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 30,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 31,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 32,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 33,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 34,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 35,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 36,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 37,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 38,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 5,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 6,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 7,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 8,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 9,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 10,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 11,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 12,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 13,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 14,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 15,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 16,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 17,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 18,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 19,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 20,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 21,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 22,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 23,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 24,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 25,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 26,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 27,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 28,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 29,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 30,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 31,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 32,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 33,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 34,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 35,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 36,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 37,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 38,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 39,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 40,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 41,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 42,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 43,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 44,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 45,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 46,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 47,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 48,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 49,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 50,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 51,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 52,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 53,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 54,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 55,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 56,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 57,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 58,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 59,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 60,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 61,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 62,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 63,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 64,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 65,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 66,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 67,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 68,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 69,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 70,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 71,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 72,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 73,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 74,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 75,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 76,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 77,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 78,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 79,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 80,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 81,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 82,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 83,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 84,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 85,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 86,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 87,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 88,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 89,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 90,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 91,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 92,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 93,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 94,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 95,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 96,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 97,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 98,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 99,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 100,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 101,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 102,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 103,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 104,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 105,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 106,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 107,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 108,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 109,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 110,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 111,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 112,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 113,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 114,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 115,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 116,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 117,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 118,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 119,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 120,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 121,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 122,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 123,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 124,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 125,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 126,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 127,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 128,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508), new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508), new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508), new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508), new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508), new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508), new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "AssignedAt",
                value: new DateTime(2025, 7, 14, 19, 37, 35, 784, DateTimeKind.Utc).AddTicks(508));

            migrationBuilder.CreateIndex(
                name: "IX_ProcessDocuments_CompaniaId",
                table: "ProcessDocuments",
                column: "CompaniaId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessDocuments_EstadoProcesamiento",
                table: "ProcessDocuments",
                column: "EstadoProcesamiento");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessDocuments_FechaCreacion",
                table: "ProcessDocuments",
                column: "FechaCreacion");

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessDocuments_Companies_CompaniaId",
                table: "ProcessDocuments",
                column: "CompaniaId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProcessDocuments_Companies_CompaniaId",
                table: "ProcessDocuments");

            migrationBuilder.DropIndex(
                name: "IX_ProcessDocuments_CompaniaId",
                table: "ProcessDocuments");

            migrationBuilder.DropIndex(
                name: "IX_ProcessDocuments_EstadoProcesamiento",
                table: "ProcessDocuments");

            migrationBuilder.DropIndex(
                name: "IX_ProcessDocuments_FechaCreacion",
                table: "ProcessDocuments");

            migrationBuilder.DropColumn(
                name: "CodigoCompania",
                table: "ProcessDocuments");

            migrationBuilder.DropColumn(
                name: "CompaniaId",
                table: "ProcessDocuments");

            migrationBuilder.DropColumn(
                name: "CostoProcessamiento",
                table: "ProcessDocuments");

            migrationBuilder.DropColumn(
                name: "EnviadoVelneo",
                table: "ProcessDocuments");

            migrationBuilder.DropColumn(
                name: "FechaEnvioVelneo",
                table: "ProcessDocuments");

            migrationBuilder.DropColumn(
                name: "FechaFinProcesamiento",
                table: "ProcessDocuments");

            migrationBuilder.DropColumn(
                name: "FechaInicioProcesamiento",
                table: "ProcessDocuments");

            migrationBuilder.DropColumn(
                name: "HashArchivo",
                table: "ProcessDocuments");

            migrationBuilder.DropColumn(
                name: "IntentosProcesamiento",
                table: "ProcessDocuments");

            migrationBuilder.DropColumn(
                name: "MaxIntentos",
                table: "ProcessDocuments");

            migrationBuilder.DropColumn(
                name: "MensajeError",
                table: "ProcessDocuments");

            migrationBuilder.DropColumn(
                name: "MetadatosJson",
                table: "ProcessDocuments");

            migrationBuilder.DropColumn(
                name: "NivelConfianza",
                table: "ProcessDocuments");

            migrationBuilder.DropColumn(
                name: "NumeroPaginas",
                table: "ProcessDocuments");

            migrationBuilder.DropColumn(
                name: "Prioridad",
                table: "ProcessDocuments");

            migrationBuilder.DropColumn(
                name: "RespuestaVelneo",
                table: "ProcessDocuments");

            migrationBuilder.DropColumn(
                name: "TamanoArchivo",
                table: "ProcessDocuments");

            migrationBuilder.DropColumn(
                name: "TiempoProcessamiento",
                table: "ProcessDocuments");

            migrationBuilder.DropColumn(
                name: "TipoMime",
                table: "ProcessDocuments");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 21,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 22,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 23,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 24,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 25,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 26,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 27,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 28,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 29,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 30,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 31,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 32,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 33,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 34,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 35,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 36,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 37,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 38,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 5,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 6,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 7,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 8,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 9,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 10,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 11,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 12,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 13,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 14,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 15,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 16,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 17,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 18,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 19,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 20,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 21,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 22,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 23,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 24,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 25,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 26,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 27,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 28,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 29,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 30,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 31,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 32,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 33,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 34,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 35,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 36,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 37,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 38,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 39,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 40,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 41,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 42,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 43,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 44,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 45,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 46,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 47,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 48,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 49,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 50,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 51,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 52,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 53,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 54,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 55,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 56,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 57,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 58,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 59,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 60,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 61,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 62,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 63,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 64,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 65,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 66,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 67,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 68,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 69,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 70,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 71,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 72,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 73,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 74,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 75,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 76,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 77,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 78,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 79,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 80,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 81,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 82,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 83,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 84,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 85,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 86,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 87,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 88,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 89,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 90,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 91,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 92,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 93,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 94,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 95,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 96,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 97,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 98,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 99,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 100,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 101,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 102,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 103,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 104,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 105,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 106,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 107,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 108,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 109,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 110,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 111,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 112,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 113,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 114,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 115,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 116,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 117,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 118,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 119,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 120,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 121,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 122,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 123,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 124,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 125,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 126,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 127,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 128,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399), new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399), new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399), new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399), new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399), new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399), new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "AssignedAt",
                value: new DateTime(2025, 7, 3, 19, 55, 24, 867, DateTimeKind.Utc).AddTicks(4399));
        }
    }
}
