using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AbcBank.Migrations.MyDb
{
    public partial class AccountTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AccountName = table.Column<string>(nullable: false),
                    AccountNumber = table.Column<string>(nullable: false),
                    Balance = table.Column<double>(nullable: false),
                    CloseDate = table.Column<DateTime>(nullable: false),
                    DailyIn = table.Column<double>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    Descriminator = table.Column<string>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsJoint = table.Column<bool>(nullable: false),
                    SortCode = table.Column<string>(nullable: true),
                    OverDraft = table.Column<double>(nullable: true),
                    MonthlyCount = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_AccountName",
                table: "Accounts",
                column: "AccountName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
