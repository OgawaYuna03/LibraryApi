using Microsoft.AspNetCore.Mvc;
using LibraryApi.Domains.Models;
using LibraryApi.Domains.Exceptions;
using LibraryApi.Application.Usecases.Products.Interfaces;
using LibraryApi.Presentation.Adapters;
using LibraryApi.Presentation.ViewModels;
using Swashbuckle.AspNetCore.Annotations;
namespace LibraryApi.Presentation.Controllers;
// <summary>
// ユースケース:[図書を削除する]を実現するコントローラ
// </summary>
[ApiController]
[Route("library/api")]
[SwaggerTag("図書削除API")]
public class DeleteBookController : ControllerBase
{
    private readonly IDeleteBookUsecase _usecase;
    
    public DeleteBookController(
       IDeleteBookUsecase usecase
     )
    {
        _usecase = usecase;
       
    }
    /* public async Task<IActionResult> GetBookById(string bookId)
    {
        try
        {
            await _usecase.ExistsByIdAsync(bookId);
            return Ok();
        }
        catch (NotFoundException ex)
        {
            // エラーレスポンスを返却する
            return NotFound(new
            { code = "BOOK_NOT_FOUND", message = ex.Message });
        }
    }
    */

    [HttpDelete("book/{bookId}")]
    [SwaggerOperation(Summary = "図書を削除",
                      Description = "図書情報を受け取り、図書を削除する")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "図書削除成功")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "指定された図書が存在しない")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "サーバー内部エラー")]
     public async Task<IActionResult> Delete([FromRoute] string bookId)
    {
       
          try
        {
           await _usecase.ExistsByIdAsync(bookId);
            await _usecase.DeleteByIdAsync(bookId);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            // エラーレスポンスを返却する
            return NotFound(new
            { code = "BOOKID_NOT_FOUND", message = ex.Message });
        }
    }

}