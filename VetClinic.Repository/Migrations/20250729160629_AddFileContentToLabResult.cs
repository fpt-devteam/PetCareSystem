using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VetClinic.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddFileContentToLabResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "FileContent",
                table: "LabResults",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileContent",
                table: "LabResults");
        }
    }
}
