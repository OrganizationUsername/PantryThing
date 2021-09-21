using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pantry.Data.Migrations
{
    public partial class LocationFoods : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocationFoods",
                columns: table => new
                {
                    LocationFoodsId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FoodId = table.Column<int>(type: "INTEGER", nullable: false),
                    LocationId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<double>(type: "REAL", nullable: false),
                    Birth = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Expiry = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Exists = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationFoods", x => x.LocationFoodsId);
                    table.ForeignKey(
                        name: "FK_LocationFoods_Foods_FoodId",
                        column: x => x.FoodId,
                        principalTable: "Foods",
                        principalColumn: "FoodId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LocationFoods_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "LocationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocationFoods_FoodId",
                table: "LocationFoods",
                column: "FoodId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationFoods_LocationId",
                table: "LocationFoods",
                column: "LocationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocationFoods");
        }
    }
}
