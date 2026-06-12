using Microsoft.EntityFrameworkCore;
using LibraryApi.Infrastructure.Entities;
using LibraryApi.Domains.Models;
namespace LibraryApi.Infrastructure.Contexts;
/// <summary>
/// アプリケーション用データベースコンテキスト（PostgreSQL対応）
/// 方針：
/// - Book : BookCategory = N:1（カテゴリ削除で図書も削除：Cascade）
/// - Book : BookStock    = 1:1（図書削除で在庫も削除：Cascade）
/// - UUIDはドメイン層で生成して保存（DB側での自動生成はしない）
/// - 1:1は book_stock.book_id の一意制約で担保
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="options"></param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    /// <summary>
    /// 図書テーブルアクセスプロパティ
    /// </summary>
    public DbSet<BookEntity> Books => Set<BookEntity>();
    /// <summary>
    /// 図書カテゴリテーブルアクセスプロパティ
    /// </summary>
    public DbSet<CategoryEntity> Categories => Set<CategoryEntity>();
    /// <summary>
    /// 図書在庫テーブルアクセスプロパティ
    /// </summary>
    public DbSet<BookStockEntity> BookStocks => Set<BookStockEntity>();

    // TODO: Fluent API でマッピングを定義する
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 図書テーブルに対する動作設定
        modelBuilder.Entity<BookEntity>(e =>
        {
            // 図書Id(UUID)はユニーク
            e.HasIndex(b => b.BookUuid).IsUnique();
            // 図書名はvarchar(50)でNULL許容
            e.Property(b => b.Title).HasMaxLength(50);
            // 図書カテゴリと図書のカーディナリティ(1:N) 図書カテゴリ削除時に図書も削除
            e.HasOne(b => b.Category)
                .WithMany(b => b.Books!)
                .HasForeignKey(b => b.CategoryId)
                .HasConstraintName("fk_book_category")
                .OnDelete(DeleteBehavior.Cascade);
            // 図書と図書在庫のカーディナリティ(1:1) 図書削除時に図書在庫も削除
            e.HasOne(b => b.BookStock)
                .WithOne(s => s.Book!)
                .HasForeignKey<BookStockEntity>(s => s.BookId)
                .HasConstraintName("fk_book_stock_book")
                .OnDelete(DeleteBehavior.Cascade);
            // C#のstring ⇔ PostgreSQLのuuidを自動変換する
            e.Property(b => b.BookUuid).HasMaxLength(36);
            // .HasConversion(
                // v => Guid.Parse(v),
                // v => v.ToString()
           // );
        });
        // 図書カテゴリの動作設定
        modelBuilder.Entity<CategoryEntity>(e =>
        {
            // 図書カテゴリId(UUID)はユニーク
            e.HasIndex(c => c.CategoryUuid).IsUnique();
            // 図書カテゴリ名はvarchar(20)でNULL許容
            e.Property(c => c.Name).HasMaxLength(20);

            // C#のstring ⇔ PostgreSQLのuuidを自動変換する
            e.Property(c => c.CategoryUuid).HasMaxLength(36);
             //.HasConversion(
                // v => Guid.Parse(v),  // C#(string)をDB(uuid)に書き込む時の処理
                // v => v.ToString()    // DB(uuid)をC#(string)に読み込む時の処理
          //  );
        });
        // 図書在庫の動作設定
        modelBuilder.Entity<BookStockEntity>(e =>
        {
            // 図書在庫Id(UUID)はユニーク
            e.HasIndex(s => s.StockUuid).IsUnique();
            // 図書Id(UUID)はユニーク
            e.HasIndex(s => s.BookId).IsUnique();
            // C#のstring ⇔ PostgreSQLのuuidを自動変換する
            e.Property(s => s.StockUuid).HasMaxLength(36);
             //.HasConversion(
               //  v => Guid.Parse(v),
               //  v => v.ToString()
            //);
        });
    }
}