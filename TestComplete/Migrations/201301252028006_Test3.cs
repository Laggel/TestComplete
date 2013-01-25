namespace TestComplete.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Test3 : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.UserProfile",
            //    c => new
            //        {
            //            UserId = c.Int(nullable: false, identity: true),
            //            UserName = c.String(),
            //        })
            //    .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.Recursoes",
                c => new
                    {
                        RecursoId = c.Int(nullable: false, identity: true),
                        Descripcion = c.String(),
                    })
                .PrimaryKey(t => t.RecursoId);
            
            CreateTable(
                "dbo.RecursoUsuarios",
                c => new
                    {
                        RecursoUsuarioId = c.Int(nullable: false, identity: true),
                        RecursoId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        FechaEntrada = c.DateTime(nullable: false),
                        FechaSalida = c.DateTime(),
                        Estado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.RecursoUsuarioId)
                .ForeignKey("dbo.Recursoes", t => t.RecursoId, cascadeDelete: true)
                .ForeignKey("dbo.UserProfile", t => t.UserId, cascadeDelete: true)
                .Index(t => t.RecursoId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Queues",
                c => new
                    {
                        QueueId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        FechaEntrada = c.DateTime(nullable: false),
                        FechaSalida = c.DateTime(),
                        Estado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.QueueId)
                .ForeignKey("dbo.UserProfile", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Queues", new[] { "UserId" });
            DropIndex("dbo.RecursoUsuarios", new[] { "UserId" });
            DropIndex("dbo.RecursoUsuarios", new[] { "RecursoId" });
            DropForeignKey("dbo.Queues", "UserId", "dbo.UserProfile");
            DropForeignKey("dbo.RecursoUsuarios", "UserId", "dbo.UserProfile");
            DropForeignKey("dbo.RecursoUsuarios", "RecursoId", "dbo.Recursoes");
            DropTable("dbo.Queues");
            DropTable("dbo.RecursoUsuarios");
            DropTable("dbo.Recursoes");
            DropTable("dbo.UserProfile");
        }
    }
}
