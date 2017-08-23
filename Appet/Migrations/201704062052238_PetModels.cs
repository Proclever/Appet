namespace Appet.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PetModels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Eats",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        pet_id = c.Int(nullable: false),
                        user_id = c.String(),
                        user_name = c.String(),
                        datetime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Pets", t => t.pet_id, cascadeDelete: true)
                .Index(t => t.pet_id);
            
            CreateTable(
                "dbo.Pets",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        img = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Notes",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        pet_id = c.Int(nullable: false),
                        user_id = c.String(),
                        user_name = c.String(),
                        nnote = c.String(),
                        datetime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Pets", t => t.pet_id, cascadeDelete: true)
                .Index(t => t.pet_id);
            
            CreateTable(
                "dbo.UserPets",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        pet_id = c.Int(nullable: false),
                        user_id = c.String(),
                        user_name = c.String(),
                        user_email = c.String(),
                        ismain = c.Boolean(nullable: false),
                        isaccepted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Pets", t => t.pet_id, cascadeDelete: true)
                .Index(t => t.pet_id);
            
            CreateTable(
                "dbo.Walks",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        pet_id = c.Int(nullable: false),
                        user_id = c.String(),
                        user_name = c.String(),
                        datetime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Pets", t => t.pet_id, cascadeDelete: true)
                .Index(t => t.pet_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Eats", "pet_id", "dbo.Pets");
            DropForeignKey("dbo.Walks", "pet_id", "dbo.Pets");
            DropForeignKey("dbo.UserPets", "pet_id", "dbo.Pets");
            DropForeignKey("dbo.Notes", "pet_id", "dbo.Pets");
            DropIndex("dbo.Walks", new[] { "pet_id" });
            DropIndex("dbo.UserPets", new[] { "pet_id" });
            DropIndex("dbo.Notes", new[] { "pet_id" });
            DropIndex("dbo.Eats", new[] { "pet_id" });
            DropTable("dbo.Walks");
            DropTable("dbo.UserPets");
            DropTable("dbo.Notes");
            DropTable("dbo.Pets");
            DropTable("dbo.Eats");
        }
    }
}
