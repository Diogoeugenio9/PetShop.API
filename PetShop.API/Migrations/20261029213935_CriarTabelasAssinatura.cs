using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetShop.API.Migrations
{
    /// <inheritdoc />
    public partial class CriarTabelasAssinatura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assinaturas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdministradorId = table.Column<int>(type: "int", nullable: false),
                    Plano = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CheckoutId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AssinaturaExternaId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RenovacaoCancelada = table.Column<bool>(type: "bit", nullable: false),
                    CriadaEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtivadaEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PagoAte = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CanceladaEm = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assinaturas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assinaturas_Administradores_AdministradorId",
                        column: x => x.AdministradorId,
                        principalTable: "Administradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventosPagamento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventoId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Evento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RecebidoEm = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventosPagamento", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assinaturas_AdministradorId",
                table: "Assinaturas",
                column: "AdministradorId");

            migrationBuilder.CreateIndex(
                name: "IX_Assinaturas_CheckoutId",
                table: "Assinaturas",
                column: "CheckoutId",
                unique: true,
                filter: "[CheckoutId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EventosPagamento_EventoId",
                table: "EventosPagamento",
                column: "EventoId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assinaturas");

            migrationBuilder.DropTable(
                name: "EventosPagamento");
        }
    }
}
