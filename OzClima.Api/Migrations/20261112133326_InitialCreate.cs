using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OzClima.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AreasMonitoradas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Bairro = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Latitude = table.Column<double>(type: "REAL", nullable: false),
                    Longitude = table.Column<double>(type: "REAL", nullable: false),
                    NivelArborizacao = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Ativa = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreasMonitoradas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AlertasCalor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AreaMonitoradaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Nivel = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Mensagem = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    DataHora = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertasCalor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlertasCalor_AreasMonitoradas_AreaMonitoradaId",
                        column: x => x.AreaMonitoradaId,
                        principalTable: "AreasMonitoradas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeiturasClimaticas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AreaMonitoradaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Temperatura = table.Column<decimal>(type: "TEXT", nullable: false),
                    Umidade = table.Column<decimal>(type: "TEXT", nullable: false),
                    DataHora = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeiturasClimaticas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeiturasClimaticas_AreasMonitoradas_AreaMonitoradaId",
                        column: x => x.AreaMonitoradaId,
                        principalTable: "AreasMonitoradas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PontosApoio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AreaMonitoradaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Endereco = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Latitude = table.Column<double>(type: "REAL", nullable: false),
                    Longitude = table.Column<double>(type: "REAL", nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PontosApoio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PontosApoio_AreasMonitoradas_AreaMonitoradaId",
                        column: x => x.AreaMonitoradaId,
                        principalTable: "AreasMonitoradas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlertasCalor_AreaMonitoradaId",
                table: "AlertasCalor",
                column: "AreaMonitoradaId");

            migrationBuilder.CreateIndex(
                name: "IX_LeiturasClimaticas_AreaMonitoradaId_DataHora",
                table: "LeiturasClimaticas",
                columns: new[] { "AreaMonitoradaId", "DataHora" });

            migrationBuilder.CreateIndex(
                name: "IX_PontosApoio_AreaMonitoradaId",
                table: "PontosApoio",
                column: "AreaMonitoradaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlertasCalor");

            migrationBuilder.DropTable(
                name: "LeiturasClimaticas");

            migrationBuilder.DropTable(
                name: "PontosApoio");

            migrationBuilder.DropTable(
                name: "AreasMonitoradas");
        }
    }
}
