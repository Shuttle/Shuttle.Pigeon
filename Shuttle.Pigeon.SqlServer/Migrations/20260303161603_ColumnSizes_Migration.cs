using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shuttle.Pigeon.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class ColumnSizes_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SenderDisplayName",
                schema: "pigeon",
                table: "Message",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(130)",
                oldMaxLength: 130,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Sender",
                schema: "pigeon",
                table: "Message",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(130)",
                oldMaxLength: 130);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SenderDisplayName",
                schema: "pigeon",
                table: "Message",
                type: "nvarchar(130)",
                maxLength: 130,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Sender",
                schema: "pigeon",
                table: "Message",
                type: "nvarchar(130)",
                maxLength: 130,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);
        }
    }
}
