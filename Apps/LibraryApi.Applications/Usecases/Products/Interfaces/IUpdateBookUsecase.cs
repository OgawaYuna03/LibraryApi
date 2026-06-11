using LibraryApi.Domains.Models;
namespace LibraryApi.Application.Usecases.Products.Interfaces;
/// <summary>
/// ユースケース:[図書を変更する]を実現するインターフェイス
/// </summary>
public interface IUpdateBookUsecase
{
    /// <summary>
    /// 指定された図書Idの図書を取得する
    /// クライアント側の[入力画面]で利用するため
    /// </summary>
    /// <param name="id">図書Id</param>
    /// <returns>該当図書、図書在庫、図書カテゴリ</returns>
    /// <exception cref="NotFoundException">該当データが存在しない場合にスローされる</exception>
    Task<Book> GetBookByIdAsync(string id);   

    /// <summary>
    /// 指定ざれた図書の存在有無を調べる
    /// </summary>
    /// <param name="titleName">図書目</param>
    /// <returns>なし</returns>
    /// <exception cref="ExistsException">同一図書名が存在する場合にスローされる</exception>
    Task ExistsByTitleNameAsync(string titleName);

    /// <summary>
    /// 図書を変更するする
    /// </summary>
    /// <param name="title">変更対象対象図書</param>
    /// <returns>なし</returns>
    /// <exception cref="NotFoundException">図書が存在しない場合にスローされる</exception>
    Task UpdateBookAsync(Book title);
}