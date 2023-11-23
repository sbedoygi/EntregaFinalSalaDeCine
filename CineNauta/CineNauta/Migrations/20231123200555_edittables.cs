using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cine_Nauta.Migrations
{
    /// <inheritdoc />
    public partial class edittables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_AspNetUsers_UserId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Functions_FunctionId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Seats_Reservations_ReservationId",
                table: "Seats");

            migrationBuilder.DropIndex(
                name: "IX_Seats_ReservationId",
                table: "Seats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reservations",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ReservationId",
                table: "Seats");

            migrationBuilder.RenameTable(
                name: "Reservations",
                newName: "Reservation");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_UserId",
                table: "Reservation",
                newName: "IX_Reservation_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_FunctionId",
                table: "Reservation",
                newName: "IX_Reservation_FunctionId");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "TemporalSales",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "FunctionId",
                table: "Reservation",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SeatCount",
                table: "Reservation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reservation",
                table: "Reservation",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_AspNetUsers_UserId",
                table: "Reservation",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_Functions_FunctionId",
                table: "Reservation",
                column: "FunctionId",
                principalTable: "Functions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_AspNetUsers_UserId",
                table: "Reservation");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_Functions_FunctionId",
                table: "Reservation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reservation",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "SeatCount",
                table: "Reservation");

            migrationBuilder.RenameTable(
                name: "Reservation",
                newName: "Reservations");

            migrationBuilder.RenameIndex(
                name: "IX_Reservation_UserId",
                table: "Reservations",
                newName: "IX_Reservations_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservation_FunctionId",
                table: "Reservations",
                newName: "IX_Reservations_FunctionId");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "TemporalSales",
                type: "int",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "ReservationId",
                table: "Seats",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FunctionId",
                table: "Reservations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reservations",
                table: "Reservations",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Seats_ReservationId",
                table: "Seats",
                column: "ReservationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_AspNetUsers_UserId",
                table: "Reservations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Functions_FunctionId",
                table: "Reservations",
                column: "FunctionId",
                principalTable: "Functions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_Reservations_ReservationId",
                table: "Seats",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "Id");
        }
    }
}
