using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace Appet.Models
{
    public class Pet
    {
        [Key]
        public int id { get; set; }
        [DisplayName("Imię")]
        public string name { get; set; }
        [DisplayName("Zdjęcie")]
        public string img { get; set; }
        public virtual ICollection<Eat> Eat { get; set; }
        public virtual ICollection<Walk> Walk { get; set; }
        public virtual ICollection<Note> Note { get; set; }
        public virtual ICollection<UserPet> UserPet { get; set; }
    }

    public class Eat
    {
        [Key]
        public int id { get; set; }
        [ForeignKey("Pet")]
        public int pet_id { get; set; }
        public string user_id { get; set; }
        public string user_name { get; set; }
        public DateTime datetime { get; set; }
        public virtual Pet Pet { get; set; }
    }

    public class Walk
    {
        [Key]
        public int id { get; set; }
        [ForeignKey("Pet")]
        public int pet_id { get; set; }
        public string user_id { get; set; }
        public string user_name { get; set; }
        public DateTime datetime { get; set; }
        public virtual Pet Pet { get; set; }
    }

    public class Note
    {
        [Key]
        public int id { get; set; }
        [ForeignKey("Pet")]
        public int pet_id { get; set; }
        public string user_id { get; set; }
        public string user_name { get; set; }
        [DisplayName("Notatka")]
        public string nnote { get; set; }
        public DateTime datetime { get; set; }
        public virtual Pet Pet { get; set; }
    }

    public class UserPet
    {
        [Key]
        public int id { get; set; }
        [ForeignKey("Pet")]
        public int pet_id { get; set; }
        public string user_id { get; set; }
        [DisplayName("Nazwa użytkownika lub email")]
        public string user_name { get; set; }
        public string user_email { get; set; }
        public bool ismain { get; set; }
        public bool isaccepted { get; set; }
        public virtual Pet Pet { get; set; }
    }

    public class PetModelsDBContext : DbContext
    {
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Eat> Eats { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Walk> Walks { get; set; }
        public DbSet<UserPet> UserPets { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Candidate>().HasKey(l => l.ID);
        }
    }
}