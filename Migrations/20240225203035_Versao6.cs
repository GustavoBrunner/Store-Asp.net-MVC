using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BancoDeDados.Migrations
{
    /// <inheritdoc />
    public partial class Versao6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Cliente");

            migrationBuilder.DropColumn(
                name: "Nome",
                table: "Cliente");

            migrationBuilder.DropColumn(
                name: "Senha",
                table: "Cliente");

            migrationBuilder.AlterColumn<int>(
                name: "Estoque",
                table: "Produto",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Referencia",
                table: "Endereco",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Endereco",
                type: "char(2)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Complemento",
                table: "Endereco",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CEP",
                table: "Endereco",
                type: "char(9)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<bool>(
                name: "Selecionado",
                table: "Endereco",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Pedido",
                columns: table => new
                {
                    IdPedido = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdCliente = table.Column<int>(type: "INTEGER", nullable: false),
                    DataPedido = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DataEntrega = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ValorTotal = table.Column<double>(type: "REAL", nullable: true),
                    EnderecoEntrega_Logradouro = table.Column<string>(type: "TEXT", nullable: true),
                    EnderecoEntrega_Numero = table.Column<string>(type: "TEXT", nullable: true),
                    EnderecoEntrega_Complemento = table.Column<string>(type: "TEXT", nullable: true),
                    EnderecoEntrega_Bairro = table.Column<string>(type: "TEXT", nullable: true),
                    EnderecoEntrega_Cidade = table.Column<string>(type: "TEXT", nullable: true),
                    EnderecoEntrega_Estado = table.Column<string>(type: "char(2)", nullable: true),
                    EnderecoEntrega_CEP = table.Column<string>(type: "char(9)", nullable: true),
                    EnderecoEntrega_Referencia = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedido", x => x.IdPedido);
                    table.ForeignKey(
                        name: "FK_Pedido_Cliente_IdCliente",
                        column: x => x.IdCliente,
                        principalTable: "Cliente",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Senha = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "TEXT", nullable: true, defaultValueSql: "datetime('now', 'localtime', 'start of day')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.IdUsuario);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_IdCliente",
                table: "Pedido",
                column: "IdCliente");

            migrationBuilder.AddForeignKey(
                name: "FK_Cliente_Usuario_IdUsuario",
                table: "Cliente",
                column: "IdUsuario",
                principalTable: "Usuario",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cliente_Usuario_IdUsuario",
                table: "Cliente");

            migrationBuilder.DropTable(
                name: "Pedido");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropColumn(
                name: "Selecionado",
                table: "Endereco");

            migrationBuilder.AlterColumn<int>(
                name: "Estoque",
                table: "Produto",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Referencia",
                table: "Endereco",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Endereco",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(2)");

            migrationBuilder.AlterColumn<string>(
                name: "Complemento",
                table: "Endereco",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CEP",
                table: "Endereco",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(9)");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Cliente",
                type: "TEXT",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "Cliente",
                type: "TEXT",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Senha",
                table: "Cliente",
                type: "TEXT",
                maxLength: 128,
                nullable: false,
                defaultValue: "");
        }
    }
}
