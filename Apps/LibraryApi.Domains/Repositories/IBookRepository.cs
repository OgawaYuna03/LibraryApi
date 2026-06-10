using LibraryApi.Domains.Models;
namespace LibraryApi.Domains.Repositories;

/// <summary>
///  ドメインオブジェクト:商品のCRUD操作インターフェイス
/// </summary>
public interface IBookRepository
{
    /// <summary>
    /// 商品を永続化する
    /// </summary>
    /// <param name="book">永続化する商品</param>
    /// <returns>なし</returns>
    Task CreateAsync(Book book);

    /// <summary>
    /// 商品を更新する
    /// </summary>
    /// <param name="book">更新対象の商品</param>
    /// <returns>true:更新成功 false:更新失敗</returns>
    Task<bool> UpdateByIdAsync(Book book);

    /// <summary>
    /// 指定された商品Idの商品と在庫、商品カテゴリを返す
    /// </summary>
    /// <param name="id">商品Id</param>
    /// <returns>Product または null</returns>
    Task<Book?> SelectByIdWithBookStockAndCategoryAsync(string id);

    /// <summary>
    /// 指定されたキーワードで商品を部分一致検索して商品と在庫、商品カテゴリを取得する
    /// </summary>
    /// <param name="keyword">検索キーワード</param>
    /// <returns>Prodyctのリスト</returns>
    Task<List<Book>> SelectByTitleLikeWithBookStockAndCategoryAsync(string keyword);

    /// <summary>
    /// 商品を削除する
    /// </summary>
    /// <param name="id">削除対象の商品Id(UUID)</param>
    /// <returns>true:削除成功 false:削除失敗</returns>
    Task<bool> DeleteByIdAsync(string id);

    /// <summary>
    /// 指定された商品名の存在有無を返す
    /// </summary>
    /// <param name="title">商品名</param>
    /// <returns>true:存在する false:存在しない</returns> 
    Task<bool> ExistsByTitleAsync(string title);
}