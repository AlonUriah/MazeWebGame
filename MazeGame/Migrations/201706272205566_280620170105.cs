namespace MazeGame.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _280620170105 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "Maze", c => c.String(nullable: false));
            DropColumn("dbo.Games", "MazeString");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Games", "MazeString", c => c.String(nullable: false));
            DropColumn("dbo.Games", "Maze");
        }
    }
}
