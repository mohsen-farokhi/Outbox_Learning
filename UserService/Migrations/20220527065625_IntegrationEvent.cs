using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.Migrations;

public partial class IntegrationEvent : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "IntegrationEventOutbox",
            columns: table => new
            {
                ID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Event = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Data = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_IntegrationEventOutbox", x => x.ID);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "IntegrationEventOutbox");
    }
}
