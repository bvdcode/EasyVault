using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyVault.Server.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "access_events",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ip_address = table.Column<string>(type: "TEXT", nullable: false),
                    route = table.Column<string>(type: "TEXT", nullable: false),
                    user_agent = table.Column<string>(type: "TEXT", nullable: false),
                    method = table.Column<string>(type: "TEXT", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_access_events", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vaults",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    row_version = table.Column<byte[]>(type: "BLOB", rowVersion: true, nullable: false),
                    secret_key_hash_sha512 = table.Column<string>(type: "TEXT", nullable: false),
                    hash_algorithm = table.Column<string>(type: "TEXT", nullable: false),
                    encrypted_data = table.Column<string>(type: "TEXT", nullable: false),
                    salt = table.Column<byte[]>(type: "BLOB", nullable: false),
                    created_from_ip_address = table.Column<string>(type: "TEXT", nullable: false),
                    created_from_user_agent = table.Column<string>(type: "TEXT", nullable: false),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    updated_at = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vaults", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "access_events");

            migrationBuilder.DropTable(
                name: "vaults");
        }
    }
}
