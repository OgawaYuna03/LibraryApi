using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LibraryApi.Domains.Models;
namespace LibraryApi.Infrastructure.Entities;
/// <summary>
/// product_stockテーブルに対応するEntity Framework Coreのエンティティ
/// </summary>
[Table("book_stock")]
public class BookStockEntity
{
    [Column("id")]
    [Key] // 主キーをマッピング
    // 列名と同じ名称のプロパティなので[Column]は使わない
    public int Id { get; set; }

    [Required] // NOT NUll
    [StringLength(36)] // データ長は36文字
    [Column("stock_uuid")]// マッピングする列名
    public string StockUuid { get; set; } = string.Empty;

    [Column("stock")]
    [Required] // NOT NULL
    // 列名と同じ名称のプロパティなので[Column]は使わない
    public int Stock { get; set; }

    [Column("book_id")]// マッピングする列名
    public int BookId { get; set; }

    // 逆向きのナビゲーション
    [ForeignKey("ProductId")]
    public BookEntity? Book { get; set; }
}