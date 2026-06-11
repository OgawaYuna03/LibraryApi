using LibraryApi.Domains.Adapters;
using LibraryApi.Domains.Models;
using LibraryApi.Presentation.ViewModels;
namespace LibraryApi.Presentation.Adapters;
/// <summary>
/// RegisterBookViewModelからドメインオブジェクト:Bookへ変換するアダプタ
/// </summary>
public class RegisterBookViewModelAdapter : IRestorer<Book, RegisterBookViewModel>
{
    /// <summary>
    /// RegisterBookViewModelからドメインオブジェクト:Bookを復元する
    /// </summary>
    /// <param name="target">ユースケース:[新図書を登録する]を実現するViewModel</param>
    /// <returns></returns>
    public Task<Book> RestoreAsync(RegisterBookViewModel target)
    {
        // 図書カテゴリを生成する
        var category = new Category(target.CategoryId, target.Category);
        // 図書在庫を生成する
        var bookStock = new BookStock(target.Stock);
        // 図書を生成する
        var book = new Book(Guid.NewGuid().ToString(), target.Title, target.Author);
        // 図書カテゴリと図書在庫を設定する
        book.ChangeCategory(category);
        book.ChangeStock(bookStock);
        return Task.FromResult(book);
    }
}