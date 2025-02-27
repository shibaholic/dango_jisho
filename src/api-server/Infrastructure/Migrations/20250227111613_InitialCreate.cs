using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Entries",
                columns: table => new
                {
                    ent_seq = table.Column<string>(type: "text", nullable: false),
                    SelectedKanjiIndex = table.Column<int>(type: "integer", nullable: true),
                    SelectedReadingIndex = table.Column<int>(type: "integer", nullable: false),
                    PriorityScore = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entries", x => x.ent_seq);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    RefreshToken = table.Column<Guid>(type: "uuid", nullable: true),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KanjiElements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ent_seq = table.Column<string>(type: "text", nullable: false),
                    keb = table.Column<string>(type: "text", nullable: false),
                    ke_inf = table.Column<List<string>>(type: "text[]", nullable: false),
                    ke_pri = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KanjiElements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KanjiElements_Entries_ent_seq",
                        column: x => x.ent_seq,
                        principalTable: "Entries",
                        principalColumn: "ent_seq",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReadingElements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ent_seq = table.Column<string>(type: "text", nullable: false),
                    reb = table.Column<string>(type: "text", nullable: false),
                    re_nokanji = table.Column<bool>(type: "boolean", nullable: false),
                    re_restr = table.Column<List<string>>(type: "text[]", nullable: false),
                    re_inf = table.Column<List<string>>(type: "text[]", nullable: false),
                    re_pri = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReadingElements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReadingElements_Entries_ent_seq",
                        column: x => x.ent_seq,
                        principalTable: "Entries",
                        principalColumn: "ent_seq",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Senses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ent_seq = table.Column<string>(type: "text", nullable: false),
                    stagk = table.Column<List<string>>(type: "text[]", nullable: false),
                    stagr = table.Column<List<string>>(type: "text[]", nullable: false),
                    pos = table.Column<List<string>>(type: "text[]", nullable: false),
                    xref = table.Column<List<string>>(type: "text[]", nullable: false),
                    ant = table.Column<List<string>>(type: "text[]", nullable: false),
                    field = table.Column<List<string>>(type: "text[]", nullable: false),
                    misc = table.Column<List<string>>(type: "text[]", nullable: false),
                    s_inf = table.Column<List<string>>(type: "text[]", nullable: false),
                    dial = table.Column<List<string>>(type: "text[]", nullable: false),
                    gloss = table.Column<List<string>>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Senses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Senses_Entries_ent_seq",
                        column: x => x.ent_seq,
                        principalTable: "Entries",
                        principalColumn: "ent_seq",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudySets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NewEntryGoal = table.Column<int>(type: "integer", nullable: false),
                    NewEntryCount = table.Column<int>(type: "integer", nullable: false),
                    NewQueue = table.Column<List<string>>(type: "text[]", nullable: false),
                    LearningQueue = table.Column<List<string>>(type: "text[]", nullable: false),
                    BaseQueue = table.Column<List<string>>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudySets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudySets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "NOW()"),
                    TotalEntries = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TrackedEntries",
                columns: table => new
                {
                    ent_seq = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LevelStateType = table.Column<string>(type: "text", nullable: false),
                    OldLevelStateType = table.Column<string>(type: "text", nullable: true),
                    SpecialCategory = table.Column<string>(type: "text", nullable: true),
                    Score = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    LastReviewDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    NextReviewDays = table.Column<int>(type: "integer", nullable: true),
                    NextReviewMinutes = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackedEntries", x => new { x.ent_seq, x.UserId });
                    table.ForeignKey(
                        name: "FK_TrackedEntries_Entries_ent_seq",
                        column: x => x.ent_seq,
                        principalTable: "Entries",
                        principalColumn: "ent_seq",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrackedEntries_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ent_seq = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    KanjiId = table.Column<int>(type: "integer", nullable: true),
                    ReadingId = table.Column<int>(type: "integer", nullable: false)
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
                name: "LSource",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SenseId = table.Column<int>(type: "integer", nullable: false),
                    LangValue = table.Column<string>(type: "text", nullable: false),
                    lang = table.Column<string>(type: "text", nullable: true),
                    ls_part = table.Column<bool>(type: "boolean", nullable: false),
                    ls_wasei = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LSource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LSource_Senses_SenseId",
                        column: x => x.SenseId,
                        principalTable: "Senses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TagInStudySets",
                columns: table => new
                {
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudySetId = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagInStudySets", x => new { x.TagId, x.StudySetId });
                    table.ForeignKey(
                        name: "FK_TagInStudySets_StudySets_StudySetId",
                        column: x => x.StudySetId,
                        principalTable: "StudySets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagInStudySets_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntryEvents",
                columns: table => new
                {
                    ent_seq = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Serial = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Created = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "NOW()"),
                    EventType = table.Column<int>(type: "integer", nullable: false),
                    ReviewValue = table.Column<string>(type: "text", nullable: true),
                    ChangeValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntryEvents", x => new { x.ent_seq, x.UserId, x.Serial });
                    table.ForeignKey(
                        name: "FK_EntryEvents_TrackedEntries_ent_seq_UserId",
                        columns: x => new { x.ent_seq, x.UserId },
                        principalTable: "TrackedEntries",
                        principalColumns: new[] { "ent_seq", "UserId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntryIsTagged",
                columns: table => new
                {
                    ent_seq = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    AddedToTagDate = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "NOW()"),
                    UserOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntryIsTagged", x => new { x.ent_seq, x.UserId, x.TagId });
                    table.ForeignKey(
                        name: "FK_EntryIsTagged_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntryIsTagged_TrackedEntries_ent_seq_UserId",
                        columns: x => new { x.ent_seq, x.UserId },
                        principalTable: "TrackedEntries",
                        principalColumns: new[] { "ent_seq", "UserId" },
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateIndex(
                name: "IX_EntryIsTagged_TagId",
                table: "EntryIsTagged",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_KanjiElements_ent_seq",
                table: "KanjiElements",
                column: "ent_seq");

            migrationBuilder.CreateIndex(
                name: "IX_LSource_SenseId",
                table: "LSource",
                column: "SenseId");

            migrationBuilder.CreateIndex(
                name: "IX_ReadingElements_ent_seq",
                table: "ReadingElements",
                column: "ent_seq");

            migrationBuilder.CreateIndex(
                name: "IX_Senses_ent_seq",
                table: "Senses",
                column: "ent_seq");

            migrationBuilder.CreateIndex(
                name: "IX_StudySets_UserId",
                table: "StudySets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TagInStudySets_StudySetId",
                table: "TagInStudySets",
                column: "StudySetId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_UserId",
                table: "Tags",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackedEntries_UserId",
                table: "TrackedEntries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardSenses");

            migrationBuilder.DropTable(
                name: "EntryEvents");

            migrationBuilder.DropTable(
                name: "EntryIsTagged");

            migrationBuilder.DropTable(
                name: "LSource");

            migrationBuilder.DropTable(
                name: "TagInStudySets");

            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "TrackedEntries");

            migrationBuilder.DropTable(
                name: "Senses");

            migrationBuilder.DropTable(
                name: "StudySets");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "KanjiElements");

            migrationBuilder.DropTable(
                name: "ReadingElements");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Entries");
        }
    }
}
