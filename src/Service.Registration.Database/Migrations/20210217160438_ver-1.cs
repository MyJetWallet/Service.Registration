using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.Registration.Database.Migrations
{
    public partial class ver1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_registrations",
                schema: "registration",
                table: "registrations");

            migrationBuilder.AlterColumn<string>(
                name: "BrokerId",
                schema: "registration",
                table: "registrations",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_registrations",
                schema: "registration",
                table: "registrations",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX-registration-Registrations-BrokerId-ClientId",
                schema: "registration",
                table: "registrations",
                columns: new[] { "BrokerId", "ClientId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_registrations",
                schema: "registration",
                table: "registrations");

            migrationBuilder.DropIndex(
                name: "IX-registration-Registrations-BrokerId-ClientId",
                schema: "registration",
                table: "registrations");

            migrationBuilder.AlterColumn<string>(
                name: "BrokerId",
                schema: "registration",
                table: "registrations",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_registrations",
                schema: "registration",
                table: "registrations",
                columns: new[] { "BrokerId", "ClientId" });
        }
    }
}
