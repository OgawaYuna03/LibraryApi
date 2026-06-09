using System.Security.Authentication;
using LibraryApi.Applications.Exceptions;
namespace LibraryApi.Domains.Models;
/// <summary>
/// 商品を表すドメインオブジェクト(集約ルート)
/// </summary>
public class Book
{
    // 業務上の図書識別子（UUID）
    public string BookUuid { get; private set; } = string.Empty;
    // 書名（最大50文字）
    public string Title { get; private set; } = string.Empty;
    // 著書名（最大30文字）
    public string Author { get; private set; } = string.Empty;
   // 分類（null不可）
    public Category? Category { get; private set; }
   
    // 蔵書情報（null不可）
    public BookStock? BookStock { get; private set; }



    /// <summary>
    /// 再構築・復元用コンストラクタ（UUID指定）
    /// </summary>
    /// <param name="bookUuid">識別UUID</param>
    /// <param name="title">書名</param>
    /// <param name="author">著書名</param>
    /// <param name="category">分類</param>
    /// <param name="bookStock">蔵書</param>
    public Book(string bookUuid, string title, string author, Category category, BookStock bookStock)
    {
        ValidateUuid(bookUuid); // UUID形式検証
        BookUuid = bookUuid;
        ValidateTitle(title);        // 書名検証
        Title = title;
        ValidateAuthor(author);      // 著者検証
        Author = author;
        Category = category ?? throw new DomainException("分類は必須です。");
        BookStock = bookStock ?? throw new DomainException("在庫情報は必須です。");
    }

    /// <summary>
    /// 新規作成用コンストラクタ（UUID自動生成）
    /// </summary>
    /// <param name="title">書名</param>
    /// <param name="author">著書名</param>
    /// <param name="category">分類</param>
    /// <param name="bookstock">在庫</param>
    public Book(string title, string author, Category category, BookStock stock)
        : this(Guid.NewGuid().ToString(), title, author, category, stock) { }

    /// <summary>
    /// 再構築・復元用コンストラクタ（UUID指定）
    /// </summary>
    /// <param name="bookUuid">識別UUID</param>
    /// <param name="title">書名</param>
    /// <param name="author">著書名</param>
    /// <param name="category">分類</param>
    /// <param name="bookStock">蔵書</param>
    public Book(string bookUuid, string title, string author)
    {
        ValidateUuid(bookUuid); // UUID形式検証
        BookUuid = bookUuid;
        ValidateTitle(title);        // 商品名検証
        Title = title;
        ValidateAuthor(author);      // 価格検証
        Author = author;
    }
    /// <summary>
    /// UUIDの形式検証
    /// </summary>
    private void ValidateUuid(string bookUuid)
    {
        if (!Guid.TryParse(bookUuid, out _))
            throw new DomainException("UUIDの形式が正しくありません。");
    }

    //  書名の最大長
    private const int MaxTitleLength =50;
    /// <summary>
    /// 商品名の検証
    /// </summary>
    private void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("書名は必須です。");
        if (title.Length > MaxTitleLength)
            throw new DomainException($"書名は{MaxTitleLength}文字以内である必要があります。");
    }
     //  著書名の最大長
    private const int MaxAuthorLength =30;
    /// <summary>
    /// 著書名の検証
    /// </summary>
    private void ValidateAuthor(string author)
    {
        if (string.IsNullOrWhiteSpace(author))
            throw new DomainException("著書名は必須です。");
        if (author.Length > MaxAuthorLength)
            throw new DomainException($"著書名は{MaxAuthorLength}文字以内である必要があります。");
    }

    
    

    /// <summary>
    /// 書名の変更
    /// </summary>
    public void ChangeTitle(string title)
    {
        ValidateTitle(title);
        Title = title;
    }

    /// <summary>
    /// 著書名の変更
    /// </summary>
    public void ChangeAuthor(string author)
    {
        ValidateAuthor(author);
        Author = author;
    }

    /// <summary>
    /// カテゴリの変更
    /// </summary>
    public void ChangeCategory(Category category)
    {
        Category = category ?? throw new DomainException("分類名は必須です。");
    }

    /// <summary>
    /// 在庫の変更
    /// </summary>
    public void ChangeStock(BookStock bookStock)
    {
        BookStock = bookStock ?? throw new DomainException("蔵書情報は必須です。");
    }

    /// <summary>
    /// 識別子の等価性判定
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        return obj is Book other && BookUuid == other.BookUuid;
    }
    public override int GetHashCode() => BookUuid.GetHashCode();

    /// <summary>
    /// インスタンスの内容
    /// </summary>
    /// <returns></returns>
    public override string ToString()
        => $"{BookUuid}: {Title} , {Author} / {Category?.Name ?? "未分類"} , 蔵書数: {BookStock?.Stock ?? 0}";
}