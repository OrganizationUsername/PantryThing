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
                name: "RecipeHierarchy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Instruction = table.Column<string>(type: "TEXT", nullable: true),
                    TimeCost = table.Column<double>(type: "REAL", nullable: false),
                    RecipeHierarchyId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeHierarchy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecipeHierarchy_RecipeHierarchy_RecipeHierarchyId",
                        column: x => x.RecipeHierarchyId,
                        principalTable: "RecipeHierarchy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BetterRecipes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MainOutputFoodId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BetterRecipes", x => x.Id);
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
                    FoodTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Amount = table.Column<double>(type: "REAL", nullable: false),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BetterRecipeId = table.Column<int>(type: "INTEGER", nullable: true),
                    BetterRecipeId1 = table.Column<int>(type: "INTEGER", nullable: true),
                    RecipeId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodInstances_BetterRecipes_BetterRecipeId",
                        column: x => x.BetterRecipeId,
                        principalTable: "BetterRecipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FoodInstances_BetterRecipes_BetterRecipeId1",
                        column: x => x.BetterRecipeId1,
                        principalTable: "BetterRecipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FoodInstances_Foods_FoodTypeId",
                        column: x => x.FoodTypeId,
                        principalTable: "Foods",
                        principalColumn: "FoodId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Recipe",
                columns: table => new
                {
                    RecipeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    OutputFoodInstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    InputFoodInstanceId = table.Column<int>(type: "INTEGER", nullable: false),
                    RecipeStepsId = table.Column<int>(type: "INTEGER", nullable: false),
                    RecipeHierarchyId = table.Column<int>(type: "INTEGER", nullable: true),
                    TimeCost = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipe", x => x.RecipeId);
                    table.ForeignKey(
                        name: "FK_Recipe_FoodInstances_OutputFoodInstanceId",
                        column: x => x.OutputFoodInstanceId,
                        principalTable: "FoodInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Recipe_RecipeHierarchy_RecipeHierarchyId",
                        column: x => x.RecipeHierarchyId,
                        principalTable: "RecipeHierarchy",
                        principalColumn: "Id",
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
                    BetterRecipeId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeStep", x => x.RecipeStepId);
                    table.ForeignKey(
                        name: "FK_RecipeStep_BetterRecipes_BetterRecipeId",
                        column: x => x.BetterRecipeId,
                        principalTable: "BetterRecipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecipeStep_Recipe_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipe",
                        principalColumn: "RecipeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Equipment",
                columns: table => new
                {
                    EquipmentId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    RecipeHierarchyId = table.Column<int>(type: "INTEGER", nullable: true),
                    RecipeStepId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipment", x => x.EquipmentId);
                    table.ForeignKey(
                        name: "FK_Equipment_RecipeHierarchy_RecipeHierarchyId",
                        column: x => x.RecipeHierarchyId,
                        principalTable: "RecipeHierarchy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Equipment_RecipeStep_RecipeStepId",
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
                name: "IX_Equipment_RecipeHierarchyId",
                table: "Equipment",
                column: "RecipeHierarchyId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_RecipeStepId",
                table: "Equipment",
                column: "RecipeStepId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodInstances_BetterRecipeId",
                table: "FoodInstances",
                column: "BetterRecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodInstances_BetterRecipeId1",
                table: "FoodInstances",
                column: "BetterRecipeId1");

            migrationBuilder.CreateIndex(
                name: "IX_FoodInstances_FoodTypeId",
                table: "FoodInstances",
                column: "FoodTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodInstances_RecipeId",
                table: "FoodInstances",
                column: "RecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_Recipe_OutputFoodInstanceId",
                table: "Recipe",
                column: "OutputFoodInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_Recipe_RecipeHierarchyId",
                table: "Recipe",
                column: "RecipeHierarchyId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeHierarchy_RecipeHierarchyId",
                table: "RecipeHierarchy",
                column: "RecipeHierarchyId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeStep_BetterRecipeId",
                table: "RecipeStep",
                column: "BetterRecipeId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeStep_RecipeId",
                table: "RecipeStep",
                column: "RecipeId");

            migrationBuilder.AddForeignKey(
                name: "FK_FoodInstances_Recipe_RecipeId",
                table: "FoodInstances",
                column: "RecipeId",
                principalTable: "Recipe",
                principalColumn: "RecipeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BetterRecipes_Foods_MainOutputFoodId",
                table: "BetterRecipes");

            migrationBuilder.DropForeignKey(
                name: "FK_FoodInstances_Foods_FoodTypeId",
                table: "FoodInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_Recipe_RecipeHierarchy_RecipeHierarchyId",
                table: "Recipe");

            migrationBuilder.DropForeignKey(
                name: "FK_FoodInstances_BetterRecipes_BetterRecipeId",
                table: "FoodInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_FoodInstances_BetterRecipes_BetterRecipeId1",
                table: "FoodInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_FoodInstances_Recipe_RecipeId",
                table: "FoodInstances");

            migrationBuilder.DropTable(
                name: "Equipment");

            migrationBuilder.DropTable(
                name: "RecipeStep");

            migrationBuilder.DropTable(
                name: "Foods");

            migrationBuilder.DropTable(
                name: "RecipeHierarchy");

            migrationBuilder.DropTable(
                name: "BetterRecipes");

            migrationBuilder.DropTable(
                name: "Recipe");

            migrationBuilder.DropTable(
                name: "FoodInstances");
        }
    }
}
