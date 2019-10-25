﻿// <auto-generated />
using ImageGallery.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace ImageGallery.API.Migrations
{
    [DbContext(typeof(GalleryContext))]
    [Migration("20191025142440_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ImageGallery.API.Entities.Image", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("OwnerId")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(150);

                    b.HasKey("Id");

                    b.ToTable("Images");
                });
#pragma warning restore 612, 618
        }
    }
}
