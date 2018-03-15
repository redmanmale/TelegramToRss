using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Redmanmale.TelegramToRss.Migrations
{
    public partial class ChannelLastCheck : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastUpdate",
                table: "Channels",
                newName: "LastCheck");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastPost",
                table: "Channels",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastPost",
                table: "Channels");

            migrationBuilder.RenameColumn(
                name: "LastCheck",
                table: "Channels",
                newName: "LastUpdate");
        }
    }
}
