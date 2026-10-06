using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetShop.API.Migrations
{
    /// <inheritdoc />
    public partial class PortalClienteAgendaVacinas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Observacoes",
                table: "PetsModelo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Peso",
                table: "PetsModelo",
                type: "decimal(6,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sexo",
                table: "PetsModelo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AgendamentoId",
                table: "Lancamentos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServicoId",
                table: "Lancamentos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenhaHash",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Observacoes",
                table: "Agendamentos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ConfiguracoesLoja",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdministradorId = table.Column<int>(type: "int", nullable: false),
                    HoraAbertura = table.Column<TimeSpan>(type: "time", nullable: false),
                    HoraFechamento = table.Column<TimeSpan>(type: "time", nullable: false),
                    InicioIntervalo = table.Column<TimeSpan>(type: "time", nullable: true),
                    FimIntervalo = table.Column<TimeSpan>(type: "time", nullable: true),
                    IntervaloEntreHorariosMinutos = table.Column<int>(type: "int", nullable: false),
                    CapacidadeSimultanea = table.Column<int>(type: "int", nullable: false),
                    AntecedenciaMinimaHoras = table.Column<int>(type: "int", nullable: false),
                    DiasFuncionamento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MetaAgendamentosMensal = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracoesLoja", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfiguracoesLoja_Administradores_AdministradorId",
                        column: x => x.AdministradorId,
                        principalTable: "Administradores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vacinas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PetId = table.Column<int>(type: "int", nullable: false),
                    NomeVacina = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Fabricante = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataAplicacao = table.Column<DateOnly>(type: "date", nullable: false),
                    DataProximaDose = table.Column<DateOnly>(type: "date", nullable: true),
                    DoseUnica = table.Column<bool>(type: "bit", nullable: false),
                    Observacoes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vacinas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vacinas_PetsModelo_PetId",
                        column: x => x.PetId,
                        principalTable: "PetsModelo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lancamentos_AgendamentoId",
                table: "Lancamentos",
                column: "AgendamentoId",
                unique: true,
                filter: "[AgendamentoId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracoesLoja_AdministradorId",
                table: "ConfiguracoesLoja",
                column: "AdministradorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vacinas_PetId",
                table: "Vacinas",
                column: "PetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracoesLoja");

            migrationBuilder.DropTable(
                name: "Vacinas");

            migrationBuilder.DropIndex(
                name: "IX_Lancamentos_AgendamentoId",
                table: "Lancamentos");

            migrationBuilder.DropColumn(
                name: "Observacoes",
                table: "PetsModelo");

            migrationBuilder.DropColumn(
                name: "Peso",
                table: "PetsModelo");

            migrationBuilder.DropColumn(
                name: "Sexo",
                table: "PetsModelo");

            migrationBuilder.DropColumn(
                name: "AgendamentoId",
                table: "Lancamentos");

            migrationBuilder.DropColumn(
                name: "ServicoId",
                table: "Lancamentos");

            migrationBuilder.DropColumn(
                name: "SenhaHash",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Observacoes",
                table: "Agendamentos");
        }
    }
}
