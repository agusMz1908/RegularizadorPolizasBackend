using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegularizadorPolizas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFase1VelneoEntidades16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "Currencies");

            migrationBuilder.AlterColumn<string>(
                name: "Simbolo",
                table: "Currencies",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(5)",
                oldMaxLength: 5)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Moneda",
                table: "Currencies",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldMaxLength: 10)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaModificacion",
                table: "Currencies",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "Currencies",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Currencies",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldMaxLength: 10)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "EsMonedaBase",
                table: "Currencies",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "TipoCambio",
                table: "Currencies",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AutorizacionesDatos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutorizacionesDatos", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Calidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descripcion = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calidades", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descripcion = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CoberturasBicicleta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoberturasBicicleta", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Combustibles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descripcion = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Combustibles", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Departamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Dptnom = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Dptbonint = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ScCod = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departamentos", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Destinos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Desnom = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descod = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Destinos", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EdosGestion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EdosGestion", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FormasPago",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descripcion = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RequiereAutorizacion = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    EsContado = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MaximoCuotas = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormasPago", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MotivosNoRenovacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotivosNoRenovacion", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Secciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Icono = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Secciones", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Talleres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Telefono = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Direccion = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Contacto = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Cel = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CTMA = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Disponible24h = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Tarjeta = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Web = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observaciones = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Talleres", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TiposAlarma",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposAlarma", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TiposContrato",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TpoConDssc = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Secciones = table.Column<int>(type: "int", nullable: false),
                    TpoDet = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposContrato", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TiposRiesgo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TpoRieDssc = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RieDesc = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposRiesgo", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TiposSiniestro",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposSiniestro", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.CreateIndex(
                name: "IX_AutorizacionesDatos_Activo",
                table: "AutorizacionesDatos",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_AutorizacionesDatos_Name",
                table: "AutorizacionesDatos",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Calidades_Activo",
                table: "Calidades",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_Calidades_Name",
                table: "Calidades",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_Activo",
                table: "Categorias",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_Name",
                table: "Categorias",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CoberturasBicicleta_Activo",
                table: "CoberturasBicicleta",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_CoberturasBicicleta_Name",
                table: "CoberturasBicicleta",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Combustibles_Activo",
                table: "Combustibles",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_Combustibles_Name",
                table: "Combustibles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departamentos_Activo",
                table: "Departamentos",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_Departamentos_Dptnom",
                table: "Departamentos",
                column: "Dptnom",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departamentos_ScCod",
                table: "Departamentos",
                column: "ScCod");

            migrationBuilder.CreateIndex(
                name: "IX_Destinos_Activo",
                table: "Destinos",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_Destinos_Descod",
                table: "Destinos",
                column: "Descod");

            migrationBuilder.CreateIndex(
                name: "IX_Destinos_Desnom",
                table: "Destinos",
                column: "Desnom",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EdosGestion_Activo",
                table: "EdosGestion",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_EdosGestion_Name",
                table: "EdosGestion",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormasPago_Activo",
                table: "FormasPago",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_FormasPago_EsContado",
                table: "FormasPago",
                column: "EsContado");

            migrationBuilder.CreateIndex(
                name: "IX_FormasPago_Name",
                table: "FormasPago",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MotivosNoRenovacion_Activo",
                table: "MotivosNoRenovacion",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_MotivosNoRenovacion_Name",
                table: "MotivosNoRenovacion",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Secciones_Activo",
                table: "Secciones",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_Secciones_Name",
                table: "Secciones",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Talleres_Email",
                table: "Talleres",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Talleres_Name",
                table: "Talleres",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Talleres_Telefono",
                table: "Talleres",
                column: "Telefono");

            migrationBuilder.CreateIndex(
                name: "IX_TiposAlarma_Activo",
                table: "TiposAlarma",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_TiposAlarma_Name",
                table: "TiposAlarma",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiposContrato_Activo",
                table: "TiposContrato",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_TiposContrato_TpoConDssc",
                table: "TiposContrato",
                column: "TpoConDssc",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiposRiesgo_Activo",
                table: "TiposRiesgo",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_TiposRiesgo_TpoRieDssc",
                table: "TiposRiesgo",
                column: "TpoRieDssc",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiposSiniestro_Activo",
                table: "TiposSiniestro",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_TiposSiniestro_Name",
                table: "TiposSiniestro",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AutorizacionesDatos");

            migrationBuilder.DropTable(
                name: "Calidades");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "CoberturasBicicleta");

            migrationBuilder.DropTable(
                name: "Combustibles");

            migrationBuilder.DropTable(
                name: "Departamentos");

            migrationBuilder.DropTable(
                name: "Destinos");

            migrationBuilder.DropTable(
                name: "EdosGestion");

            migrationBuilder.DropTable(
                name: "FormasPago");

            migrationBuilder.DropTable(
                name: "MotivosNoRenovacion");

            migrationBuilder.DropTable(
                name: "Secciones");

            migrationBuilder.DropTable(
                name: "Talleres");

            migrationBuilder.DropTable(
                name: "TiposAlarma");

            migrationBuilder.DropTable(
                name: "TiposContrato");

            migrationBuilder.DropTable(
                name: "TiposRiesgo");

            migrationBuilder.DropTable(
                name: "TiposSiniestro");

            migrationBuilder.DropColumn(
                name: "EsMonedaBase",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "TipoCambio",
                table: "Currencies");

            migrationBuilder.AlterColumn<string>(
                name: "Simbolo",
                table: "Currencies",
                type: "varchar(5)",
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Moneda",
                table: "Currencies",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaModificacion",
                table: "Currencies",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaCreacion",
                table: "Currencies",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Currencies",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "Currencies",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
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
    }
}
