using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shuttle.Pigeon.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class MessageParameter_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessageParameter",
                schema: "pigeon",
                columns: table => new
                {
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(130)", maxLength: 130, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageParameter", x => new { x.MessageId, x.Name });
                    table.ForeignKey(
                        name: "FK_MessageParameter_Message_MessageId",
                        column: x => x.MessageId,
                        principalSchema: "pigeon",
                        principalTable: "Message",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageParameter",
                schema: "pigeon");
        }
    }
}
