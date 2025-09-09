using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentDocManagement.Entity.Migrations
{
    /// <inheritdoc />
    public partial class six : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentEducations_Documents_DocumentId",
                table: "StudentEducations");

            migrationBuilder.DropIndex(
                name: "IX_StudentEducations_DocumentId",
                table: "StudentEducations");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "StudentEducations");

            migrationBuilder.AlterColumn<string>(
                name: "RegisterNo",
                table: "Student",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "RegisterNo",
                table: "AspNetUsers",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "CourseId",
                keyValue: 3,
                column: "CourseName",
                value: "Electronics and Communication Engineering");

            migrationBuilder.InsertData(
                table: "StatusMasters",
                columns: new[] { "StatusId", "StatusName" },
                values: new object[] { 5, "In Review" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "StatusMasters",
                keyColumn: "StatusId",
                keyValue: 5);

            migrationBuilder.AddColumn<int>(
                name: "DocumentId",
                table: "StudentEducations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "RegisterNo",
                table: "Student",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8);

            migrationBuilder.AlterColumn<string>(
                name: "RegisterNo",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8);

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "CourseId",
                keyValue: 3,
                column: "CourseName",
                value: "Electronics and Communication");

            migrationBuilder.CreateIndex(
                name: "IX_StudentEducations_DocumentId",
                table: "StudentEducations",
                column: "DocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentEducations_Documents_DocumentId",
                table: "StudentEducations",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "DocumentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
