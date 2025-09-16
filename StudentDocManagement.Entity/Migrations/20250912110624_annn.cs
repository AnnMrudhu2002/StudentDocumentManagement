using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentDocManagement.Entity.Migrations
{
    /// <inheritdoc />
    public partial class annn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "states",
                columns: table => new
                {
                    StateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StateName = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_states", x => x.StateId);
                });

            migrationBuilder.CreateTable(
                name: "districts",
                columns: table => new
                {
                    DistrictId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DistrictName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StateId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_districts", x => x.DistrictId);
                    table.ForeignKey(
                        name: "FK_districts_states_StateId",
                        column: x => x.StateId,
                        principalTable: "states",
                        principalColumn: "StateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pincodes",
                columns: table => new
                {
                    PincodeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DistrictId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pincodes", x => x.PincodeId);
                    table.ForeignKey(
                        name: "FK_pincodes_districts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "districts",
                        principalColumn: "DistrictId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "postOffices",
                columns: table => new
                {
                    OfficeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfficeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PincodeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_postOffices", x => x.OfficeId);
                    table.ForeignKey(
                        name: "FK_postOffices_pincodes_PincodeId",
                        column: x => x.PincodeId,
                        principalTable: "pincodes",
                        principalColumn: "PincodeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_districts_StateId_DistrictName",
                table: "districts",
                columns: new[] { "StateId", "DistrictName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pincodes_DistrictId_PincodeId",
                table: "pincodes",
                columns: new[] { "DistrictId", "PincodeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_postOffices_PincodeId",
                table: "postOffices",
                column: "PincodeId");

            migrationBuilder.CreateIndex(
                name: "IX_states_StateName",
                table: "states",
                column: "StateName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "postOffices");

            migrationBuilder.DropTable(
                name: "pincodes");

            migrationBuilder.DropTable(
                name: "districts");

            migrationBuilder.DropTable(
                name: "states");
        }
    }
}
