using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using WebAPI.Net6.BaseRepositoryGenerics.Domain.Models;

namespace WebAPI.Net6.BaseRepositoryGenerics.Repositories
{
    public partial class MyContext : DbContext
    {
        public MyContext()
        {
        }

        public MyContext(DbContextOptions<MyContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TErrorMessage> TErrorMessages { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("Chinese_PRC_CI_AS");

            modelBuilder.Entity<TErrorMessage>(entity =>
            {
                entity.ToTable("T_ErrorMessage");

                entity.Property(e => e.Id).HasComment("自增id");

                entity.Property(e => e.Cnoutput)
                    .HasMaxLength(200)
                    .HasColumnName("CNOutput")
                    .HasComment("中文输出");

                entity.Property(e => e.Enoutput)
                    .HasMaxLength(200)
                    .HasColumnName("ENOutput")
                    .HasComment("英文输出");

                entity.Property(e => e.IsEnabled).HasComment("是否启用(0:禁用, 1:启用)");

                entity.Property(e => e.Keywords)
                    .HasMaxLength(200)
                    .HasComment("错误信息关键字");

                entity.Property(e => e.Operator)
                    .HasMaxLength(50)
                    .HasComment("操作人");

                entity.Property(e => e.OriginalMessage)
                    .HasMaxLength(500)
                    .HasComment("原始错误信息");

                entity.Property(e => e.ProductTypes)
                    .HasMaxLength(50)
                    .HasComment("产品类型，多个用英文逗号分隔。");

                entity.Property(e => e.UpdateTime)
                    .HasColumnType("datetime")
                    .HasComment("更新时间");
            });
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    }
}
