using System.ComponentModel.DataAnnotations;
namespace LibraryApi.Presentation.ViewModels;
/// <summary>
/// ユースケース:[新商品を登録する]を実現するViewModel
/// </summary>
public class RegisterBookViewModel
{
    // 商品名
    [Required(ErrorMessage = "書名は必須です。")]
    [StringLength(50, ErrorMessage = "書名は{1}文字以内で入力してください。")]
    public string Title { get; set; } = string.Empty;
    // 単価
    [Required(ErrorMessage = "著者名は必須です。")]
    [StringLength(30, ErrorMessage = "著者名は{1}文字以内で入力してください。")]

    public string Author { get; set; } = string.Empty;
    // 在庫数
    [Required(ErrorMessage = "蔵書数は必須です。")]
    [Range(0, int.MaxValue, ErrorMessage = "蔵書数は0以上の整数を指定してください。")]
    public int Stock { get; set; }
    // 商品カテゴリId(UUID)
    [Required(ErrorMessage = "識別Idは必須です。")]
    [RegularExpression(
    "^[0-9a-fA-F]{8}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{12}$",
    ErrorMessage = "識別IdはUUID形式で指定してください。")]
    public string CategoryId { get; set; } = string.Empty;
    // 商品カテゴリ名
    [Required(ErrorMessage = "分類名は必須です。")]
    [StringLength(20, ErrorMessage = "分類名は{1}文字以内で入力してください。")]
    public string Category { get; set; } = string.Empty;
}