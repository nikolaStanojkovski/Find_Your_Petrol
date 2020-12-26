namespace Find_Your_Petrol1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PetrolStationFuelsModelAdded : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.PetrolStationFuels", newName: "PetrolStationFuels");

            
        }
        
        public override void Down()
        {

        }
    }
}
