namespace Find_Your_Petrol1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Second : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PetrolStations", "Prikaz", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PetrolStations", "Prikaz");
        }
    }
}
