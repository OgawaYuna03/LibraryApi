using LibraryApi.Domains.Adapters;
using LibraryApi.Domains.Models;
using LibraryApi.Domains.Exceptions;
using LibraryApi.Infrastructure.Entities;
namespace LibraryApi.Infrastructure.Adapters;
/// <summary>
/// ドメインオブジェクト:ProductとProductEntityの相互変換クラス
/// </summary> 
/// <typeparam name="Book">ドメインオブジェクト:Book</typeparam>
/// <typeparam name="BookEntity">EFCore:BookEntity</typeparam>
public class BookEntityAdapter :
IConverter<Book, BookEntity>, IRestorer<Book, BookEntity>
{
    /// <summary>
    /// ドメインオブジェクト:BookをBookEntityに変換する
    /// </summary>
    /// <param name="domain">ドメインオブジェクト:Book</param>
    /// <returns>EFCore:BookEntity</returns>
    public Task<BookEntity> ConvertAsync(Book domain)
    {
        // 引数domainがnullの場合
        _ = domain ?? throw new InternalException("引数domainがnullです。");
        // ドメインオブジェクト:DepartmentをDepartmentEntityに変換する
        var entity = new BookEntity();
        entity.BookUuid = domain.BookUuid;
        entity.Title = domain.Title;
        entity.Author = domain.Author;
        return Task.FromResult(entity);
    }

    /// <summary>
    /// BookEntityからドメインオブジェクト:Bookを復元する
    /// </summary>
    /// <param name="target">>EFCore:BookEntity</param>
    /// <returns>ドメインオブジェクト:Book</returns>
    public Task<Book> RestoreAsync(BookEntity target)
    {
        // 引数targetがnullの場合
        _ = target ?? throw new InternalException("引数targetがnullです。");
        // BookEntityからドメインオブジェクト:Bookを復元する
        var domain = new Book(target.BookUuid, target.Title, target.Author);
        return Task.FromResult(domain);
    }
}