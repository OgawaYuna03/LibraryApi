using LibraryApi.Domains.Models;
namespace LibraryApi.Application.Usecases.Products.Interfaces;
/// <summary>
/// ユースケース:[商品を変更する]を実現するインターフェイス
/// </summary>
public interface IUpdateBookUsecase
{
    /// <summary>
    /// 指定された商品Idの商品を取得する
    /// クライアント側の[入力画面]で利用するため
    /// </summary>
    /// <param name="id">商品Id</param>
    /// <returns>該当商品、商品在庫、商品カテゴリ</returns>
    /// <exception cref="NotFoundException">該当データが存在しない場合にスローされる</exception>
    Task<Book> GetBookByIdAsync(string id);   

    /// <summary>
    /// 指定ざれた商品の存在有無を調べる
    /// </summary>
    /// <param name="titleName">商品目</param>
    /// <returns>なし</returns>
    /// <exception cref="ExistsException">同一商品名が存在する場合にスローされる</exception>
    Task ExistsByTitleNameAsync(string titleName);

    /// <summary>
    /// 商品を変更するする
    /// </summary>
    /// <param name="title">変更対象対象商品</param>
    /// <returns>なし</returns>
    /// <exception cref="NotFoundException">商品が存在しない場合にスローされる</exception>
    Task UpdateBookAsync(Book title);
}