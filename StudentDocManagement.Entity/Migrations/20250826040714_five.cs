using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StudentDocManagement.Entity.Migrations
{
    /// <inheritdoc />
    public partial class five : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_StatusMasters_AccountStatusId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Student_StatusMasters_ApplicationStatusId",
                table: "Student");

            migrationBuilder.DeleteData(
                table: "StatusMasters",
                keyColumn: "StatusId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "StatusMasters",
                keyColumn: "StatusId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "StatusMasters",
                keyColumn: "StatusId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "StatusMasters",
                keyColumn: "StatusId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "StatusMasters",
                keyColumn: "StatusId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "StatusMasters",
                keyColumn: "StatusId",
                keyValue: 10);

            migrationBuilder.DropColumn(
                name: "StatusType",
                table: "StatusMasters");

            migrationBuilder.RenameColumn(
                name: "ApplicationStatusId",
                table: "Student",
                newName: "StatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Student_ApplicationStatusId",
                table: "Student",
                newName: "IX_Student_StatusId");

            migrationBuilder.RenameColumn(
                name: "AccountStatusId",
                table: "AspNetUsers",
                newName: "StatusId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_AccountStatusId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_StatusId");

            migrationBuilder.UpdateData(
                table: "StatusMasters",
                keyColumn: "StatusId",
                keyValue: 4,
                column: "StatusName",
                value: "Changes Needed");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_StatusMasters_StatusId",
                table: "AspNetUsers",
                column: "StatusId",
                principalTable: "StatusMasters",
                principalColumn: "StatusId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Student_StatusMasters_StatusId",
                table: "Student",
                column: "StatusId",
                principalTable: "StatusMasters",
                principalColumn: "StatusId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_StatusMasters_StatusId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Student_StatusMasters_StatusId",
                table: "Student");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "Student",
                newName: "ApplicationStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Student_StatusId",
                table: "Student",
                newName: "IX_Student_ApplicationStatusId");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "AspNetUsers",
                newName: "AccountStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_StatusId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_AccountStatusId");

            migrationBuilder.AddColumn<string>(
                name: "StatusType",
                table: "StatusMasters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "StatusMasters",
                keyColumn: "StatusId",
                keyValue: 1,
                column: "StatusType",
                value: "RegistrationStatus");

            migrationBuilder.UpdateData(
                table: "StatusMasters",
                keyColumn: "StatusId",
                keyValue: 2,
                column: "StatusType",
                value: "RegistrationStatus");

            migrationBuilder.UpdateData(
                table: "StatusMasters",
                keyColumn: "StatusId",
                keyValue: 3,
                column: "StatusType",
                value: "RegistrationStatus");

            migrationBuilder.UpdateData(
                table: "StatusMasters",
                keyColumn: "StatusId",
                keyValue: 4,
                columns: new[] { "StatusName", "StatusType" },
                values: new object[] { "Pending", "ProfileStatus" });

            migrationBuilder.InsertData(
                table: "StatusMasters",
                columns: new[] { "StatusId", "StatusName", "StatusType" },
                values: new object[,]
                {
                    { 5, "Approved", "ProfileStatus" },
                    { 6, "Changes Needed", "ProfileStatus" },
                    { 7, "Rejected", "ProfileStatus" },
                    { 8, "Pending", "DocumentStatus" },
                    { 9, "Approved", "DocumentStatus" },
                    { 10, "Rejected", "DocumentStatus" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_StatusMasters_AccountStatusId",
                table: "AspNetUsers",
                column: "AccountStatusId",
                principalTable: "StatusMasters",
                principalColumn: "StatusId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Student_StatusMasters_ApplicationStatusId",
                table: "Student",
                column: "ApplicationStatusId",
                principalTable: "StatusMasters",
                principalColumn: "StatusId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
