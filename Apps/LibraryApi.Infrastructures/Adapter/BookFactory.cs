using LibraryApi.Domains.Models;
using LibraryApi.Infrastructure.Entities;
using LibraryApi.Infrastructure.Adapters;
namespace LibraryApi.Infrastructure.Adapters;
/// <summary>
/// 本、カテゴリ、蔵書オブジェクトの相互変換Factoryクラス
/// ドメインオブジェクト:BookとBookEntityの相互変換
/// ドメインオブジェクト:BookCategoryとBookEntityの相互変換
/// ドメインオブジェクト:BookStockとBookStockEntityの相互変換
/// </summary>
public class BookFactory
{
    private readonly BookEntityAdapter _bookEntityAdapter;
    private readonly CategoryEntityAdapter _categoryEntityAdapter;
    private readonly BookStockEntityAdapter _bookStockEntityAdapter;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="bookEntityAdapter">BookとBookEntityの相互変換</param>
    /// <param name="categoryEntityAdapter">CategoryとEntityの相互変換</param>
    /// <param name="bookStockEntityAdapter">BookStockとBookStockEntityの相互変換</param>
    public BookFactory(
        BookEntityAdapter bookEntityAdapter,
        CategoryEntityAdapter categoryEntityAdapter,
        BookStockEntityAdapter bookStockEntityAdapter)
    {
        _bookEntityAdapter = bookEntityAdapter;
        _categoryEntityAdapter = categoryEntityAdapter;
        _bookStockEntityAdapter = bookStockEntityAdapter;
    }

    /// <summary>
    /// 図書、図書カテゴリ、図書在庫の集約関係を構築したEntityを生成して返す
    /// </summary>
    /// <param name="domain">ルートドメインオブジェクト:book</param>
    /// <returns>集約関係を構築したbookEntity</returns>
    public async Task<BookEntity> ConvertAsync(Book domain)
    {
        // bookからbookEntityを生成する
        var entity = await _bookEntityAdapter.ConvertAsync(domain);
        // 図書カテゴリ、在庫が存在しない場合はリターンする
        if (domain.Category is null && domain.BookStock is null)
        {
            return entity;
        }
        // 図書カテゴリが存在する
        if (domain.Category != null)
        {
            // BookをBookEntityに変換してプロパティに設定する
            entity.Category =
                await _categoryEntityAdapter.ConvertAsync(domain.Category);
        }
        // 在庫が存在する
        if (domain.BookStock != null)
        {
            // BookStockをBookStockEntityに変換してプロパティに設定する
            entity.BookStock =
                await _bookStockEntityAdapter.ConvertAsync(domain.BookStock);
        }
        return entity;
    }

    /// <summary>
    /// 図書、図書カテゴリ、図書在庫の集約関係を構築したEntityリストを生成して返す
    /// </summary>
    /// <param name="domains">ルートドメインオブジェクトのリスト:List<book></param>
    /// <returns>集約関係を構築したbookEntityのリスト</returns>
    public async Task<List<BookEntity>> ConvertAsync(List<Book> domains)
    {
        // BookEntityのリストを生成する
        var entityies = new List<BookEntity>();
        foreach (var domain in domains)
        {
            // リストから取り出したBookをBookEntityに変換してリストに追加する
            entityies.Add(await ConvertAsync(domain));
        }
        return entityies;
    }

    /// <summary>
    /// bookEntityの集約関係からドメインオブジェクト:bookを復元する
    /// </summary>
    /// <param name="target">bookEntity</param>
    /// <returns>復元したbook</returns>
    public async Task<Book> RestoreAsync(BookEntity target)
    {
        // bookEntityからbookを復元する
        var book = await _bookEntityAdapter.RestoreAsync(target);
        // 図書カテゴリ、図書在庫が存在しない場合はリターンする   
        if (target.Category is null && target.BookStock is null)
        {
            return book;
        }
        // 図書カテゴリが存在する
        if (target.Category != null)
        {
            // CategoryEntityからCategoryを復元してプロパティに設定する
            book.ChangeCategory(
                await _categoryEntityAdapter.RestoreAsync(target.Category));
        }
        // 図書在庫が存在する
        if (target.BookStock != null)
        {
            // bookStockEntityからbookStockを復元してプロパティに設定する
            book.ChangeStock(
                await _bookStockEntityAdapter.RestoreAsync(target.BookStock));
        }
        return book;
    }

    /// <summary>
    /// 図書、図書カテゴリ、図書アジ子の集約関係を構築したEntityリストからドメインオブジェクトのリストを復元する
    /// </summary>
    /// <param name="targets">List<bookEntity></param>
    /// <returns>book<List></returns>
    public async Task<List<Book>> RestoreAsync(List<BookEntity> targets)
    {
        // bookのリストを生成する
        var books = new List<Book>();
        foreach (var target in targets)
        {
            // bookEntityを取り出しbookを復元してリストに追加する
            books.Add(await RestoreAsync(target));
        }
        return books;
    }
}