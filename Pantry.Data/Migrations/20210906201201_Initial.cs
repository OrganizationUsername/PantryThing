using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pantry.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Foods",
                columns: table => new
                {
                    FoodId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Foods", x => x.FoodId);
                });

            migrationBuilder.CreateTable(
                name: "BetterRecipes",
                columns: table => new
                {
                    RecipeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MainOutputFoodId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BetterRecipes", x => x.RecipeId);
                    table.ForeignKey(
                        name: "FK_BetterRecipes_Foods_MainOutputFoodId",
                        column: x => x.MainOutputFoodId,
                        principalTable: "Foods",
                        principalColumn: "FoodId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FoodInstances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FoodTypeFoodId = table.Column<int>(type: "INTEGER", nullable: true),
                    Amount = table.Column<double>(type: "REAL", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BetterRecipeRecipeId = table.Column<int>(type: "INTEGER", nullable: true),
                    BetterRecipeRecipeId1 = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodInstances_BetterRecipes_BetterRecipeRecipeId",
                        column: x => x.BetterRecipeRecipeId,
                        principalTable: "BetterRecipes",
                        principalColumn: "RecipeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FoodInstances_BetterRecipes_BetterRecipeRecipeId1",
                        column: x => x.BetterRecipeRecipeId1,
                        principalTable: "BetterRecipes",
                        principalColumn: "RecipeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FoodInstances_Foods_FoodTypeFoodId",
                        column: x => x.FoodTypeFoodId,
                        principalTable: "Foods",
                        principalColumn: "FoodId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecipeStep",
                columns: table => new
                {
                    RecipeStepId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RecipeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    Instruction = table.Column<string>(type: "TEXT", nullable: true),
                    TimeCost = table.Column<double>(type: "REAL", nullable: false),
                    BetterRecipeRecipeId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeStep", x => x.RecipeStepId);
                    table.ForeignKey(
                        name: "FK_RecipeStep_BetterRecipes_BetterRecipeRecipeId",
                        column: x => x.BetterRecipeRecipeId,
                        principalTable: "BetterRecipes",
                        principalColumn: "RecipeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Equipments",
                columns: table => new
                {
                    EquipmentId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    RecipeStepId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipments", x => x.EquipmentId);
                    table.ForeignKey(
                        name: "FK_Equipments_RecipeStep_RecipeStepId",
                        column: x => x.RecipeStepId,
                        principalTable: "RecipeStep",
                        principalColumn: "RecipeStepId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BetterRecipes_MainOutputFoodId",
                table: "BetterRecipes",
                column: "MainOutputFoodId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipments_RecipeStepId",
                table: "Equipments",
                column: "RecipeStepId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodInstances_BetterRecipeRecipeId",
                table: "FoodInstances",
                column: "BetterRecipeRecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodInstances_BetterRecipeRecipeId1",
                table: "FoodInstances",
                column: "BetterRecipeRecipeId1");

            migrationBuilder.CreateIndex(
                name: "IX_FoodInstances_FoodTypeFoodId",
                table: "FoodInstances",
                column: "FoodTypeFoodId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeStep_BetterRecipeRecipeId",
                table: "RecipeStep",
                column: "BetterRecipeRecipeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Equipments");

            migrationBuilder.DropTable(
                name: "FoodInstances");

            migrationBuilder.DropTable(
                name: "RecipeStep");

            migrationBuilder.DropTable(
                name: "BetterRecipes");

            migrationBuilder.DropTable(
                name: "Foods");
        }
    }
}
