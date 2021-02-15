using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.Registration.Database.Migrations
{
    public partial class ver0 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "registration");

            migrationBuilder.CreateTable(
                name: "registrations",
                schema: "registration",
                columns: table => new
                {
                    BrokerId = table.Column<string>(type: "text", nullable: false),
                    ClientId = table.Column<string>(type: "text", nullable: false),
                    BrandId = table.Column<string>(type: "text", nullable: true),
                    RegistrationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registrations", x => new { x.BrokerId, x.ClientId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "registrations",
                schema: "registration");
        }
    }
}
