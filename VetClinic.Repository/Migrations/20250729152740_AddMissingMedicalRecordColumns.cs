using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VetClinic.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingMedicalRecordColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExaminationNotes",
                table: "MedicalRecords",
                type: "ntext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FollowUpInstructions",
                table: "MedicalRecords",
                type: "ntext",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextFollowUpDate",
                table: "MedicalRecords",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExaminationNotes",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "FollowUpInstructions",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "NextFollowUpDate",
                table: "MedicalRecords");
        }
    }
}
