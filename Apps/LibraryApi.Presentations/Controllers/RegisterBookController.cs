using Microsoft.AspNetCore.Mvc;
using LibraryApi.Domains.Models;
using LibraryApi.Domains.Exceptions;
using LibraryApi.Application.Usecases.Products.Interfaces;
using LibraryApi.Presentation.Adapters;
using LibraryApi.Presentation.ViewModels;
using Swashbuckle.AspNetCore.Annotations;
namespace LibraryApi.Presentation.Controllers;
/// <summary>
/// ユースケース:[新図書を登録する]を実現するコントローラ
/// </summary>
[ApiController]
[Route("library/api")]
[SwaggerTag("図書登録API")]
public class RegisterBookController : ControllerBase
{
    private readonly IRegisterBookUsecase _usecase;
    private readonly RegisterBookViewModelAdapter _adapter;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="usecase">ユースケース:[新図書を登録する]を実現するインターフェイス</param>
    /// <param name="adapter">RegisterBookViewModelからドメインオブジェクト:Bookへ変換するアダプタ</param>
    public RegisterBookController(
        IRegisterBookUsecase usecase,
        RegisterBookViewModelAdapter adapter)
    {
        _usecase = usecase;
        _adapter = adapter;
    }

    /// <summary>
    /// 図書カテゴリ一覧の取得
    /// </summary>
    /// <returns></returns>
    [HttpGet("categories")]
    [SwaggerOperation(Summary = "分類一覧を取得",
                      Description = "登録可能なすべての分類名を返します。")]
    [SwaggerResponse(StatusCodes.Status200OK, "分類一覧", typeof(List<Category>))]
    public async Task<IActionResult> GetCategories()
    {
        var result = await _usecase.GetCategoriesAsync();
        return Ok(result);
    }

    /// <summary>
    /// 選択された図書カテゴリIdで図書カテゴリを取得する
    /// </summary>
    /// <param name="categoryId">図書カテゴリId(UUID)</param>
    /// <returns>該当するカテゴリが存在すればOK(200)、存在しなければNotFound(404)</returns>
    [HttpGet("categories/{categoryId}")]
    [SwaggerOperation(Summary = "分類名の取得",
                      Description = "指定された分類識別Idに一致する分類名を返します。")]
    [SwaggerResponse(StatusCodes.Status200OK, "分類名が見つかった場合", typeof(Category))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "該当分類名が存在しない場合")]
    public async Task<IActionResult> GetCategoryById(string categoryId)
    {
        try
        {
            var category = await _usecase.GetCategoryByIdAsync(categoryId);
            return Ok(category);
        }
        catch (NotFoundException ex)
        {
            // エラーレスポンスを返却する
            return NotFound(new
            { code = "CATEGORY_NOT_FOUND", message = ex.Message });
        }
    }

    // <summary>
    // 図書が既に存在するかを検証する
    // </summary>
    // <param name="bookName">検証対象の図書名</param>
    // <returns>
    // 存在しない場合:Ok(200)、存在する場合:Conflict(409) 
    // </returns>
    // [HttpGet("validate")]
    [SwaggerOperation(Summary = "図書名の存在確認",
                         Description = "図書名が既に存在するかを検証する")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "図書名が未入力の場合")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "図書名が既に存在する場合")]
    public async Task<IActionResult> ValidateBook([FromQuery] string bookName)
    {
        // 図書名がnullか空白
        if (string.IsNullOrWhiteSpace(bookName))
        {
            return BadRequest(new
            { code = "INVALID_BOOK_NAME", message = "図書名は必須です。" });
        }
        try
        {
            // 図書名の存在有無を調べる
            await _usecase.ExistsByTitleAsync(bookName);
            return Ok(new { exists = false });
        }
        catch (ExistsException ex)
        {
            // 図書が既に存在する場合
            return Conflict(new
            { code = "BOOK_ALREADY_EXISTS", message = ex.Message });
        }
    }

    /// <summary>
    /// 新図書を登録する
    /// </summary>
    /// <param name="model">図書登録用ViewModel</param>
    /// <returns></returns>
    [HttpPost("books")]
    [SwaggerOperation(Summary = "図書を登録",
                      Description = "図書情報を受け取り、図書を登録する")]
    [SwaggerResponse(StatusCodes.Status201Created, "登録成功", typeof(Book))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "バリデーションエラーまたは業務ルール違反")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "分類識別Idが存在しない場合")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "図書が既に存在する場合")]
    public async Task<IActionResult> Register(
        RegisterBookViewModel model)
    {
        // サーバーサイドバリデーション
        if (!ModelState.IsValid)
        {
            // プロパティ名をキー、エラーメッセージ配列を値とするディクショナリに変換する
            var details = ModelState
                .Where(kv => kv.Value?.Errors.Count > 0) // エラーがある項目だけを抽出する
                .ToDictionary( // Dictionaryに変換する
                               // キー:プロパティ名 ("Name", "Price" など)
                    kv => kv.Key,
                    // 値: 当該プロパティのエラーメッセージ一覧
                    kv => kv.Value!.Errors
                        // エラーメッセージが空やnullの場合は "Invalid value."に置換する
                        .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage)
                            ? "Invalid value." : e.ErrorMessage)
                        .ToArray()
                );
            return BadRequest(new
            { code = "VALIDATION_ERROR", message = "入力内容に誤りがあります。", details });
        }
        try
        {
            // 存在しない図書カテゴリを受信した(ミスしている)
            await _usecase.GetCategoryByIdAsync(model.CategoryId);
            // 既に登録済みの図書を受信した(ミスしている)
            await _usecase.ExistsByTitleAsync(model.Title);
            // RegisterBookViewModelからBookを復元する
            var book = await _adapter.RestoreAsync(model);
            // 図書を永続化する
            await _usecase.RegisterBookAsync(book);
            return Created($"/library/api/books/{book.BookUuid}", book);
        }
        catch (ExistsException ex)
        {
            // 既に存在する図書を受信した
            return Conflict(new { code = "BOOK_ALREADY_EXISTS", message = ex.Message });
        }
        catch (NotFoundException ex)
        {
            // 存在しない図書カテゴリIdを受信した
            return NotFound(new { code = "CATEGORY_NOT_FOUND", message = ex.Message });
        }
        catch (DomainException ex)
        {
            // 業務ルール違反のデータを受信した
            return BadRequest(new { code = "DOMAIN_RULE_VIOLATION", message = ex.Message });
        }
    }
}