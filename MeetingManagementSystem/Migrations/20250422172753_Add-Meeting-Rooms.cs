using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetingManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddMeetingRooms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_MeetingRoom_MeetingRoomId",
                table: "Reservations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MeetingRoom",
                table: "MeetingRoom");

            migrationBuilder.RenameTable(
                name: "MeetingRoom",
                newName: "MeetingRooms");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MeetingRooms",
                table: "MeetingRooms",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_MeetingRooms_MeetingRoomId",
                table: "Reservations",
                column: "MeetingRoomId",
                principalTable: "MeetingRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_MeetingRooms_MeetingRoomId",
                table: "Reservations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MeetingRooms",
                table: "MeetingRooms");

            migrationBuilder.RenameTable(
                name: "MeetingRooms",
                newName: "MeetingRoom");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MeetingRoom",
                table: "MeetingRoom",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_MeetingRoom_MeetingRoomId",
                table: "Reservations",
                column: "MeetingRoomId",
                principalTable: "MeetingRoom",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
