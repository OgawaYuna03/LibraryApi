using Microsoft.AspNetCore.Mvc;
using LibraryApi.Domains.Models;
using Swashbuckle.AspNetCore.Annotations;
using LibraryApi.Application.Usecases.Products.Interfaces;
namespace LibraryApi.Presentation.Controllers;
/// <summary>
/// ユースケース:[書籍をキーワード検索する]を実現するコントローラ
/// </summary>
[ApiController]
[Route("library/api")]
[SwaggerTag("書籍をキーワード検索API")]
public class SearchBookByKeywordController : ControllerBase
{
    private readonly ISearchBookByKeywordUsecase _usecase;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="usecase">ユースケース:[図書をキーワード検索する]を実現するインターフェイス</param>
    public SearchBookByKeywordController(ISearchBookByKeywordUsecase usecase)
    {
        _usecase = usecase;
    }

    /// <summary>
    /// キーワードで図書を検索する
    /// </summary>
    /// <param name="keyword">検索キーワード</param>
    /// <returns>検索結果の図書一覧</returns>
    [HttpGet()]
     // [ProducesResponseType]から[SwaggerResponse]に変更する
    [SwaggerResponse(StatusCodes.Status200OK, "検索に成功した場合、書籍リストを返す", typeof(List<Book>))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "キーワード未入力など、リクエストが不正な場合")]
    public async Task<IActionResult> Search([FromQuery] string? keyword)
    {
        // 未入力チェック
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return BadRequest(
            new { code = "INVALID_KEYWORD", message = "検索キーワードを入力してください。" });
        }
        if(keyword.Length>50)
        {
            return BadRequest(
              new{code ="INVALID_KEYWORD",message = "検索キーワードは50文字以内で入力してください。"}  
            );
        }
        // 図書キーワード検索する
        var result = await _usecase.ExecuteAsync(keyword.Trim());
        return Ok(result);
    }
}