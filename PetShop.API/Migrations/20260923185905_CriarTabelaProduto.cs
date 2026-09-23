using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetShop.API.Migrations
{
    /// <inheritdoc />
    public partial class CriarTabelaProduto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Categoria = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    Unidade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrecoUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    EstoqueMinimo = table.Column<int>(type: "int", nullable: false),
                    Fornecedor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Produtos");
        }
    }
}
