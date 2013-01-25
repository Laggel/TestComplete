namespace TestComplete.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Test2 : DbMigration
    {
        public override void Up()
        {
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
        }
    }
}
