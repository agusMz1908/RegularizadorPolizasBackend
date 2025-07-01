using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegularizadorPolizas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyAuxiliaryEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coberturas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Cobdsc = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coberturas", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ComisionesPorSeccion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Compania = table.Column<int>(type: "int", nullable: false),
                    Seccion = table.Column<int>(type: "int", nullable: false),
                    Comipor = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Comi_bo = table.Column<decimal>(type: "decimal(5,2)", nullable: false, defaultValue: 0m),
                    Opera_comi = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, defaultValue: "N")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Detalle = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Tipos_de_contrato = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Productos_de_vida = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Anios_d = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Anios_h = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComisionesPorSeccion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComisionesPorSeccion_Companies_Compania",
                        column: x => x.Compania,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComisionesPorSeccion_Secciones_Seccion",
                        column: x => x.Seccion,
                        principalTable: "Secciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ContactosCompania",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Companias = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Telefono = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Mail = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactosCompania", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContactosCompania_Companies_Companias",
                        column: x => x.Companias,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ImpuestosPorSeccion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Companias = table.Column<int>(type: "int", nullable: false),
                    Impuestos = table.Column<int>(type: "int", nullable: false),
                    Secciones = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImpuestosPorSeccion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImpuestosPorSeccion_Companies_Companias",
                        column: x => x.Companias,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImpuestosPorSeccion_Secciones_Secciones",
                        column: x => x.Secciones,
                        principalTable: "Secciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "NumerosUtiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Companias = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Telefono = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumerosUtiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NumerosUtiles_Companies_Companias",
                        column: x => x.Companias,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 21,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 22,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 23,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 24,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 25,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 26,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 27,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 28,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 29,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 30,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 31,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 32,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 33,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 34,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 35,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 36,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 37,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 38,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 5,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 6,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 7,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 8,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 9,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 10,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 11,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 12,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 13,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 14,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 15,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 16,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 17,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 18,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 19,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 20,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 21,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 22,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 23,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 24,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 25,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 26,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 27,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 28,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 29,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 30,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 31,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 32,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 33,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 34,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 35,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 36,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 37,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 38,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 39,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 40,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 41,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 42,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 43,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 44,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 45,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 46,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 47,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 48,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 49,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 50,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 51,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 52,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 53,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 54,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 55,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 56,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 57,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 58,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 59,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 60,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 61,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 62,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 63,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 64,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 65,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 66,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 67,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 68,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 69,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 70,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 71,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 72,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 73,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 74,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 75,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 76,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 77,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 78,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 79,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 80,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 81,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 82,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 83,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 84,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 85,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 86,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 87,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 88,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 89,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 90,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 91,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 92,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 93,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 94,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 95,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 96,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 97,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 98,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 99,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 100,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 101,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 102,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 103,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 104,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 105,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 106,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 107,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 108,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 109,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 110,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 111,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 112,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 113,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 114,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 115,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 116,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 117,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 118,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 119,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 120,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 121,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 122,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 123,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 124,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 125,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 126,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 127,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 128,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733), new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733), new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733), new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733), new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733), new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733), new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "AssignedAt",
                value: new DateTime(2025, 6, 30, 22, 49, 55, 632, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.CreateIndex(
                name: "IX_Coberturas_Activo",
                table: "Coberturas",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_Coberturas_Cobdsc",
                table: "Coberturas",
                column: "Cobdsc",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComisionesPorSeccion_Compania",
                table: "ComisionesPorSeccion",
                column: "Compania");

            migrationBuilder.CreateIndex(
                name: "IX_ComisionesPorSeccion_Compania_Seccion",
                table: "ComisionesPorSeccion",
                columns: new[] { "Compania", "Seccion" });

            migrationBuilder.CreateIndex(
                name: "IX_ComisionesPorSeccion_Seccion",
                table: "ComisionesPorSeccion",
                column: "Seccion");

            migrationBuilder.CreateIndex(
                name: "IX_ContactosCompania_Companias",
                table: "ContactosCompania",
                column: "Companias");

            migrationBuilder.CreateIndex(
                name: "IX_ContactosCompania_Mail",
                table: "ContactosCompania",
                column: "Mail");

            migrationBuilder.CreateIndex(
                name: "IX_ContactosCompania_Name",
                table: "ContactosCompania",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ImpuestosPorSeccion_Companias",
                table: "ImpuestosPorSeccion",
                column: "Companias");

            migrationBuilder.CreateIndex(
                name: "IX_ImpuestosPorSeccion_Companias_Secciones_Impuestos",
                table: "ImpuestosPorSeccion",
                columns: new[] { "Companias", "Secciones", "Impuestos" });

            migrationBuilder.CreateIndex(
                name: "IX_ImpuestosPorSeccion_Impuestos",
                table: "ImpuestosPorSeccion",
                column: "Impuestos");

            migrationBuilder.CreateIndex(
                name: "IX_ImpuestosPorSeccion_Secciones",
                table: "ImpuestosPorSeccion",
                column: "Secciones");

            migrationBuilder.CreateIndex(
                name: "IX_NumerosUtiles_Companias",
                table: "NumerosUtiles",
                column: "Companias");

            migrationBuilder.CreateIndex(
                name: "IX_NumerosUtiles_Name",
                table: "NumerosUtiles",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_NumerosUtiles_Telefono",
                table: "NumerosUtiles",
                column: "Telefono");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coberturas");

            migrationBuilder.DropTable(
                name: "ComisionesPorSeccion");

            migrationBuilder.DropTable(
                name: "ContactosCompania");

            migrationBuilder.DropTable(
                name: "ImpuestosPorSeccion");

            migrationBuilder.DropTable(
                name: "NumerosUtiles");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 21,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 22,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 23,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 24,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 25,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 26,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 27,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 28,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 29,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 30,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 31,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 32,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 33,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 34,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 35,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 36,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 37,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 38,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 5,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 6,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 7,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 8,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 9,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 10,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 11,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 12,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 13,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 14,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 15,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 16,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 17,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 18,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 19,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 20,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 21,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 22,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 23,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 24,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 25,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 26,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 27,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 28,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 29,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 30,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 31,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 32,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 33,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 34,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 35,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 36,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 37,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 38,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 39,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 40,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 41,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 42,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 43,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 44,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 45,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 46,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 47,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 48,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 49,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 50,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 51,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 52,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 53,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 54,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 55,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 56,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 57,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 58,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 59,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 60,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 61,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 62,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 63,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 64,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 65,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 66,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 67,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 68,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 69,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 70,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 71,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 72,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 73,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 74,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 75,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 76,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 77,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 78,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 79,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 80,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 81,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 82,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 83,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 84,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 85,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 86,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 87,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 88,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 89,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 90,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 91,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 92,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 93,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 94,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 95,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 96,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 97,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 98,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 99,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 100,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 101,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 102,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 103,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 104,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 105,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 106,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 107,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 108,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 109,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 110,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 111,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 112,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 113,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 114,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 115,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 116,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 117,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 118,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 119,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 120,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 121,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 122,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 123,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 124,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 125,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 126,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 127,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 128,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364), new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364), new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364), new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364), new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364), new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364), new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "AssignedAt",
                value: new DateTime(2025, 6, 30, 21, 43, 10, 861, DateTimeKind.Utc).AddTicks(7364));
        }
    }
}
