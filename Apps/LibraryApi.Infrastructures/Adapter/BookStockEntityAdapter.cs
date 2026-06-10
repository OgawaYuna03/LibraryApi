using LibraryApi.Domains.Adapters;
using LibraryApi.Domains.Models;
using LibraryApi.Domains.Exceptions;
using LibraryApi.Infrastructure.Entities;
namespace LibraryApi.Infrastructure.Adapters;
/// <summary>
/// ドメインオブジェクト:ProductStockとProductStockEntityの相互変換クラス
/// </summary> 
/// <typeparam name="ProductStock">ドメインオブジェクト:ProductStock</typeparam>
/// <typeparam name="ProductStockEntity">EFCore:ProductStockEntity</typeparam>
public class BookStockEntityAdapter :
IConverter<BookStock, BookStockEntity>, IRestorer<BookStock, BookStockEntity>
{
    /// <summary>
    /// ドメインオブジェクト:ProductStockをProductStockEntityに変換する
    /// </summary>
    /// <param name="domain">ドメインオブジェクト:ProductStock</param>
    /// <returns>EFCore:ProductStockEntity</returns>
    public Task<BookStockEntity> ConvertAsync(BookStock domain)
    {
        // 引数domainがnullの場合
        _ = domain ?? throw new InternalException("引数domainがnullです。");
        // ドメインオブジェクト:ProductStockをProductStockEntityに変換する
        var entity = new BookStockEntity();
        entity.StockUuid = domain.StockUuid;
        entity.Stock = domain.Stock;
        return Task.FromResult(entity);
    }
    
    /// <summary>
    /// ProductStockEntityからドメインオブジェクト:ProductStockを復元する
    /// </summary>
    /// <param name="target">>EFCore:ProductStockEntity</param>
    /// <returns>ドメインオブジェクト:ProductStock</returns>
    public Task<BookStock> RestoreAsync(BookStockEntity target)
    {
        // 引数targetがnullの場合
        _ = target ?? throw new InternalException("引数targetがnullです。");
        // ProductStockEntityからドメインオブジェクト:ProductStockを復元する
        var domain = new BookStock(target.StockUuid, target.Stock);
        return Task.FromResult(domain);
    }
}