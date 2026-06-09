using LibraryApi.Applications.Exceptions;
namespace LibraryApi.Domains.Models;
/// <summary>
/// 蔵書を表すドメインオブジェクト
/// </summary>
public class BookStock
{
    // 業務上の識別id（UUID）
    public string StockUuid { get; private set; } = string.Empty;
    // 蔵書数
    public int Stock { get; private set; }

    /// <summary>
    /// 再構築・復元用コンストラクタ（UUIDを指定）
    /// </summary>
    /// <param name="stockUuid">蔵書UUID</param>
    /// <param name="stock">蔵書数</param>
    public BookStock(string stockUuid, int stock)
    {
        ValidateUuid(stockUuid);  // UUID形式の検証
        StockUuid = stockUuid;
        ValidateStock(stock);     // 在庫数の検証
        Stock = stock;
    }

    /// <summary>
    /// 新規作成用コンストラクタ（UUID自動生成）
    /// </summary>
    /// <param name="stock">蔵書</param>
    public BookStock(int stock)
        : this(Guid.NewGuid().ToString(), stock) { }

    /// <summary>
    /// 在庫数のルール検証（0以上）
    /// </summary>
    private void ValidateStock(int stock)
    {
        if (stock < 0)
            throw new DomainException("蔵書数は0以上である必要があります。");
    }
    /// <summary>
    /// UUIDの形式検証
    /// </summary>
    private void ValidateUuid(string uuid)
    {
        if (!Guid.TryParse(uuid, out _))
            throw new DomainException("UUIDの形式が正しくありません。");
    }

    /// <summary>
    /// 在庫数の変更
    /// </summary>
    public void ChangeStock(int stock)
    {
        ValidateStock(stock);
        Stock = stock;
    }

    /// <summary>
    /// 識別子の等価性判定
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        return obj is BookStock other && StockUuid == other.StockUuid;
    }
    public override int GetHashCode() => StockUuid.GetHashCode();

    /// <summary>
    /// インスタンスの内容
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"{StockUuid}: {Stock} 個";
}