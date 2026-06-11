using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LibraryApi.Domains.Models;
namespace LibraryApi.Infrastructure.Entities;
/// <summary>
/// productテーブルに対応するEntity Framework Coreのエンティティ
/// </summary>
[Table("book")]
public class BookEntity
{
    [Key] // 主キーをマッピング
    [Column("id")]
    // 列名と同じ名称のプロパティなので[Column]は使わない
    public int Id { get; set; }

    [Required] // NOT NUll
    [StringLength(36)] // データ長は36文字
    [Column("book_uuid")]// マッピングする列名
    public string BookUuid { get; set; } = string.Empty;

    [Column("title")]
    [Required] // NOT NULL
    [StringLength(50)]// データ長は20文字
    // 列名と同じ名称のプロパティなので[Column]は使わない
    public string Title { get; set; } = string.Empty;

    [Column("author")]
    [Required] // NOT NULL
    // 列名と同じ名称のプロパティなので[Column]は使わない
    public string Author { get; set; } = string.Empty;

    [Column("category_id")]// マッピングする列名
    public int? CategoryId { get; set; }

    // Categroyエンティティへのナビゲーションプロパティ
    // CategoryIdプロパティの値と外部キー関係にある
    // null許容にし、図書のカテゴリを含めないケースも許可する
    [ForeignKey("CategoryId")]
    public CategoryEntity? Category { get; set; }

    // 在庫情報（1:1 関係を想定）
    public BookStockEntity? BookStock { get; set; }

    public override string ToString()
    {
        return $"Id={Id}, BookUuid={BookUuid}, Title={Title}, Author={Author}, " +
               $"Category={Category?.Name}, Stock={BookStock?.Stock}";
    }
}