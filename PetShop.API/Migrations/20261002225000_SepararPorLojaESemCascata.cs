using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetShop.API.Migrations
{
    public partial class SepararPorLojaESemCascata : Migration
    {
        private static readonly string[] TabelasComLoja = { "Clientes", "Servicos", "Produtos", "Lancamentos" };
        private static readonly string[] TabelasComExcluido = { "Clientes", "PetsModelo", "Servicos", "Produtos" };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (var tabela in TabelasComExcluido)
            {
                migrationBuilder.AddColumn<bool>(
                    name: "Excluido",
                    table: tabela,
                    type: "bit",
                    nullable: false,
                    defaultValue: false);
            }

            foreach (var tabela in TabelasComLoja)
            {
                migrationBuilder.AddColumn<int>(
                    name: "AdministradorId",
                    table: tabela,
                    type: "int",
                    nullable: true);
            }

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM [Administradores])
   AND (EXISTS (SELECT 1 FROM [Clientes]) OR EXISTS (SELECT 1 FROM [Servicos])
        OR EXISTS (SELECT 1 FROM [Produtos]) OR EXISTS (SELECT 1 FROM [Lancamentos]))
    RAISERROR('Existem dados mas nenhum administrador. Cadastre um administrador (POST api/admin/autenticacao/registrar) e aplique a migration de novo.', 16, 1);");

            foreach (var tabela in TabelasComLoja)
            {
                migrationBuilder.Sql(
                    $"UPDATE [{tabela}] SET [AdministradorId] = (SELECT MIN([Id]) FROM [Administradores]) WHERE [AdministradorId] IS NULL;");
            }

            foreach (var tabela in TabelasComLoja)
            {
                migrationBuilder.AlterColumn<int>(
                    name: "AdministradorId",
                    table: tabela,
                    type: "int",
                    nullable: false,
                    oldClrType: typeof(int),
                    oldType: "int",
                    oldNullable: true);

                migrationBuilder.CreateIndex(
                    name: $"IX_{tabela}_AdministradorId",
                    table: tabela,
                    column: "AdministradorId");

                migrationBuilder.AddForeignKey(
                    name: $"FK_{tabela}_Administradores_AdministradorId",
                    table: tabela,
                    column: "AdministradorId",
                    principalTable: "Administradores",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            }

            migrationBuilder.DropForeignKey(
                name: "FK_PetsModelo_Clientes_ClienteId",
                table: "PetsModelo");

            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_PetsModelo_PetId",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Servicos_ServicoId",
                table: "Agendamentos");

            migrationBuilder.AddForeignKey(
                name: "FK_PetsModelo_Clientes_ClienteId",
                table: "PetsModelo",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_PetsModelo_PetId",
                table: "Agendamentos",
                column: "PetId",
                principalTable: "PetsModelo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Servicos_ServicoId",
                table: "Agendamentos",
                column: "ServicoId",
                principalTable: "Servicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PetsModelo_Clientes_ClienteId",
                table: "PetsModelo");

            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_PetsModelo_PetId",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Servicos_ServicoId",
                table: "Agendamentos");

            migrationBuilder.AddForeignKey(
                name: "FK_PetsModelo_Clientes_ClienteId",
                table: "PetsModelo",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_PetsModelo_PetId",
                table: "Agendamentos",
                column: "PetId",
                principalTable: "PetsModelo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Servicos_ServicoId",
                table: "Agendamentos",
                column: "ServicoId",
                principalTable: "Servicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            foreach (var tabela in TabelasComLoja)
            {
                migrationBuilder.DropForeignKey(
                    name: $"FK_{tabela}_Administradores_AdministradorId",
                    table: tabela);

                migrationBuilder.DropIndex(
                    name: $"IX_{tabela}_AdministradorId",
                    table: tabela);

                migrationBuilder.DropColumn(
                    name: "AdministradorId",
                    table: tabela);
            }

            foreach (var tabela in TabelasComExcluido)
            {
                migrationBuilder.DropColumn(
                    name: "Excluido",
                    table: tabela);
            }
        }
    }
}
