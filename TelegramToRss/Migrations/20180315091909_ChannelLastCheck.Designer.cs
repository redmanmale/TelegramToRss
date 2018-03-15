﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Redmanmale.TelegramToRss.DAL;
using System;

namespace Redmanmale.TelegramToRss.Migrations
{
    [DbContext(typeof(GeneralDbContext))]
    [Migration("20180315091909_ChannelLastCheck")]
    partial class ChannelLastCheck
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011");

            modelBuilder.Entity("Redmanmale.TelegramToRss.DAL.Channel", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("LastCheck");

                    b.Property<long>("LastNumber");

                    b.Property<DateTime?>("LastPost");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Url")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Channels");
                });

            modelBuilder.Entity("Redmanmale.TelegramToRss.DAL.Post", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("ChannelId");

                    b.Property<string>("Header")
                        .IsRequired();

                    b.Property<long>("Number");

                    b.Property<DateTime>("PublishDate");

                    b.Property<int>("State");

                    b.Property<string>("Text")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.ToTable("BlogPosts");
                });

            modelBuilder.Entity("Redmanmale.TelegramToRss.DAL.Post", b =>
                {
                    b.HasOne("Redmanmale.TelegramToRss.DAL.Channel", "Channel")
                        .WithMany()
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
