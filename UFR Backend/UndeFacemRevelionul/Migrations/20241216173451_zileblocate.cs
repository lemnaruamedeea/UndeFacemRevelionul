﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UndeFacemRevelionul.Migrations
{
    public partial class zileblocate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BlockedUntil",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockedUntil",
                table: "Users");
        }
    }
}
