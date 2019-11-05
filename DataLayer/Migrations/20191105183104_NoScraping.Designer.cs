﻿// <auto-generated />
using System;
using DataLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DataLayer.Migrations
{
    [DbContext(typeof(LibationContext))]
    [Migration("20191105183104_NoScraping")]
    partial class NoScraping
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DataLayer.Book", b =>
                {
                    b.Property<int>("BookId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AudibleProductId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DatePublished")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsAbridged")
                        .HasColumnType("bit");

                    b.Property<int>("LengthInMinutes")
                        .HasColumnType("int");

                    b.Property<string>("PictureId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("BookId");

                    b.HasIndex("AudibleProductId");

                    b.HasIndex("CategoryId");

                    b.ToTable("Books");
                });

            modelBuilder.Entity("DataLayer.BookContributor", b =>
                {
                    b.Property<int>("BookId")
                        .HasColumnType("int");

                    b.Property<int>("ContributorId")
                        .HasColumnType("int");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<byte>("Order")
                        .HasColumnType("tinyint");

                    b.HasKey("BookId", "ContributorId", "Role");

                    b.HasIndex("BookId");

                    b.HasIndex("ContributorId");

                    b.ToTable("BookContributor");
                });

            modelBuilder.Entity("DataLayer.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AudibleCategoryId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ParentCategoryCategoryId")
                        .HasColumnType("int");

                    b.HasKey("CategoryId");

                    b.HasIndex("AudibleCategoryId");

                    b.HasIndex("ParentCategoryCategoryId");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            CategoryId = -1,
                            AudibleCategoryId = "",
                            Name = ""
                        });
                });

            modelBuilder.Entity("DataLayer.Contributor", b =>
                {
                    b.Property<int>("ContributorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AudibleAuthorId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ContributorId");

                    b.HasIndex("Name");

                    b.ToTable("Contributors");
                });

            modelBuilder.Entity("DataLayer.LibraryBook", b =>
                {
                    b.Property<int>("BookId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime2");

                    b.HasKey("BookId");

                    b.ToTable("Library");
                });

            modelBuilder.Entity("DataLayer.Series", b =>
                {
                    b.Property<int>("SeriesId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AudibleSeriesId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SeriesId");

                    b.HasIndex("AudibleSeriesId");

                    b.ToTable("Series");
                });

            modelBuilder.Entity("DataLayer.SeriesBook", b =>
                {
                    b.Property<int>("SeriesId")
                        .HasColumnType("int");

                    b.Property<int>("BookId")
                        .HasColumnType("int");

                    b.Property<float?>("Index")
                        .HasColumnType("real");

                    b.HasKey("SeriesId", "BookId");

                    b.HasIndex("BookId");

                    b.HasIndex("SeriesId");

                    b.ToTable("SeriesBook");
                });

            modelBuilder.Entity("DataLayer.Book", b =>
                {
                    b.HasOne("DataLayer.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("DataLayer.Rating", "Rating", b1 =>
                        {
                            b1.Property<int>("BookId")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int")
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                            b1.Property<float>("OverallRating")
                                .HasColumnType("real");

                            b1.Property<float>("PerformanceRating")
                                .HasColumnType("real");

                            b1.Property<float>("StoryRating")
                                .HasColumnType("real");

                            b1.HasKey("BookId");

                            b1.ToTable("Books");

                            b1.WithOwner()
                                .HasForeignKey("BookId");
                        });

                    b.OwnsMany("DataLayer.Supplement", "Supplements", b1 =>
                        {
                            b1.Property<int>("SupplementId")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int")
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                            b1.Property<int>("BookId")
                                .HasColumnType("int");

                            b1.Property<string>("Url")
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("SupplementId");

                            b1.HasIndex("BookId");

                            b1.ToTable("Supplement");

                            b1.WithOwner("Book")
                                .HasForeignKey("BookId");
                        });

                    b.OwnsOne("DataLayer.UserDefinedItem", "UserDefinedItem", b1 =>
                        {
                            b1.Property<int>("BookId")
                                .HasColumnType("int");

                            b1.Property<string>("Tags")
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("BookId");

                            b1.ToTable("UserDefinedItem");

                            b1.WithOwner("Book")
                                .HasForeignKey("BookId");

                            b1.OwnsOne("DataLayer.Rating", "Rating", b2 =>
                                {
                                    b2.Property<int>("UserDefinedItemBookId")
                                        .HasColumnType("int");

                                    b2.Property<float>("OverallRating")
                                        .HasColumnType("real");

                                    b2.Property<float>("PerformanceRating")
                                        .HasColumnType("real");

                                    b2.Property<float>("StoryRating")
                                        .HasColumnType("real");

                                    b2.HasKey("UserDefinedItemBookId");

                                    b2.ToTable("UserDefinedItem");

                                    b2.WithOwner()
                                        .HasForeignKey("UserDefinedItemBookId");
                                });
                        });
                });

            modelBuilder.Entity("DataLayer.BookContributor", b =>
                {
                    b.HasOne("DataLayer.Book", "Book")
                        .WithMany("ContributorsLink")
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataLayer.Contributor", "Contributor")
                        .WithMany("BooksLink")
                        .HasForeignKey("ContributorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataLayer.Category", b =>
                {
                    b.HasOne("DataLayer.Category", "ParentCategory")
                        .WithMany()
                        .HasForeignKey("ParentCategoryCategoryId");
                });

            modelBuilder.Entity("DataLayer.LibraryBook", b =>
                {
                    b.HasOne("DataLayer.Book", "Book")
                        .WithOne()
                        .HasForeignKey("DataLayer.LibraryBook", "BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DataLayer.SeriesBook", b =>
                {
                    b.HasOne("DataLayer.Book", "Book")
                        .WithMany("SeriesLink")
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataLayer.Series", "Series")
                        .WithMany("BooksLink")
                        .HasForeignKey("SeriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
