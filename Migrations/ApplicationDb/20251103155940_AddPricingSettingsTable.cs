using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StretchFitnessHub.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class AddPricingSettingsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PricingSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GymOnlyPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GymWithClassPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ClassOnlyPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalkInGymPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WalkInClassPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricingSettings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PricingSettings");
        }
    }
}
