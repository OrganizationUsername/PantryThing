using Microsoft.EntityFrameworkCore.Migrations;

namespace Pantry.Data.Migrations
{
    public partial class removeLocationsFromEquipment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipments_Locations_LocationId",
                table: "Equipments");

            migrationBuilder.DropIndex(
                name: "IX_Equipments_LocationId",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Equipments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Equipments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_LocationId",
                table: "Equipments",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipments_Locations_LocationId",
                table: "Equipments",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
