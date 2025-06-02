using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RegularizadorPolizas.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Brokers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Codigo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Domicilio = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Telefono = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brokers", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Corrcod = table.Column<int>(type: "int", nullable: true),
                    Subcorr = table.Column<int>(type: "int", nullable: true),
                    Clinom = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Telefono = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Clitelcel = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Clifchnac = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Clifching = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Clifchegr = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Clicargo = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Clicon = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Cliruc = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Clirsoc = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Cliced = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Clilib = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Clicatlib = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Clitpo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Clidir = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Cliemail = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Clivtoced = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Clivtolib = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Cliposcod = table.Column<int>(type: "int", nullable: true),
                    Clitelcorr = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Clidptnom = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Clisex = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Clitelant = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Cliobse = table.Column<string>(type: "TEXT", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Clifax = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Cliclasif = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Clinumrel = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Clicasapt = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Clidircob = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Clibse = table.Column<int>(type: "int", nullable: true),
                    Clifoto = table.Column<byte[]>(type: "LONGBLOB", nullable: false),
                    Pruebamillares = table.Column<int>(type: "int", nullable: true),
                    Ingresado = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Clialias = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Clipor = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Clisancor = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Clirsa = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Codposcob = table.Column<int>(type: "int", nullable: true),
                    Clidptcob = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Cli_s_cris = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Clifchnac1 = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Clilocnom = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Cliloccob = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Categorias_de_cliente = table.Column<int>(type: "int", nullable: true),
                    Sc_departamentos = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Sc_localidades = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Fch_ingreso = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Grupos_economicos = table.Column<int>(type: "int", nullable: true),
                    Etiquetas = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Doc_digi = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Password = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Habilita_app = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Referido = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Altura = table.Column<int>(type: "int", nullable: true),
                    Peso = table.Column<int>(type: "int", nullable: true),
                    Cliberkley = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Clifar = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Clisurco = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Clihdi = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Climapfre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Climetlife = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Clisancris = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Clisbi = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Edo_civil = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Not_bien_mail = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Not_bien_wap = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Ing_poliza_mail = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Ing_poliza_wap = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Ing_siniestro_mail = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Ing_siniestro_wap = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Noti_obs_sini_mail = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Noti_obs_sini_wap = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Last_update = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    App_id = table.Column<int>(type: "int", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Alias = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Codigo = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Codigo = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Simbolo = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Polizas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Clinro = table.Column<int>(type: "int", nullable: true),
                    Comcod = table.Column<int>(type: "int", nullable: true),
                    Seccod = table.Column<int>(type: "int", nullable: true),
                    Condom = table.Column<string>(type: "TEXT", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conmaraut = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conanioaut = table.Column<int>(type: "int", nullable: true),
                    Concodrev = table.Column<int>(type: "int", nullable: true),
                    Conmataut = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conficto = table.Column<int>(type: "int", nullable: true),
                    Conmotor = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conpadaut = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conchasis = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conclaaut = table.Column<int>(type: "int", nullable: true),
                    Condedaut = table.Column<int>(type: "int", nullable: true),
                    Conresciv = table.Column<int>(type: "int", nullable: true),
                    Conbonnsin = table.Column<int>(type: "int", nullable: true),
                    Conbonant = table.Column<int>(type: "int", nullable: true),
                    Concaraut = table.Column<int>(type: "int", nullable: true),
                    Concesnom = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Concestel = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Concapaut = table.Column<int>(type: "int", nullable: true),
                    Conpremio = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Contot = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Moncod = table.Column<int>(type: "int", nullable: true),
                    Concuo = table.Column<int>(type: "int", nullable: true),
                    Concomcorr = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    Catdsc = table.Column<int>(type: "int", nullable: true),
                    Desdsc = table.Column<int>(type: "int", nullable: true),
                    Caldsc = table.Column<int>(type: "int", nullable: true),
                    Flocod = table.Column<int>(type: "int", nullable: true),
                    Concar = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conpol = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conend = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Confchdes = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Confchhas = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Conimp = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Connroser = table.Column<int>(type: "int", nullable: true),
                    Rieres = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conges = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Congesti = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Congesfi = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Congeses = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Convig = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Concan = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Congrucon = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conpadre = table.Column<int>(type: "int", nullable: true),
                    Conidpad = table.Column<int>(type: "int", nullable: true),
                    Confchcan = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Concaucan = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Contipoemp = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conmatpar = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conmatte = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Concapla = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Conflota = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Condednum = table.Column<int>(type: "int", nullable: true),
                    Consta = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Contra = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conconf = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conobjtot = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Contpoact = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conesp = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Convalacr = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Convallet = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Condecram = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conmedtra = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conviades = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conviaa = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conviaenb = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conviakb = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Conviakn = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Conviatra = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conviacos = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Conviafle = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Dptnom = table.Column<int>(type: "int", nullable: true),
                    Conedaret = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Congar = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Condecpri = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Condecpro = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Condecptj = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Conubi = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Concaudsc = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conincuno = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conviagas = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Conviarec = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Conviapri = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Linobs = table.Column<int>(type: "int", nullable: true),
                    Concomdes = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Concalcom = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Tpoconcod = table.Column<int>(type: "int", nullable: true),
                    Tpovivcod = table.Column<int>(type: "int", nullable: true),
                    Tporiecod = table.Column<int>(type: "int", nullable: true),
                    Modcod = table.Column<int>(type: "int", nullable: true),
                    Concapase = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Conpricap = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Tposegdsc = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conriecod = table.Column<int>(type: "int", nullable: true),
                    Conriedsc = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conrecfin = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Conimprf = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Conafesin = table.Column<int>(type: "int", nullable: true),
                    Conautcor = table.Column<int>(type: "int", nullable: true),
                    Conlinrie = table.Column<int>(type: "int", nullable: true),
                    Conconesp = table.Column<int>(type: "int", nullable: true),
                    Conlimnav = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Contpocob = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Connomemb = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Contpoemb = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Lincarta = table.Column<int>(type: "int", nullable: true),
                    Cancecod = table.Column<int>(type: "int", nullable: true),
                    Concomotr = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Conautcome = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conviafac = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conviamon = table.Column<int>(type: "int", nullable: true),
                    Conviatpo = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Connrorc = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Condedurc = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Lininclu = table.Column<int>(type: "int", nullable: true),
                    Linexclu = table.Column<int>(type: "int", nullable: true),
                    Concapret = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Forpagvid = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Clinom = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Tarcod = table.Column<int>(type: "int", nullable: true),
                    Corrnom = table.Column<int>(type: "int", nullable: true),
                    Connroint = table.Column<int>(type: "int", nullable: true),
                    Conautnd = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conpadend = table.Column<int>(type: "int", nullable: true),
                    Contotpri = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Padreaux = table.Column<int>(type: "int", nullable: true),
                    Conlinflot = table.Column<int>(type: "int", nullable: true),
                    Conflotimp = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Conflottotal = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Conflotsaldo = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Conaccicer = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Concerfin = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Condetemb = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conclaemb = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Confabemb = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conbanemb = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conmatemb = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Convelemb = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conmatriemb = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conptoemb = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Otrcorrcod = table.Column<int>(type: "int", nullable: true),
                    Condeta = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observaciones = table.Column<string>(type: "TEXT", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Clipcupfia = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Conclieda = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Condecrea = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Condecaju = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conviatot = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Contpoemp = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Congaran = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Congarantel = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Mot_no_ren = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Condetrc = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conautcort = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Condetail = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Clinro1 = table.Column<int>(type: "int", nullable: true),
                    Consumsal = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Conespbon = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Leer = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Enviado = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Sob_recib = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Leer_obs = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Sublistas = table.Column<string>(type: "TEXT", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Com_sub_corr = table.Column<int>(type: "int", nullable: true),
                    Tipos_de_alarma = table.Column<int>(type: "int", nullable: true),
                    Tiene_alarma = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Coberturas_bicicleta = table.Column<int>(type: "int", nullable: true),
                    Com_bro = table.Column<int>(type: "int", nullable: true),
                    Com_bo = table.Column<int>(type: "int", nullable: true),
                    Contotant = table.Column<decimal>(type: "decimal(15,2)", nullable: true),
                    Cotizacion = table.Column<string>(type: "TEXT", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Motivos_no_renovacion = table.Column<int>(type: "int", nullable: true),
                    Com_alias = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ramo = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Clausula = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Aereo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Maritimo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Terrestre = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Max_aereo = table.Column<int>(type: "int", nullable: true),
                    Max_mar = table.Column<int>(type: "int", nullable: true),
                    Max_terrestre = table.Column<int>(type: "int", nullable: true),
                    Tasa = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    Facturacion = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Importacion = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Exportacion = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Offshore = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Transito_interno = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Coning = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Cat_cli = table.Column<int>(type: "int", nullable: true),
                    Llamar = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Granizo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Idorden = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Var_ubi = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Mis_rie = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Ingresado = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Last_update = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Comcod1 = table.Column<int>(type: "int", nullable: true),
                    Comcod2 = table.Column<int>(type: "int", nullable: true),
                    Pagos_efectivo = table.Column<int>(type: "int", nullable: true),
                    Productos_de_vida = table.Column<int>(type: "int", nullable: true),
                    App_id = table.Column<int>(type: "int", nullable: true),
                    Update_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Gestion = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Asignado = table.Column<int>(type: "int", nullable: true),
                    Combustibles = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentoPdf = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Procesado = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Polizas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Polizas_Brokers_Corrnom",
                        column: x => x.Corrnom,
                        principalTable: "Brokers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Polizas_Clients_Clinro",
                        column: x => x.Clinro,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Polizas_Companies_Comcod",
                        column: x => x.Comcod,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Polizas_Currencies_Moncod",
                        column: x => x.Moncod,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Polizas_Polizas_Conpadre",
                        column: x => x.Conpadre,
                        principalTable: "Polizas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProcessDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NombreArchivo = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RutaArchivo = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipoDocumento = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EstadoProcesamiento = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ResultadoJson = table.Column<string>(type: "TEXT", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PolizaId = table.Column<int>(type: "int", nullable: true),
                    FechaProcesamiento = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessDocuments_Polizas_PolizaId",
                        column: x => x.PolizaId,
                        principalTable: "Polizas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProcessDocuments_Users_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Renovations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PolizaId = table.Column<int>(type: "int", nullable: false),
                    PolizaNuevaId = table.Column<int>(type: "int", nullable: true),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Estado = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observaciones = table.Column<string>(type: "TEXT", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Renovations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Renovations_Polizas_PolizaId",
                        column: x => x.PolizaId,
                        principalTable: "Polizas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Renovations_Polizas_PolizaNuevaId",
                        column: x => x.PolizaNuevaId,
                        principalTable: "Polizas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Renovations_Users_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "Activo", "Alias", "Codigo", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "BSE", "BSE", "Banco de Seguros del Estado" },
                    { 2, true, "SURA", "SURA", "SURA Uruguay" },
                    { 3, true, "MAPFRE", "MAPFRE", "Mapfre Uruguay" },
                    { 4, true, "SAN CRISTOBAL", "SC", "San Cristóbal" }
                });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "Activo", "Codigo", "Nombre", "Simbolo" },
                values: new object[,]
                {
                    { 1, true, "UYU", "Peso Uruguayo", "$" },
                    { 2, true, "USD", "Dólar Americano", "US$" },
                    { 3, true, "UI", "Unidad Indexada", "UI" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Activo", "Email", "FechaCreacion", "FechaModificacion", "Nombre" },
                values: new object[] { 1, true, "admin@sistema.com", new DateTime(2025, 5, 26, 16, 8, 52, 542, DateTimeKind.Local).AddTicks(5783), new DateTime(2025, 5, 26, 16, 8, 52, 542, DateTimeKind.Local).AddTicks(6023), "Administrador" });

            migrationBuilder.CreateIndex(
                name: "IX_Brokers_Codigo",
                table: "Brokers",
                column: "Codigo");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Cliced",
                table: "Clients",
                column: "Cliced");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Cliemail",
                table: "Clients",
                column: "Cliemail");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Cliruc",
                table: "Clients",
                column: "Cliruc");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Codigo",
                table: "Companies",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_Codigo",
                table: "Currencies",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Polizas_Activo",
                table: "Polizas",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_Polizas_Clinro",
                table: "Polizas",
                column: "Clinro");

            migrationBuilder.CreateIndex(
                name: "IX_Polizas_Comcod",
                table: "Polizas",
                column: "Comcod");

            migrationBuilder.CreateIndex(
                name: "IX_Polizas_Confchdes",
                table: "Polizas",
                column: "Confchdes");

            migrationBuilder.CreateIndex(
                name: "IX_Polizas_Confchhas",
                table: "Polizas",
                column: "Confchhas");

            migrationBuilder.CreateIndex(
                name: "IX_Polizas_Conmataut",
                table: "Polizas",
                column: "Conmataut");

            migrationBuilder.CreateIndex(
                name: "IX_Polizas_Conpadre",
                table: "Polizas",
                column: "Conpadre");

            migrationBuilder.CreateIndex(
                name: "IX_Polizas_Conpol",
                table: "Polizas",
                column: "Conpol");

            migrationBuilder.CreateIndex(
                name: "IX_Polizas_Corrnom",
                table: "Polizas",
                column: "Corrnom");

            migrationBuilder.CreateIndex(
                name: "IX_Polizas_Moncod",
                table: "Polizas",
                column: "Moncod");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessDocuments_PolizaId",
                table: "ProcessDocuments",
                column: "PolizaId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessDocuments_UsuarioId",
                table: "ProcessDocuments",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Renovations_PolizaId",
                table: "Renovations",
                column: "PolizaId");

            migrationBuilder.CreateIndex(
                name: "IX_Renovations_PolizaNuevaId",
                table: "Renovations",
                column: "PolizaNuevaId");

            migrationBuilder.CreateIndex(
                name: "IX_Renovations_UsuarioId",
                table: "Renovations",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessDocuments");

            migrationBuilder.DropTable(
                name: "Renovations");

            migrationBuilder.DropTable(
                name: "Polizas");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Brokers");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "Currencies");
        }
    }
}
