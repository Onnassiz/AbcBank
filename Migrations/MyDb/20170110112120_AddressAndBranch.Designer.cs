using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using AbcBank.Models;

namespace AbcBank.Migrations.MyDb
{
    [DbContext(typeof(MyDbContext))]
    [Migration("20170110112120_AddressAndBranch")]
    partial class AddressAndBranch
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

            modelBuilder.Entity("AbcBank.Models.BankBranch", b =>
                {
                    b.HasOne("AbcBank.Models.Address", "Address")
                        .WithMany("BankBranches")
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
