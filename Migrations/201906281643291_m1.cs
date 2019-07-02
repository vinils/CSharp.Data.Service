namespace Data
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class m1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Data", "DecimalValue", c => c.Decimal(precision: 18, scale: 10));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Data", "DecimalValue", c => c.Decimal(precision: 18, scale: 2));
        }
    }
}
