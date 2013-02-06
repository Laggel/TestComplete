namespace TestComplete.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QueueRecurso : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Queues", "RecursoId", c => c.Int());
            AddForeignKey("dbo.Queues", "RecursoId", "dbo.Recursoes", "RecursoId");
            CreateIndex("dbo.Queues", "RecursoId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Queues", new[] { "RecursoId" });
            DropForeignKey("dbo.Queues", "RecursoId", "dbo.Recursoes");
            DropColumn("dbo.Queues", "RecursoId");
        }
    }
}
