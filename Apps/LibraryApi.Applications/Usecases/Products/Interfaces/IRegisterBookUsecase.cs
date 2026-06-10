using LibraryApi.Domains.Models;
namespace  LibraryApi.Application.Usecases.Products.Interfaces;
/// <summary>
/// ユースケース:[新商品を登録する]を実現するインターフェイス
/// </summary>
public interface IRegisterBookUsecase {

    /// <summary>
    /// すべての商品カテゴリを取得する
    /// クライアント側の[入力画面]で利用するプルダウンを作成するため
    /// </summary>
    /// <returns>Categoryのリスト</returns>
    Task<List<Category>> GetCategoriesAsync();

    /// <summary>
    /// 指定された商品カテゴリIdの商品カテゴリを取得する
    /// クライアント側の[確認画面]で利用するため
    /// </summary>
    /// <param name="id">商品カテゴリId</param>
    /// <returns>該当商品カテゴリ</returns>
    /// <exception cref="NotFoundException">該当データが存在しない場合にスローされる</exception>
    Task<Category> GetCategoryByIdAsync(string id);

    /// <summary>
    /// 指定ざれた商品の存在有無を調べる
    /// </summary>
    /// <param name="bookName">商品目</param>
    /// <returns>なし</returns>
    /// <exception cref="ExistsException">同一商品名が存在する場合にスローされる</exception>
    Task ExistsByTitleAsync(string bookName);

    /// <summary>
    /// 新商品を登録する
    /// </summary>
    /// <param name="product">登録対象商品</param>
    /// <returns>なし</returns>
    /// <exception cref="NotFoundException">商品カテゴリが存在しない場合にスローされる</exception>
    Task RegisterBookAsync(Book book);
}