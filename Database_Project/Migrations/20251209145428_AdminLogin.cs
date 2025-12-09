using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database_Project.Migrations
{
    /// <inheritdoc />
    public partial class AdminLogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    AdminId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AdminUsername = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    AdminPasswordHash = table.Column<byte[]>(type: "BLOB", nullable: false),
                    AdminPasswordSalt = table.Column<byte[]>(type: "BLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.AdminId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admins_AdminUsername",
                table: "Admins",
                column: "AdminUsername",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");
        }
    }
}
