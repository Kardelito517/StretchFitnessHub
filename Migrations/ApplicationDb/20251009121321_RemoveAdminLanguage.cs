using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StretchFitnessHub.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class RemoveAdminLanguage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Language",
                table: "Admins",
                newName: "Email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Admins",
                newName: "Language");
        }
    }
}
