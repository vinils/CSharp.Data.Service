namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class First : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Exam",
                c => new
                    {
                        GroupId = c.Guid(nullable: false),
                        CollectionDate = c.DateTime(nullable: false),
                        DecimalValue = c.Decimal(precision: 18, scale: 2),
                        StringValue = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.GroupId, t.CollectionDate })
                .ForeignKey("dbo.Group", t => t.GroupId, cascadeDelete: true)
                .Index(t => t.GroupId);
            
            CreateTable(
                "dbo.Group",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false),
                        Initials = c.String(),
                        ParentId = c.Guid(),
                        MeasureUnit = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Group", t => t.ParentId)
                .Index(t => t.ParentId);
            
            CreateTable(
                "dbo.LimitDecimal",
                c => new
                    {
                        GroupId = c.Guid(nullable: false),
                        Priority = c.Int(nullable: false),
                        Max = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Min = c.Decimal(precision: 18, scale: 2),
                        Name = c.String(),
                    })
                .PrimaryKey(t => new { t.GroupId, t.Priority, t.Max })
                .ForeignKey("dbo.Group", t => t.GroupId, cascadeDelete: true)
                .ForeignKey("dbo.LimitDecimal", t => new { t.GroupId, t.Priority, t.Min })
                .Index(t => t.GroupId)
                .Index(t => new { t.GroupId, t.Priority, t.Min });
            
            CreateTable(
                "dbo.LimitDecimalDenormalized",
                c => new
                    {
                        GroupId = c.Guid(nullable: false),
                        CollectionDate = c.DateTime(nullable: false),
                        Name = c.String(),
                        MinType = c.Int(),
                        Min = c.Decimal(precision: 18, scale: 2),
                        MaxType = c.Int(),
                        Max = c.Decimal(precision: 18, scale: 2),
                        Color = c.Int(),
                    })
                .PrimaryKey(t => new { t.GroupId, t.CollectionDate })
                .ForeignKey("dbo.Exam", t => new { t.GroupId, t.CollectionDate })
                .Index(t => new { t.GroupId, t.CollectionDate });
            
            CreateTable(
                "dbo.LimitStringDenormalized",
                c => new
                    {
                        GroupId = c.Guid(nullable: false),
                        CollectionDate = c.DateTime(nullable: false),
                        Expected = c.String(),
                        Color = c.Int(),
                    })
                .PrimaryKey(t => new { t.GroupId, t.CollectionDate })
                .ForeignKey("dbo.Exam", t => new { t.GroupId, t.CollectionDate })
                .Index(t => new { t.GroupId, t.CollectionDate });
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LimitStringDenormalized", new[] { "GroupId", "CollectionDate" }, "dbo.Exam");
            DropForeignKey("dbo.LimitDecimalDenormalized", new[] { "GroupId", "CollectionDate" }, "dbo.Exam");
            DropForeignKey("dbo.Group", "ParentId", "dbo.Group");
            DropForeignKey("dbo.LimitDecimal", new[] { "GroupId", "Priority", "Min" }, "dbo.LimitDecimal");
            DropForeignKey("dbo.LimitDecimal", "GroupId", "dbo.Group");
            DropForeignKey("dbo.Exam", "GroupId", "dbo.Group");
            DropIndex("dbo.LimitStringDenormalized", new[] { "GroupId", "CollectionDate" });
            DropIndex("dbo.LimitDecimalDenormalized", new[] { "GroupId", "CollectionDate" });
            DropIndex("dbo.LimitDecimal", new[] { "GroupId", "Priority", "Min" });
            DropIndex("dbo.LimitDecimal", new[] { "GroupId" });
            DropIndex("dbo.Group", new[] { "ParentId" });
            DropIndex("dbo.Exam", new[] { "GroupId" });
            DropTable("dbo.LimitStringDenormalized");
            DropTable("dbo.LimitDecimalDenormalized");
            DropTable("dbo.LimitDecimal");
            DropTable("dbo.Group");
            DropTable("dbo.Exam");
        }
    }
}
