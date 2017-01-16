using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using AbcBank.Models;

namespace AbcBank.Migrations.MyDb
{
    [DbContext(typeof(MyDbContext))]
    [Migration("20170115183502_PersonTable")]
    partial class PersonTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("AbcBank.Models.Address", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

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

                    b.Property<string>("ToString");

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

            modelBuilder.Entity("AbcBank.Models.Administrator", b =>
                {
                    b.HasBaseType("AbcBank.Models.Person");

                    b.Property<string>("AdminType");

                    b.Property<DateTime>("HireDate");

                    b.ToTable("Administrator");

                    b.HasDiscriminator().HasValue("Bank Personnel");
                });

            modelBuilder.Entity("AbcBank.Models.Customer", b =>
                {
                    b.HasBaseType("AbcBank.Models.Person");

                    b.Property<string>("BankerId");

                    b.Property<string>("TellerId");

                    b.ToTable("Customer");

                    b.HasDiscriminator().HasValue("Customer");
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
        }
    }
}
