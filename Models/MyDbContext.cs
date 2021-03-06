﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AbcBank.Models
{
    public class MyDbContext : DbContext
    {
//        public MyDbContext(DbContextOptions<MyDbContext> options)
//            : base(options)
//        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(@"Server=127.0.0.1;Database=abc_bank;User Id=postgres;Password=postgres;");
        }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<BankBranch> BankBranches { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountHolder> AccountHolders { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Card> Cards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>()
                .HasIndex(b => b.PostCode).IsUnique();
            modelBuilder.Entity<BankBranch>()
                .HasOne(p => p.Address)
                .WithMany(p => p.BankBranches)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Card>()
                .HasOne(p => p.Account)
                .WithMany(p => p.Cards)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Person>()
                .HasOne(p => p.Address)
                .WithMany(p => p.Persons)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Person>()
                .HasOne(p => p.BankBranch)
                .WithMany(p => p.Persons)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<AccountHolder>()
                .HasOne(p => p.Account)
                .WithMany(p => p.AccountHolders)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<AccountHolder>()
                .HasOne(p => p.Person)
                .WithMany(p => p.AccountHolders)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Transaction>()
                .HasOne(p => p.Account)
                .WithMany(p => p.Transactions)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Transaction>()
                .HasOne(p => p.Person)
                .WithMany(p => p.Transactions)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<BankBranch>()
                .HasIndex(b => b.BranchName).IsUnique();
            modelBuilder.Entity<Card>()
                .HasIndex(b => b.CardNumber).IsUnique();
            modelBuilder.Entity<Person>()
                .HasIndex(b => b.Email).IsUnique();
            modelBuilder.Entity<Account>()
                .HasIndex(b => b.AccountNumber).IsUnique();
            modelBuilder.Entity<BankBranch>()
                .HasIndex(b => b.BranchName).IsUnique();
            modelBuilder.Entity<Person>()
                .HasDiscriminator<string>("Descriminator")
                .HasValue<Administrator>("Bank Personnel")
                .HasValue<Customer>("Customer");
            modelBuilder.Entity<Account>()
                .HasDiscriminator<string>("Descriminator")
                .HasValue<Savings>("Savings")
                .HasValue<Current>("Current");
        }
    }
}
