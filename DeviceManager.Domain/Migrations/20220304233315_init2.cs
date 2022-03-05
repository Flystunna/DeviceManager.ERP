using Microsoft.EntityFrameworkCore.Migrations;

namespace DeviceManager.Domain.Migrations
{
    public partial class init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_DeviceStatus_StatusId1",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_StatusId1",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "StatusId1",
                table: "Devices");

            migrationBuilder.AlterColumn<long>(
                name: "StatusId",
                table: "Devices",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_StatusId",
                table: "Devices",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_DeviceStatus_StatusId",
                table: "Devices",
                column: "StatusId",
                principalTable: "DeviceStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_DeviceStatus_StatusId",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_StatusId",
                table: "Devices");

            migrationBuilder.AlterColumn<int>(
                name: "StatusId",
                table: "Devices",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "StatusId1",
                table: "Devices",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Devices_StatusId1",
                table: "Devices",
                column: "StatusId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_DeviceStatus_StatusId1",
                table: "Devices",
                column: "StatusId1",
                principalTable: "DeviceStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
