using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shuttle.Pigeon.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "pigeon");

            migrationBuilder.CreateTable(
                name: "Messages",
                schema: "pigeon",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Channel = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Sender = table.Column<string>(type: "nvarchar(130)", maxLength: 130, nullable: false),
                    SenderDisplayName = table.Column<string>(type: "nvarchar(130)", maxLength: 130, nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    DateRegistered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateAccepted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateSent = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageAttachments",
                schema: "pigeon",
                columns: table => new
                {
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageAttachments", x => new { x.MessageId, x.Name });
                    table.ForeignKey(
                        name: "FK_MessageAttachments_Messages_MessageId",
                        column: x => x.MessageId,
                        principalSchema: "pigeon",
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageRecipients",
                schema: "pigeon",
                columns: table => new
                {
                    Identifier = table.Column<string>(type: "nvarchar(130)", maxLength: 130, nullable: false),
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageRecipients", x => new { x.MessageId, x.Identifier });
                    table.ForeignKey(
                        name: "FK_MessageRecipients_Messages_MessageId",
                        column: x => x.MessageId,
                        principalSchema: "pigeon",
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageAttachments",
                schema: "pigeon");

            migrationBuilder.DropTable(
                name: "MessageRecipients",
                schema: "pigeon");

            migrationBuilder.DropTable(
                name: "Messages",
                schema: "pigeon");
        }
    }
}
