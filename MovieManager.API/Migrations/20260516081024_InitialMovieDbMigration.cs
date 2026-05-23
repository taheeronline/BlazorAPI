using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieManager.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialMovieDbMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, comment: "Movie title"),
                    Director = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false, comment: "Movie director name"),
                    Genre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Movie genre category"),
                    ReleaseDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, comment: "Movie release date"),
                    Rating = table.Column<double>(type: "float", nullable: false, comment: "Movie rating (0-10)"),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, comment: "Record creation timestamp"),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, comment: "Record last modification timestamp")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Director",
                table: "Movies",
                column: "Director");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Genre",
                table: "Movies",
                column: "Genre");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Title",
                table: "Movies",
                column: "Title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Movies");
        }
    }
}
