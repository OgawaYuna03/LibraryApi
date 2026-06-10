using LibraryApi.Domains.Models;
using LibraryApi.Domains.Exceptions;
namespace LibraryApi.Application.Applications.Tests.Domains.Models;
/// <summary>
/// Productクラスの単体テストドライバ
/// </summary>
[TestClass]
[TestCategory("Domains/Models")]
public class BookTests
{
    // ヘルパー：有効なカテゴリ
    private Category CreateCategory(string name = "児童書") =>
        new Category(name);
    // ヘルパー：有効な在庫
    private BookStock CreateStock(int stock = 10) => new BookStock(stock);

    [TestMethod("コンストラクタに正常値を指定するとインスタンス生成される")]
    public void Constructor_WithValidValues_ShouldCreateInstance()
    {
        // データを用意する
        var uuid = Guid.NewGuid().ToString();
        var title = "かいけつゾロリ";
        var author = "原敬";
        var category = CreateCategory();
        var stock = CreateStock();
        // インスタンスを生成する
        var book = new Book(uuid, title, author, category, stock);
        // 商品Idを検証する
        Assert.AreEqual(uuid, book.BookUuid);
        // 商品名を検証する
        Assert.AreEqual(title, book.Title);
        // 単価を検証する
        Assert.AreEqual(author, book.Author);
        // 商品カテゴリを検証する
        Assert.AreEqual(category, book.Category);
        // 商品在庫を検証する
        Assert.AreEqual(stock, book.BookStock);
    }

    [TestMethod("新規作成の場合UUIDが自動生成される")]
    public void NewInstance_ShouldGenerateUuidAutomatically()
    {
        // データを用意する
        var title = "かいけつゾロリ";
        var author = "原敬";
        var category = CreateCategory();
        var stock = CreateStock();
        // インスタンスを生成する
        var book = new Book(title, author, category, stock);
        // 商品IdがUUID形式かどうかを検証する
        Assert.IsTrue(Guid.TryParse(book.BookUuid, out _));
        // 商品名を検証する
        Assert.AreEqual(title, book.Title);
        // 単価を検証する
        Assert.AreEqual(author, book.Author);
        // 商品カテゴリを検証する
        Assert.AreEqual(category, book.Category);
        // 商品在庫を検証する
        Assert.AreEqual(stock, book.BookStock);
    }

    [TestMethod("不正なUUIDの場合、DomainExceptionがスローされる")]
    public void InvalidUuid_ShouldThrowDomainException()
    {
        // 不正なUUIDを用意する
        var invalidUuid = "abcde";
        var title = "本";
        var author = "草間彌生";
        var category = CreateCategory();
        var stock = CreateStock();
        var ex = Assert.ThrowsException<DomainException>(() =>
        {
            _ = new Book(invalidUuid, title, author, category, stock);
        });
        // 例外メッセージを検証する
        Assert.AreEqual("UUIDの形式が正しくありません。", ex.Message);
    }

    [TestMethod("書名が空白の場合、DomainExceptionがスローされる")]
    public void EmptyTitle_ShouldThrowDomainException()
    {
        var category = CreateCategory();
        var stock = CreateStock();
        var ex = Assert.ThrowsException<DomainException>(() =>
        {
            _ = new Book(Guid.NewGuid().ToString(), "", "草間彌生", category, stock);
        });
        // 例外メッセージを検証する
        Assert.AreEqual("書名は必須です。", ex.Message);
    }

    [TestMethod("書名が51文字以上の場合、DomainExceptionがスローされる")]
    public void TitleLongerThan50Chars_ShouldThrowDomainException()
    {
        var title = new string('あ', 51); // 51文字
        var category = CreateCategory();
        var stock = CreateStock();
        var ex = Assert.ThrowsException<DomainException>(() =>
        {
            _ = new Book(Guid.NewGuid().ToString(), title, "草間彌生", category, stock);
        });
        // 例外メッセージを検証する
        Assert.AreEqual("書名は50文字以内である必要があります。", ex.Message);
    }

    [TestMethod("著書名が31文字以上の場合、DomainExceptionがスローされる")]
    public void  AuthorLongerThan30Chars_ShouldThrowDomainException()
    {
        var title = "書名";
        var author = new string('あ', 31); // 31文字
        var category = CreateCategory();
        var stock = CreateStock();
        var ex = Assert.ThrowsException<DomainException>(() =>
        {
            _ = new Book(Guid.NewGuid().ToString(), title, author, category, stock);
        });
        // 例外メッセージを検証する
        Assert.AreEqual("著書名は30文字以内である必要があります。", ex.Message);
    }

    [TestMethod("有効な書名に変更できる")]
    public void Title_WithValidValue_ShouldSucceed()
    {
        // インスタンスを生成する
        var book = new Book("解決ゾロリ", "草間彌生", CreateCategory(), CreateStock());
        // 商品名を変更する
        book.ChangeTitle("かいけつゾロリ");
        // 変更結果を検証する
        Assert.AreEqual("かいけつゾロリ", book.Title);
    }

    [TestMethod("有効な著書名に変更できる")]
    public void Author_WithValidValue_ShouldSucceed()
    {
        // インスタンスを生成する
        var book = new Book("本", "原敬", CreateCategory(), CreateStock());
        // 単価を変更する
        book.ChangeAuthor("草間彌生");
        // 変更結果を検証する
        Assert.AreEqual("草間彌生", book.Author);
    }

    [TestMethod("有効な分類名に変更できる")]
    public void Category_WithValidValue_ShouldSucceed()
    {
        // インスタンスを生成する
        var newCategory = CreateCategory("新カテゴリ");
        var book = new Book("本", "草間彌生", CreateCategory(), CreateStock());
        // 商品カテゴリを変更する
        book.ChangeCategory(newCategory);
        // 商品カテゴリを検証する
        Assert.AreEqual("新カテゴリ", book.Category!.Name);
    }


    [TestMethod("有効な蔵書に変更できる")]
    public void BookStock_WithValidValue_ShouldSucceed()
    {
        // インスタンスを生成する
        var newStock = CreateStock(30);
        var book = new Book("本", "草間彌生", CreateCategory(), CreateStock());
        // 蔵書数を変更する
        book.ChangeStock(newStock);
        // 蔵書数を検証する
        Assert.AreEqual(30, book.BookStock!.Stock);
    }

    [TestMethod("UUIDで等価と判定される")]
    public void Equals_WithSameUuid_ShouldReturnTrue()
    {
        // インスタンスを生成する
        var uuid = Guid.NewGuid().ToString();
        var b1 = new Book(uuid, "A", "著書名", CreateCategory(), CreateStock());
        var b2 = new Book(uuid, "B", "著書名", CreateCategory(), CreateStock());
        // 等価性を検証する
        var result = b1.Equals(b2);
        // 検証結果を評価する
        Assert.IsTrue(result);
    }

    [TestMethod("異なるUUIDで非等価と判定される")]
    public void Equals_WithDifferentUuid_ShouldReturnFalse()
    {
        // インスタンスを生成する
        var b1 = new Book("A", "草間彌生", CreateCategory(), CreateStock());
        var b2 = new Book("B", "草間彌生", CreateCategory(), CreateStock());
        // 等価性を検証する
        var result = b1.Equals(b2);
        // 非等価であることを評価する
        Assert.IsFalse(result);
    }
}