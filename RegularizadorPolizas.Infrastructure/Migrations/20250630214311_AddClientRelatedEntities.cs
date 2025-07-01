using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegularizadorPolizas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClientRelatedEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaModificacion",
                table: "Clients",
                type: "datetime(6)",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "Clients",
                type: "datetime(6)",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "Clients",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.CreateTable(
                name: "AutorizacionesCliente",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Clientes = table.Column<int>(type: "int", nullable: false),
                    AutorizacionesDeDatos = table.Column<int>(type: "int", nullable: false),
                    Autorizado = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    Fecha = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Observaciones = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutorizacionesCliente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AutorizacionesCliente_AutorizacionesDatos_AutorizacionesDeDa~",
                        column: x => x.AutorizacionesDeDatos,
                        principalTable: "AutorizacionesDatos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AutorizacionesCliente_Clients_Clientes",
                        column: x => x.Clientes,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CategoriasCliente",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ValMin = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    ValMax = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    Color = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasCliente", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Contactos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Clientes = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CargoRelacion = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Cel = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Domicilio = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Mail = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Obs = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contactos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contactos_Clients_Clientes",
                        column: x => x.Clientes,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CuentasBancarias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Clientes = table.Column<int>(type: "int", nullable: false),
                    Titular = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Tipo = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MonedaCuenta = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Sucursal = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Numero = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Subcuenta = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentasBancarias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CuentasBancarias_Clients_Clientes",
                        column: x => x.Clientes,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GruposEconomicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GruposEconomicos", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tarjetas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Clientes = table.Column<int>(type: "int", nullable: false),
                    Emisor = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Titular = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Vencimiento = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Numero = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Dato = table.Column<int>(type: "int", nullable: false),
                    Con = table.Column<int>(type: "int", nullable: false),
                    Control = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarjetas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tarjetas_Clients_Clientes",
                        column: x => x.Clientes,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Activo",
                table: "Clients",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Activo_FechaCreacion",
                table: "Clients",
                columns: new[] { "Activo", "FechaCreacion" });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Categorias_de_cliente",
                table: "Clients",
                column: "Categorias_de_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Corredor",
                table: "Clients",
                columns: new[] { "Corrcod", "Subcorr" });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Grupos_economicos",
                table: "Clients",
                column: "Grupos_economicos");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Nombre",
                table: "Clients",
                column: "Clinom");

            migrationBuilder.CreateIndex(
                name: "IX_AutorizacionesCliente_AutorizacionesDeDatos",
                table: "AutorizacionesCliente",
                column: "AutorizacionesDeDatos");

            migrationBuilder.CreateIndex(
                name: "IX_AutorizacionesCliente_Autorizado",
                table: "AutorizacionesCliente",
                column: "Autorizado");

            migrationBuilder.CreateIndex(
                name: "IX_AutorizacionesCliente_Clientes",
                table: "AutorizacionesCliente",
                column: "Clientes");

            migrationBuilder.CreateIndex(
                name: "IX_AutorizacionesCliente_Fecha",
                table: "AutorizacionesCliente",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_CategoriasCliente_Activo",
                table: "CategoriasCliente",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_CategoriasCliente_Name",
                table: "CategoriasCliente",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contactos_Cel",
                table: "Contactos",
                column: "Cel");

            migrationBuilder.CreateIndex(
                name: "IX_Contactos_Clientes",
                table: "Contactos",
                column: "Clientes");

            migrationBuilder.CreateIndex(
                name: "IX_Contactos_Mail",
                table: "Contactos",
                column: "Mail");

            migrationBuilder.CreateIndex(
                name: "IX_Contactos_Nombre",
                table: "Contactos",
                column: "Nombre");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasBancarias_Clientes",
                table: "CuentasBancarias",
                column: "Clientes");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasBancarias_Name",
                table: "CuentasBancarias",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasBancarias_Numero",
                table: "CuentasBancarias",
                column: "Numero");

            migrationBuilder.CreateIndex(
                name: "IX_GruposEconomicos_Activo",
                table: "GruposEconomicos",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_GruposEconomicos_Name",
                table: "GruposEconomicos",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tarjetas_Clientes",
                table: "Tarjetas",
                column: "Clientes");

            migrationBuilder.CreateIndex(
                name: "IX_Tarjetas_Emisor",
                table: "Tarjetas",
                column: "Emisor");

            migrationBuilder.CreateIndex(
                name: "IX_Tarjetas_Vencimiento",
                table: "Tarjetas",
                column: "Vencimiento");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_CategoriasCliente_Categorias_de_cliente",
                table: "Clients",
                column: "Categorias_de_cliente",
                principalTable: "CategoriasCliente",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_GruposEconomicos_Grupos_economicos",
                table: "Clients",
                column: "Grupos_economicos",
                principalTable: "GruposEconomicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_CategoriasCliente_Categorias_de_cliente",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_Clients_GruposEconomicos_Grupos_economicos",
                table: "Clients");

            migrationBuilder.DropTable(
                name: "AutorizacionesCliente");

            migrationBuilder.DropTable(
                name: "CategoriasCliente");

            migrationBuilder.DropTable(
                name: "Contactos");

            migrationBuilder.DropTable(
                name: "CuentasBancarias");

            migrationBuilder.DropTable(
                name: "GruposEconomicos");

            migrationBuilder.DropTable(
                name: "Tarjetas");

            migrationBuilder.DropIndex(
                name: "IX_Clients_Activo",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Clients_Activo_FechaCreacion",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Clients_Categorias_de_cliente",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Clients_Corredor",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Clients_Grupos_economicos",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Clients_Nombre",
                table: "Clients");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaModificacion",
                table: "Clients",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValueSql: "CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "Clients",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<bool>(
                name: "Activo",
                table: "Clients",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldDefaultValue: true);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 21,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 22,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 23,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 24,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 25,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 26,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 27,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 28,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 29,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 30,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 31,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 32,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 33,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 34,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 35,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 36,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 37,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 38,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 5,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 6,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 7,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 8,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 9,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 10,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 11,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 12,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 13,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 14,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 15,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 16,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 17,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 18,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 19,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 20,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 21,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 22,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 23,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 24,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 25,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 26,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 27,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 28,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 29,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 30,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 31,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 32,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 33,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 34,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 35,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 36,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 37,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 38,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 39,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 40,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 41,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 42,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 43,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 44,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 45,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 46,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 47,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 48,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 49,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 50,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 51,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 52,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 53,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 54,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 55,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 56,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 57,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 58,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 59,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 60,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 61,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 62,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 63,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 64,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 65,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 66,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 67,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 68,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 69,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 70,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 71,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 72,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 73,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 74,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 75,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 76,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 77,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 78,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 79,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 80,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 81,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 82,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 83,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 84,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 85,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 86,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 87,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 88,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 89,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 90,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 91,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 92,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 93,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 94,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 95,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 96,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 97,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 98,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 99,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 100,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 101,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 102,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 103,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 104,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 105,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 106,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 107,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 108,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 109,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 110,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 111,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 112,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 113,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 114,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 115,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 116,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 117,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 118,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 119,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 120,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 121,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 122,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 123,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 124,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 125,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 126,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 127,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 128,
                column: "GrantedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945), new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945), new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945), new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945), new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945), new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945), new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "AssignedAt",
                value: new DateTime(2025, 6, 30, 18, 55, 53, 773, DateTimeKind.Utc).AddTicks(2945));
        }
    }
}
