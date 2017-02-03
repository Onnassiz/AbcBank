using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AbcBank.Migrations.MyDb
{
    public partial class AllDatabaseEntities : Migration
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
                    DailyOut = table.Column<double>(nullable: false),
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

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AddressToString = table.Column<string>(nullable: true),
                    County = table.Column<string>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    HouseNumber = table.Column<string>(nullable: false),
                    PostCode = table.Column<string>(nullable: false),
                    Street = table.Column<string>(nullable: false),
                    Town = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AccountId = table.Column<string>(nullable: false),
                    CardName = table.Column<string>(nullable: false),
                    CardNumber = table.Column<string>(nullable: false),
                    CardPin = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cards_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AddressId = table.Column<string>(nullable: true),
                    BankBranchId = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateOfBirth = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    Descriminator = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    MaidenName = table.Column<string>(nullable: true),
                    MarritalStatus = table.Column<string>(nullable: false),
                    MiddleName = table.Column<string>(nullable: true),
                    Sex = table.Column<string>(nullable: false),
                    CustomerCount = table.Column<int>(nullable: true),
                    HireDate = table.Column<DateTime>(nullable: true),
                    BankerId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Persons_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Persons_BankBranches_BankBranchId",
                        column: x => x.BankBranchId,
                        principalTable: "BankBranches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountHolders",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AccountId = table.Column<string>(nullable: false),
                    PersonId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountHolders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountHolders_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountHolders_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AccountId = table.Column<string>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    From = table.Column<string>(nullable: true),
                    PersonId = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_AccountName",
                table: "Accounts",
                column: "AccountName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountHolders_AccountId",
                table: "AccountHolders",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountHolders_PersonId",
                table: "AccountHolders",
                column: "PersonId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Cards_AccountId",
                table: "Cards",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_CardNumber",
                table: "Cards",
                column: "CardNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_AddressId",
                table: "Persons",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_BankBranchId",
                table: "Persons",
                column: "BankBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Persons_Email",
                table: "Persons",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AccountId",
                table: "Transactions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PersonId",
                table: "Transactions",
                column: "PersonId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountHolders");

            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "BankBranches");

            migrationBuilder.DropTable(
                name: "Addresses");
        }
    }
}
