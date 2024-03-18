using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BancoDeDados.Migrations
{
    /// <inheritdoc />
    public partial class Versao4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Endereco_Bairro",
                table: "Cliente");

            migrationBuilder.DropColumn(
                name: "Endereco_CEP",
                table: "Cliente");

            migrationBuilder.DropColumn(
                name: "Endereco_Cidade",
                table: "Cliente");

            migrationBuilder.DropColumn(
                name: "Endereco_Complemento",
                table: "Cliente");

            migrationBuilder.DropColumn(
                name: "Endereco_Estado",
                table: "Cliente");

            migrationBuilder.DropColumn(
                name: "Endereco_Logradouro",
                table: "Cliente");

            migrationBuilder.DropColumn(
                name: "Endereco_Numero",
                table: "Cliente");

            migrationBuilder.DropColumn(
                name: "Endereco_Referencia",
                table: "Cliente");

            migrationBuilder.CreateTable(
                name: "Endereco",
                columns: table => new
                {
                    ClienteModelIdUsuario = table.Column<int>(type: "INTEGER", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Logradouro = table.Column<string>(type: "TEXT", nullable: false),
                    Numero = table.Column<string>(type: "TEXT", nullable: false),
                    Complemento = table.Column<string>(type: "TEXT", nullable: false),
                    Bairro = table.Column<string>(type: "TEXT", nullable: false),
                    Cidade = table.Column<string>(type: "TEXT", nullable: false),
                    Estado = table.Column<string>(type: "TEXT", nullable: false),
                    CEP = table.Column<string>(type: "TEXT", nullable: false),
                    Referencia = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Endereco", x => new { x.ClienteModelIdUsuario, x.Id });
                    table.ForeignKey(
                        name: "FK_Endereco_Cliente_ClienteModelIdUsuario",
                        column: x => x.ClienteModelIdUsuario,
                        principalTable: "Cliente",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Endereco");

            migrationBuilder.AddColumn<string>(
                name: "Endereco_Bairro",
                table: "Cliente",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Endereco_CEP",
                table: "Cliente",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Endereco_Cidade",
                table: "Cliente",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Endereco_Complemento",
                table: "Cliente",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Endereco_Estado",
                table: "Cliente",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Endereco_Logradouro",
                table: "Cliente",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Endereco_Numero",
                table: "Cliente",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Endereco_Referencia",
                table: "Cliente",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
