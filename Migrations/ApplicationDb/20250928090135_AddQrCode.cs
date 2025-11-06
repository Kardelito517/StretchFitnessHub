using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StretchFitnessHub.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class AddQrCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "QrCodeImage",
                table: "Members",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QrCodeValue",
                table: "Members",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QrCodeImage",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "QrCodeValue",
                table: "Members");
        }
    }
}
