using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StretchFitnessHub.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class AddIsApproveToWalkIns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApprove",
                table: "WalkIns",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApprove",
                table: "WalkIns");
        }
    }
}
