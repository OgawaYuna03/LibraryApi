using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LibraryApi.Domains.Models;
namespace LibraryApi.Infrastructure.Entities;
/// <summary>
/// product_categoryテーブルに対応するEntity Framework Coreのエンティティ
/// </summary>
[Table("category")]
public class CategoryEntity
{
    [Column("id")]
    [Key] // 主キーをマッピング
    // 列名と同じ名称のプロパティなので[Column]は使わない
    public int Id { get; set; }

    [Required] // NOT NUll
    [StringLength(36)] // データ長は36文字
    [Column("category_uuid")]// マッピングする列名
    public string CategoryUuid { get; set; } = string.Empty;

    [Column("name")]
    [Required] // NOT NULL
    [StringLength(20)]// データ長は20文字
    // 列名と同じ名称のプロパティなので[Column]は使わない
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// カテゴリに属する図書(1:N)
    /// </summary>
    public List<BookEntity> Books { get; set; } = new();

    public override string ToString()
    {
        return $"Id={Id},CategoryUuid={CategoryUuid},Name={Name}";
    }
}