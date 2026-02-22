using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetShop.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCamposPets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Pets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataCadastro",
                table: "Pets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Especie",
                table: "Pets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Idade",
                table: "Pets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Raca",
                table: "Pets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "DataCadastro",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "Especie",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "Idade",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "Raca",
                table: "Pets");
        }
    }
}
