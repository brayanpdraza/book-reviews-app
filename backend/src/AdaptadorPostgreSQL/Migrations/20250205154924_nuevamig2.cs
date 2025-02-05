using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdaptadorPostgreSQL.Migrations
{
    public partial class nuevamig2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
           name: "FotoPerfil",
           table: "Usuarios",
           nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Eliminar el campo FotoPerfil de la tabla Usuarios en caso de revertir la migración
            migrationBuilder.DropColumn(
                name: "FotoPerfil",
                table: "Usuarios");
        }
    }
}
