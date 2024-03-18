using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BancoDeDados.Migrations
{
    /// <inheritdoc />
    public partial class Versao3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CPF = table.Column<string>(type: "char(14)", nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Endereco_Logradouro = table.Column<string>(type: "TEXT", nullable: false),
                    Endereco_Numero = table.Column<string>(type: "TEXT", nullable: false),
                    Endereco_Complemento = table.Column<string>(type: "TEXT", nullable: false),
                    Endereco_Bairro = table.Column<string>(type: "TEXT", nullable: false),
                    Endereco_Cidade = table.Column<string>(type: "TEXT", nullable: false),
                    Endereco_Estado = table.Column<string>(type: "TEXT", nullable: false),
                    Endereco_CEP = table.Column<string>(type: "TEXT", nullable: false),
                    Endereco_Referencia = table.Column<string>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Senha = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cliente", x => x.IdUsuario);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cliente");
        }
    }
}
