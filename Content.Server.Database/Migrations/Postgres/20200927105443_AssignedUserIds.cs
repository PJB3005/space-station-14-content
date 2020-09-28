﻿﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Content.Server.Database.Migrations.Postgres
{
    public partial class AssignedUserIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssignedUserIds",
                columns: table => new
                {
                    AssignedUserIdId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserName = table.Column<string>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignedUserIds", x => x.AssignedUserIdId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssignedUserIds_UserId",
                table: "AssignedUserIds",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssignedUserIds_UserName",
                table: "AssignedUserIds",
                column: "UserName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssignedUserIds");
        }
    }
}
