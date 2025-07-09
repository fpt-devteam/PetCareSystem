using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VetClinic.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddPetExtendedProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Pets",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Pets",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Pets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MedicalNotes",
                table: "Pets",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MicrochipId",
                table: "Pets",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "MedicalNotes",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "MicrochipId",
                table: "Pets");
        }
    }
}
