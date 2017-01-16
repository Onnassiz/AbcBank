using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AbcBank.Migrations.MyDb
{
    public partial class AddressAndBranch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    County = table.Column<string>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    HouseNumber = table.Column<string>(nullable: false),
                    PostCode = table.Column<string>(nullable: false),
                    Street = table.Column<string>(nullable: false),
                    ToString = table.Column<string>(nullable: true),
                    Town = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BankBranches",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AddressId = table.Column<string>(nullable: true),
                    BranchName = table.Column<string>(nullable: false),
                    SortCode = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankBranches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankBranches_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_PostCode",
                table: "Addresses",
                column: "PostCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankBranches_AddressId",
                table: "BankBranches",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_BankBranches_BranchName",
                table: "BankBranches",
                column: "BranchName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankBranches");

            migrationBuilder.DropTable(
                name: "Addresses");
        }
    }
}
