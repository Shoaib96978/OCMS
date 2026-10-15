using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OCMS.Migrations
{
    /// <inheritdoc />
    public partial class AddComplaintIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Complaints_Status",
                table: "Complaints",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Complaints_TrackId",
                table: "Complaints",
                column: "TrackId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Complaints_Status",
                table: "Complaints");

            migrationBuilder.DropIndex(
                name: "IX_Complaints_TrackId",
                table: "Complaints");
        }
    }
}
