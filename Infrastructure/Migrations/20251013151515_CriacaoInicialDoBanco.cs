using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoInicialDoBanco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    IdUsuario = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Senha = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailVerificado = table.Column<bool>(type: "bit", nullable: false),
                    UltimoLogin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.IdUsuario);
                });

            migrationBuilder.CreateTable(
                name: "CodigosTemporarios",
                columns: table => new
                {
                    IdCodigoTemporario = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdUsuario = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoDeCodigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CodigoHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataExpiracao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataUtilizacao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodigosTemporarios", x => x.IdCodigoTemporario);
                    table.ForeignKey(
                        name: "FK_CodigosTemporarios_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Projeto",
                columns: table => new
                {
                    IdProjeto = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdUsuario = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Deletado = table.Column<bool>(type: "bit", nullable: false),
                    DataDeletado = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projeto", x => x.IdProjeto);
                    table.ForeignKey(
                        name: "FK_Projeto_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tarefa",
                columns: table => new
                {
                    IdTarefa = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdUsuario = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Concluido = table.Column<bool>(type: "bit", nullable: false),
                    Ordem = table.Column<int>(type: "int", nullable: false),
                    Deletado = table.Column<bool>(type: "bit", nullable: false),
                    DataDeletado = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarefa", x => x.IdTarefa);
                    table.ForeignKey(
                        name: "FK_Tarefa_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Anotacao",
                columns: table => new
                {
                    IdAnotacao = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Conteudo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdProjeto = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Favorito = table.Column<bool>(type: "bit", nullable: false),
                    Ordem = table.Column<int>(type: "int", nullable: false),
                    Deletado = table.Column<bool>(type: "bit", nullable: false),
                    DataDeletado = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anotacao", x => x.IdAnotacao);
                    table.ForeignKey(
                        name: "FK_Anotacao_Projeto_IdProjeto",
                        column: x => x.IdProjeto,
                        principalTable: "Projeto",
                        principalColumn: "IdProjeto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Anotacao_IdProjeto",
                table: "Anotacao",
                column: "IdProjeto");

            migrationBuilder.CreateIndex(
                name: "IX_CodigosTemporarios_IdUsuario",
                table: "CodigosTemporarios",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Projeto_IdUsuario",
                table: "Projeto",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Tarefa_IdUsuario",
                table: "Tarefa",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Email",
                table: "Usuario",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Anotacao");

            migrationBuilder.DropTable(
                name: "CodigosTemporarios");

            migrationBuilder.DropTable(
                name: "Tarefa");

            migrationBuilder.DropTable(
                name: "Projeto");

            migrationBuilder.DropTable(
                name: "Usuario");
        }
    }
}
