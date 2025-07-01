using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegularizadorPolizas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentIntelligenceFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Idorden",
                table: "Polizas",
                newName: "IdOrden");

            migrationBuilder.RenameColumn(
                name: "Var_ubi",
                table: "Polizas",
                newName: "VarUbi");

            migrationBuilder.RenameColumn(
                name: "Update_date",
                table: "Polizas",
                newName: "UpdateDate");

            migrationBuilder.RenameColumn(
                name: "Transito_interno",
                table: "Polizas",
                newName: "TransitoInterno");

            migrationBuilder.RenameColumn(
                name: "Tipos_de_alarma",
                table: "Polizas",
                newName: "VehiculoId");

            migrationBuilder.RenameColumn(
                name: "Tiene_alarma",
                table: "Polizas",
                newName: "TieneAlarma");

            migrationBuilder.RenameColumn(
                name: "Sob_recib",
                table: "Polizas",
                newName: "SobRecib");

            migrationBuilder.RenameColumn(
                name: "Productos_de_vida",
                table: "Polizas",
                newName: "TiposDeAlarma");

            migrationBuilder.RenameColumn(
                name: "Pagos_efectivo",
                table: "Polizas",
                newName: "ProductosDeVida");

            migrationBuilder.RenameColumn(
                name: "Motivos_no_renovacion",
                table: "Polizas",
                newName: "PagosEfectivo");

            migrationBuilder.RenameColumn(
                name: "Mot_no_ren",
                table: "Polizas",
                newName: "MotNoRen");

            migrationBuilder.RenameColumn(
                name: "Mis_rie",
                table: "Polizas",
                newName: "MisRie");

            migrationBuilder.RenameColumn(
                name: "Max_terrestre",
                table: "Polizas",
                newName: "MotivosNoRenovacion");

            migrationBuilder.RenameColumn(
                name: "Max_mar",
                table: "Polizas",
                newName: "MaxTerrestre");

            migrationBuilder.RenameColumn(
                name: "Max_aereo",
                table: "Polizas",
                newName: "MaxMar");

            migrationBuilder.RenameColumn(
                name: "Leer_obs",
                table: "Polizas",
                newName: "LeerObs");

            migrationBuilder.RenameColumn(
                name: "Last_update",
                table: "Polizas",
                newName: "LastUpdate");

            migrationBuilder.RenameColumn(
                name: "Com_sub_corr",
                table: "Polizas",
                newName: "MaxAereo");

            migrationBuilder.RenameColumn(
                name: "Com_bro",
                table: "Polizas",
                newName: "ComSubCorr");

            migrationBuilder.RenameColumn(
                name: "Com_bo",
                table: "Polizas",
                newName: "ComBro");

            migrationBuilder.RenameColumn(
                name: "Com_alias",
                table: "Polizas",
                newName: "ComAlias");

            migrationBuilder.RenameColumn(
                name: "Coberturas_bicicleta",
                table: "Polizas",
                newName: "ComBo");

            migrationBuilder.RenameColumn(
                name: "Cat_cli",
                table: "Polizas",
                newName: "CoberturasBicicleta");

            migrationBuilder.RenameColumn(
                name: "App_id",
                table: "Polizas",
                newName: "CatCli");

            migrationBuilder.AlterColumn<string>(
                name: "Condetail",
                table: "Polizas",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "AppId",
                table: "Polizas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ConfianzaIA",
                table: "Polizas",
                type: "decimal(5,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrigenDocumento",
                table: "Polizas",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "TipoOperacion",
                table: "Polizas",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Vehiculos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Conmaraut = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conanioaut = table.Column<int>(type: "int", nullable: true),
                    Conmataut = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conmotor = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conpadaut = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conchasis = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Concodrev = table.Column<int>(type: "int", nullable: true),
                    Conficto = table.Column<int>(type: "int", nullable: true),
                    Conclaaut = table.Column<int>(type: "int", nullable: true),
                    Condedaut = table.Column<int>(type: "int", nullable: true),
                    Concaraut = table.Column<int>(type: "int", nullable: true),
                    Concapaut = table.Column<int>(type: "int", nullable: true),
                    Conresciv = table.Column<int>(type: "int", nullable: true),
                    Conbonnsin = table.Column<int>(type: "int", nullable: true),
                    Conbonant = table.Column<int>(type: "int", nullable: true),
                    CategoriaId = table.Column<int>(type: "int", nullable: true),
                    CalidadId = table.Column<int>(type: "int", nullable: true),
                    DestinoId = table.Column<int>(type: "int", nullable: true),
                    CombustibleId = table.Column<int>(type: "int", nullable: true),
                    Condetail = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Contpocob = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Contipoemp = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conmatpar = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conmatte = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Catdsc = table.Column<int>(type: "int", nullable: true),
                    Caldsc = table.Column<int>(type: "int", nullable: true),
                    Desdsc = table.Column<int>(type: "int", nullable: true),
                    Combustibles = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TieneAlarma = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    TiposDeAlarma = table.Column<int>(type: "int", nullable: true),
                    Granizo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP"),
                    CreadoPor = table.Column<int>(type: "int", nullable: true),
                    ModificadoPor = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehiculos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehiculos_Calidades_CalidadId",
                        column: x => x.CalidadId,
                        principalTable: "Calidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Vehiculos_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Vehiculos_Combustibles_CombustibleId",
                        column: x => x.CombustibleId,
                        principalTable: "Combustibles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Vehiculos_Destinos_DestinoId",
                        column: x => x.DestinoId,
                        principalTable: "Destinos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 21,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 22,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 23,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 24,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 25,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 26,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 27,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 28,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 29,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 30,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 31,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 32,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 33,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 34,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 35,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 36,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 37,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 38,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 1,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 2,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 3,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 4,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 5,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 6,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 7,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 8,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 9,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 10,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 11,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 12,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 13,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 14,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 15,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 16,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 17,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 18,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 19,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 20,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 21,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 22,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 23,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 24,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 25,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 26,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 27,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 28,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 29,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 30,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 31,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 32,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 33,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 34,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 35,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 36,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 37,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 38,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 39,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 40,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 41,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 42,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 43,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 44,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 45,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 46,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 47,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 48,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 49,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 50,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 51,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 52,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 53,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 54,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 55,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 56,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 57,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 58,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 59,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 60,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 61,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 62,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 63,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 64,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 65,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 66,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 67,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 68,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 69,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 70,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 71,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 72,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 73,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 74,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 75,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 76,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 77,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 78,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 79,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 80,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 81,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 82,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 83,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 84,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 85,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 86,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 87,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 88,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 89,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 90,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 91,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 92,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 93,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 94,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 95,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 96,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 97,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 98,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 99,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 100,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 101,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 102,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 103,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 104,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 105,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 106,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 107,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 108,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 109,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 110,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 111,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 112,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 113,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 114,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 115,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 116,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 117,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 118,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 119,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 120,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 121,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 122,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 123,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 124,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 125,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 126,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 127,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 128,
                column: "GrantedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229), new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229), new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229), new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229), new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229), new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229), new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "AssignedAt",
                value: new DateTime(2025, 7, 1, 13, 7, 21, 636, DateTimeKind.Utc).AddTicks(5229));

            migrationBuilder.CreateIndex(
                name: "IX_Polizas_VehiculoId",
                table: "Polizas",
                column: "VehiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_Activo",
                table: "Vehiculos",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_Activo_FechaCreacion",
                table: "Vehiculos",
                columns: new[] { "Activo", "FechaCreacion" });

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_CalidadId",
                table: "Vehiculos",
                column: "CalidadId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_CategoriaId",
                table: "Vehiculos",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_CombustibleId",
                table: "Vehiculos",
                column: "CombustibleId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_Conanioaut",
                table: "Vehiculos",
                column: "Conanioaut");

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_Conchasis",
                table: "Vehiculos",
                column: "Conchasis");

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_Conmaraut",
                table: "Vehiculos",
                column: "Conmaraut");

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_Conmataut",
                table: "Vehiculos",
                column: "Conmataut",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_Conmotor",
                table: "Vehiculos",
                column: "Conmotor");

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_Conpadaut",
                table: "Vehiculos",
                column: "Conpadaut");

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_DestinoId",
                table: "Vehiculos",
                column: "DestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculos_Marca_Anio_Matricula",
                table: "Vehiculos",
                columns: new[] { "Conmaraut", "Conanioaut", "Conmataut" });

            migrationBuilder.AddForeignKey(
                name: "FK_Polizas_Vehiculos_VehiculoId",
                table: "Polizas",
                column: "VehiculoId",
                principalTable: "Vehiculos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Polizas_Vehiculos_VehiculoId",
                table: "Polizas");

            migrationBuilder.DropTable(
                name: "Vehiculos");

            migrationBuilder.DropIndex(
                name: "IX_Polizas_VehiculoId",
                table: "Polizas");

            migrationBuilder.DropColumn(
                name: "AppId",
                table: "Polizas");

            migrationBuilder.DropColumn(
                name: "ConfianzaIA",
                table: "Polizas");

            migrationBuilder.DropColumn(
                name: "OrigenDocumento",
                table: "Polizas");

            migrationBuilder.DropColumn(
                name: "TipoOperacion",
                table: "Polizas");

            migrationBuilder.RenameColumn(
                name: "IdOrden",
                table: "Polizas",
                newName: "Idorden");

            migrationBuilder.RenameColumn(
                name: "VehiculoId",
                table: "Polizas",
                newName: "Tipos_de_alarma");

            migrationBuilder.RenameColumn(
                name: "VarUbi",
                table: "Polizas",
                newName: "Var_ubi");

            migrationBuilder.RenameColumn(
                name: "UpdateDate",
                table: "Polizas",
                newName: "Update_date");

            migrationBuilder.RenameColumn(
                name: "TransitoInterno",
                table: "Polizas",
                newName: "Transito_interno");

            migrationBuilder.RenameColumn(
                name: "TiposDeAlarma",
                table: "Polizas",
                newName: "Productos_de_vida");

            migrationBuilder.RenameColumn(
                name: "TieneAlarma",
                table: "Polizas",
                newName: "Tiene_alarma");

            migrationBuilder.RenameColumn(
                name: "SobRecib",
                table: "Polizas",
                newName: "Sob_recib");

            migrationBuilder.RenameColumn(
                name: "ProductosDeVida",
                table: "Polizas",
                newName: "Pagos_efectivo");

            migrationBuilder.RenameColumn(
                name: "PagosEfectivo",
                table: "Polizas",
                newName: "Motivos_no_renovacion");

            migrationBuilder.RenameColumn(
                name: "MotivosNoRenovacion",
                table: "Polizas",
                newName: "Max_terrestre");

            migrationBuilder.RenameColumn(
                name: "MotNoRen",
                table: "Polizas",
                newName: "Mot_no_ren");

            migrationBuilder.RenameColumn(
                name: "MisRie",
                table: "Polizas",
                newName: "Mis_rie");

            migrationBuilder.RenameColumn(
                name: "MaxTerrestre",
                table: "Polizas",
                newName: "Max_mar");

            migrationBuilder.RenameColumn(
                name: "MaxMar",
                table: "Polizas",
                newName: "Max_aereo");

            migrationBuilder.RenameColumn(
                name: "MaxAereo",
                table: "Polizas",
                newName: "Com_sub_corr");

            migrationBuilder.RenameColumn(
                name: "LeerObs",
                table: "Polizas",
                newName: "Leer_obs");

            migrationBuilder.RenameColumn(
                name: "LastUpdate",
                table: "Polizas",
                newName: "Last_update");

            migrationBuilder.RenameColumn(
                name: "ComSubCorr",
                table: "Polizas",
                newName: "Com_bro");

            migrationBuilder.RenameColumn(
                name: "ComBro",
                table: "Polizas",
                newName: "Com_bo");

            migrationBuilder.RenameColumn(
                name: "ComBo",
                table: "Polizas",
                newName: "Coberturas_bicicleta");

            migrationBuilder.RenameColumn(
                name: "ComAlias",
                table: "Polizas",
                newName: "Com_alias");

            migrationBuilder.RenameColumn(
                name: "CoberturasBicicleta",
                table: "Polizas",
                newName: "Cat_cli");

            migrationBuilder.RenameColumn(
                name: "CatCli",
                table: "Polizas",
                newName: "App_id");

            migrationBuilder.AlterColumn<string>(
                name: "Condetail",
                table: "Polizas",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

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
        }
    }
}
