using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetingManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class Initialmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MeetingRoom",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingRoom", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Initials = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MeetingName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EndTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ReservationOwnerId = table.Column<int>(type: "int", nullable: false),
                    MeetingRoomId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reservations_MeetingRoom_MeetingRoomId",
                        column: x => x.MeetingRoomId,
                        principalTable: "MeetingRoom",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservations_Users_ReservationOwnerId",
                        column: x => x.ReservationOwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MeetingParticipants",
                columns: table => new
                {
                    ReservationId = table.Column<int>(type: "int", nullable: false),
                    ParticipantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeetingParticipants", x => new { x.ReservationId, x.ParticipantId });
                    table.ForeignKey(
                        name: "FK_MeetingParticipant_User",
                        column: x => x.ParticipantId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeetingParticipants_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeetingParticipants_ParticipantId",
                table: "MeetingParticipants",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_MeetingRoomId",
                table: "Reservations",
                column: "MeetingRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ReservationOwnerId",
                table: "Reservations",
                column: "ReservationOwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeetingParticipants");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "MeetingRoom");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
