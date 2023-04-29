using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CountryhouseService.Migrations
{
    public partial class Requests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(480)", maxLength: 480, nullable: true),
                    WorkerId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_Requests_Ads_AdId",
                        column: x => x.AdId,
                        principalTable: "Ads",
                        principalColumn: "AdId");
                    table.ForeignKey(
                        name: "FK_Requests_AspNetUsers_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });



            migrationBuilder.CreateIndex(
                name: "IX_Requests_AdId",
                table: "Requests",
                column: "AdId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_WorkerId",
                table: "Requests",
                column: "WorkerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.DropTable(
                name: "Requests");


        }
    }
}
