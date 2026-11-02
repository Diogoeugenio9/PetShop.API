using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetShop.API.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarPeriodoTeste : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CriadoEmUtc",
                table: "Administradores",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TesteIniciadoEmUtc",
                table: "Administradores",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "TesteTerminaEmUtc",
                table: "Administradores",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            // Administradores antigos não têm data de cadastro: o teste deles começa na data desta migration.
            migrationBuilder.Sql(@"
DECLARE @agora datetime2 = SYSUTCDATETIME();
UPDATE Administradores
SET TesteIniciadoEmUtc = @agora,
    TesteTerminaEmUtc = DATEADD(day, 15, @agora)
WHERE TesteIniciadoEmUtc = '0001-01-01';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CriadoEmUtc",
                table: "Administradores");

            migrationBuilder.DropColumn(
                name: "TesteIniciadoEmUtc",
                table: "Administradores");

            migrationBuilder.DropColumn(
                name: "TesteTerminaEmUtc",
                table: "Administradores");
        }
    }
}
