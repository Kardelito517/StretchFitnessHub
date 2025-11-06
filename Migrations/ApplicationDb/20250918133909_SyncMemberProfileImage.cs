using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StretchFitnessHub.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class SyncMemberProfileImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ✅ Removed DropTable("Class") kasi wala naman sa DB

            migrationBuilder.AddColumn<string>(
                name: "ProfileImagePath",
                table: "Members",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImagePath",
                table: "Members");

            // ❌ Optional: Pwede mong alisin din itong CreateTable kung wala ka talagang balak ibalik yung Class
            // Pero safe na iwanan kung sakaling kailangan sa rollback
            migrationBuilder.CreateTable(
                name: "Class",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Instructor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Class", x => x.Id);
                });
        }
    }
}
