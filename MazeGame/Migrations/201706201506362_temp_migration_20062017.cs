namespace MazeGame.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class temp_migration_20062017 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Games",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Rows = c.Int(nullable: false),
                        Cols = c.Int(nullable: false),
                        MazeString = c.String(nullable: false),
                        Player1Id = c.Int(nullable: false),
                        Player2Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Games");
        }
    }
}
