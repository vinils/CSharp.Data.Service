using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CSharp.Data.Service.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Group",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Initials = table.Column<string>(nullable: true),
                    ParentId = table.Column<Guid>(nullable: true),
                    MeasureUnit = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Group_Group_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Data",
                columns: table => new
                {
                    GroupId = table.Column<Guid>(nullable: false),
                    CollectionDate = table.Column<DateTime>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    DecimalValue = table.Column<decimal>(type: "decimal(18,10)", nullable: true),
                    StringValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Data", x => new { x.GroupId, x.CollectionDate });
                    table.ForeignKey(
                        name: "FK_Data_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LimitDecimal",
                columns: table => new
                {
                    GroupId = table.Column<Guid>(nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    Max = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Min = table.Column<decimal>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LimitDecimal", x => new { x.GroupId, x.Priority, x.Max });
                    table.ForeignKey(
                        name: "FK_LimitDecimal_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LimitDecimal_LimitDecimal_GroupId_Priority_Min",
                        columns: x => new { x.GroupId, x.Priority, x.Min },
                        principalTable: "LimitDecimal",
                        principalColumns: new[] { "GroupId", "Priority", "Max" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LimitDecimalDenormalized",
                columns: table => new
                {
                    GroupId = table.Column<Guid>(nullable: false),
                    CollectionDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    MinType = table.Column<int>(nullable: true),
                    Min = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MaxType = table.Column<int>(nullable: true),
                    Max = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Color = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LimitDecimalDenormalized", x => new { x.GroupId, x.CollectionDate });
                    table.ForeignKey(
                        name: "FK_LimitDecimalDenormalized_Data_GroupId_CollectionDate",
                        columns: x => new { x.GroupId, x.CollectionDate },
                        principalTable: "Data",
                        principalColumns: new[] { "GroupId", "CollectionDate" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LimitStringDenormalized",
                columns: table => new
                {
                    GroupId = table.Column<Guid>(nullable: false),
                    CollectionDate = table.Column<DateTime>(nullable: false),
                    Expected = table.Column<string>(nullable: true),
                    Color = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LimitStringDenormalized", x => new { x.GroupId, x.CollectionDate });
                    table.ForeignKey(
                        name: "FK_LimitStringDenormalized_Data_GroupId_CollectionDate",
                        columns: x => new { x.GroupId, x.CollectionDate },
                        principalTable: "Data",
                        principalColumns: new[] { "GroupId", "CollectionDate" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Group_ParentId",
                table: "Group",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_LimitDecimal_GroupId_Priority_Min",
                table: "LimitDecimal",
                columns: new[] { "GroupId", "Priority", "Min" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LimitDecimal");

            migrationBuilder.DropTable(
                name: "LimitDecimalDenormalized");

            migrationBuilder.DropTable(
                name: "LimitStringDenormalized");

            migrationBuilder.DropTable(
                name: "Data");

            migrationBuilder.DropTable(
                name: "Group");
        }
    }
}
