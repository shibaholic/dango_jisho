using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReviewV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardSenses");

            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropColumn(
                name: "NextReviewDays",
                table: "TrackedEntries");

            migrationBuilder.DropColumn(
                name: "NextReviewMinutes",
                table: "TrackedEntries");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Tags");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastReviewDate",
                table: "TrackedEntries",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldNullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "SpacedTime",
                table: "TrackedEntries",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "Tags",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "TotalKnown",
                table: "Tags",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalLearning",
                table: "Tags",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalNew",
                table: "Tags",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalReviewing",
                table: "Tags",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "AddedToTagDate",
                table: "EntryIsTagged",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValueSql: "NOW()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpacedTime",
                table: "TrackedEntries");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "TotalKnown",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "TotalLearning",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "TotalNew",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "TotalReviewing",
                table: "Tags");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastReviewDate",
                table: "TrackedEntries",
                type: "timestamp",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NextReviewDays",
                table: "TrackedEntries",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NextReviewMinutes",
                table: "TrackedEntries",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Tags",
                type: "timestamp",
                nullable: false,
                defaultValueSql: "NOW()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AddedToTagDate",
                table: "EntryIsTagged",
                type: "timestamp",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KanjiId = table.Column<int>(type: "integer", nullable: true),
                    ReadingId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ent_seq = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cards_KanjiElements_KanjiId",
                        column: x => x.KanjiId,
                        principalTable: "KanjiElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cards_ReadingElements_ReadingId",
                        column: x => x.ReadingId,
                        principalTable: "ReadingElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CardSenses",
                columns: table => new
                {
                    CardId = table.Column<int>(type: "integer", nullable: false),
                    SenseId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardSenses", x => new { x.CardId, x.SenseId });
                    table.ForeignKey(
                        name: "FK_CardSenses_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CardSenses_Senses_SenseId",
                        column: x => x.SenseId,
                        principalTable: "Senses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cards_KanjiId",
                table: "Cards",
                column: "KanjiId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_ReadingId",
                table: "Cards",
                column: "ReadingId");

            migrationBuilder.CreateIndex(
                name: "IX_CardSenses_SenseId",
                table: "CardSenses",
                column: "SenseId");
        }
    }
}
