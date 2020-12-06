namespace Find_Your_Petrol1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PetrolStations",
                c => new
                    {
                        PetrolStationId = c.Int(nullable: false, identity: true),
                        ImeNaBenzinska = c.String(),
                        RabotnoVreme = c.String(),
                        Oddalecenost = c.Int(nullable: false),
                        Ocena = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.PetrolStationId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PetrolStations");
        }
    }
}
