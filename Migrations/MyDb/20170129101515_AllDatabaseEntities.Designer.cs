using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using AbcBank.Models;

namespace AbcBank.Migrations.MyDb
{
    [DbContext(typeof(MyDbContext))]
    [Migration("20170129101515_AllDatabaseEntities")]
    partial class AllDatabaseEntities
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("AbcBank.Models.Account", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AccountName")
                        .IsRequired();

                    b.Property<string>("AccountNumber")
                        .IsRequired();

                    b.Property<double>("Balance");

                    b.Property<DateTime>("CloseDate");

                    b.Property<double>("DailyIn");

                    b.Property<double>("DailyOut");

                    b.Property<DateTime>("DateCreated");

                    b.Property<string>("Descriminator")
                        .IsRequired();

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsJoint");

                    b.Property<string>("SortCode");

                    b.HasKey("Id");

                    b.HasIndex("AccountName")
                        .IsUnique();

                    b.ToTable("Accounts");

                    b.HasDiscriminator<string>("Descriminator").HasValue("Account");
                });

            modelBuilder.Entity("AbcBank.Models.AccountHolder", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AccountId")
                        .IsRequired();

                    b.Property<string>("PersonId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("PersonId");

                    b.ToTable("AccountHolders");
                });

            modelBuilder.Entity("AbcBank.Models.Address", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AddressToString");

                    b.Property<string>("County")
                        .IsRequired();

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateUpdated");

                    b.Property<string>("HouseNumber")
                        .IsRequired();

                    b.Property<string>("PostCode")
                        .IsRequired();

                    b.Property<string>("Street")
                        .IsRequired();

                    b.Property<string>("Town")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("PostCode")
                        .IsUnique();

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("AbcBank.Models.BankBranch", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AddressId");

                    b.Property<string>("BranchName")
                        .IsRequired();

                    b.Property<string>("SortCode")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("AddressId");

                    b.HasIndex("BranchName")
                        .IsUnique();

                    b.ToTable("BankBranches");
                });

            modelBuilder.Entity("AbcBank.Models.Person", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AddressId");

                    b.Property<string>("BankBranchId");

                    b.Property<DateTime>("DateCreated");

                    b.Property<DateTime>("DateOfBirth");

                    b.Property<DateTime>("DateUpdated");

                    b.Property<string>("Descriminator")
                        .IsRequired();

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("FirstName")
                        .IsRequired();

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.Property<string>("MaidenName");

                    b.Property<string>("MarritalStatus")
                        .IsRequired();

                    b.Property<string>("MiddleName");

                    b.Property<string>("Sex")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("AddressId");

                    b.HasIndex("BankBranchId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Persons");

                    b.HasDiscriminator<string>("Descriminator").HasValue("Person");
                });

            modelBuilder.Entity("AbcBank.Models.Transaction", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AccountId")
                        .IsRequired();

                    b.Property<double>("Amount");

                    b.Property<DateTime>("DateCreated");

                    b.Property<string>("Description");

                    b.Property<string>("From");

                    b.Property<string>("PersonId");

                    b.Property<string>("Type")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("PersonId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("AbcBank.Models.Current", b =>
                {
                    b.HasBaseType("AbcBank.Models.Account");

                    b.Property<double>("OverDraft");

                    b.ToTable("Current");

                    b.HasDiscriminator().HasValue("Current");
                });

            modelBuilder.Entity("AbcBank.Models.Savings", b =>
                {
                    b.HasBaseType("AbcBank.Models.Account");

                    b.Property<int>("MonthlyCount");

                    b.ToTable("Savings");

                    b.HasDiscriminator().HasValue("Savings");
                });

            modelBuilder.Entity("AbcBank.Models.Administrator", b =>
                {
                    b.HasBaseType("AbcBank.Models.Person");

                    b.Property<int>("CustomerCount");

                    b.Property<DateTime>("HireDate");

                    b.ToTable("Administrator");

                    b.HasDiscriminator().HasValue("Bank Personnel");
                });

            modelBuilder.Entity("AbcBank.Models.Customer", b =>
                {
                    b.HasBaseType("AbcBank.Models.Person");

                    b.Property<string>("BankerId");

                    b.ToTable("Customer");

                    b.HasDiscriminator().HasValue("Customer");
                });

            modelBuilder.Entity("AbcBank.Models.AccountHolder", b =>
                {
                    b.HasOne("AbcBank.Models.Account", "Account")
                        .WithMany("AccountHolders")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AbcBank.Models.Person", "Person")
                        .WithMany("AccountHolders")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AbcBank.Models.BankBranch", b =>
                {
                    b.HasOne("AbcBank.Models.Address", "Address")
                        .WithMany("BankBranches")
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AbcBank.Models.Person", b =>
                {
                    b.HasOne("AbcBank.Models.Address", "Address")
                        .WithMany("Persons")
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AbcBank.Models.BankBranch", "BankBranch")
                        .WithMany("Persons")
                        .HasForeignKey("BankBranchId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AbcBank.Models.Transaction", b =>
                {
                    b.HasOne("AbcBank.Models.Account", "Account")
                        .WithMany("Transactions")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AbcBank.Models.Person", "Person")
                        .WithMany("Transactions")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
