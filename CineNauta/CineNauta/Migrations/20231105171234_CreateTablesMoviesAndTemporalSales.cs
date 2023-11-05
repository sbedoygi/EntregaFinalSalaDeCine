using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cine_Nauta.Migrations
{
    /// <inheritdoc />
    public partial class CreateTablesMoviesAndTemporalSales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MovieId",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MovieId",
                table: "Functions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Director = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LaunchYear = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    GenderId = table.Column<int>(type: "int", nullable: false),
                    ClassificationId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Movies_Classifications_ClassificationId",
                        column: x => x.ClassificationId,
                        principalTable: "Classifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Movies_Genders_GenderId",
                        column: x => x.GenderId,
                        principalTable: "Genders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemporalSales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FunctionId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<float>(type: "real", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemporalSales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemporalSales_Functions_FunctionId",
                        column: x => x.FunctionId,
                        principalTable: "Functions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_MovieId",
                table: "Rooms",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Functions_MovieId",
                table: "Functions",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_ClassificationId",
                table: "Movies",
                column: "ClassificationId");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_GenderId",
                table: "Movies",
                column: "GenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Title",
                table: "Movies",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TemporalSales_FunctionId",
                table: "TemporalSales",
                column: "FunctionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Functions_Movies_MovieId",
                table: "Functions",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Movies_MovieId",
                table: "Rooms",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Functions_Movies_MovieId",
                table: "Functions");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Movies_MovieId",
                table: "Rooms");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "TemporalSales");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_MovieId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Functions_MovieId",
                table: "Functions");

            migrationBuilder.DropColumn(
                name: "MovieId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "MovieId",
                table: "Functions");
        }
    }
}
