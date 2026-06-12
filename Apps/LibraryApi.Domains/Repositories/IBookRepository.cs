using LibraryApi.Domains.Models;
namespace LibraryApi.Domains.Repositories;

/// <summary>
///  ドメインオブジェクト:図書のCRUD操作インターフェイス
/// </summary>
public interface IBookRepository
{
    /// <summary>
    /// 図書を永続化する
    /// </summary>
    /// <param name="book">永続化する図書</param>
    /// <returns>なし</returns>
    Task CreateAsync(Book book);

    /// <summary>
    /// 図書を更新する
    /// </summary>
    /// <param name="book">更新対象の図書</param>
    /// <returns>true:更新成功 false:更新失敗</returns>
    Task<bool> UpdateByIdAsync(Book book);

    /// <summary>
    /// 指定された図書Idの図書と在庫、図書カテゴリを返す
    /// </summary>
    /// <param name="id">図書Id</param>
    /// <returns>Product または null</returns>
    Task<Book?> SelectByIdWithBookStockAndCategoryAsync(string id);

    /// <summary>
    /// 指定されたキーワードで図書を部分一致検索して図書と在庫、図書カテゴリを取得する
    /// </summary>
    /// <param name="keyword">検索キーワード</param>
    /// <returns>Prodyctのリスト</returns>
    Task<List<Book>> SelectByTitleLikeWithBookStockAndCategoryAsync(string keyword);

    /// <summary>
    /// 図書を削除する
    /// </summary>
    /// <param name="id">削除対象の図書Id(UUID)</param>
    /// <returns>true:削除成功 false:削除失敗</returns>
    Task<bool> DeleteByIdAsync(string id);

    /// <summary>
    /// 指定された図書名の存在有無を返す
    /// </summary>
    /// <param name="title">図書名</param>
    /// <returns>true:存在する false:存在しない</returns> 
    Task<bool> ExistsByTitleAsync(string title);
     /// <summary>
    /// 指定された図書名の存在有無を返す
    /// </summary>
    /// <param name="title">図書名</param>
    /// <returns>true:存在する false:存在しない</returns> 
    Task<bool> ExistsByIdAsync(string bookId);
}