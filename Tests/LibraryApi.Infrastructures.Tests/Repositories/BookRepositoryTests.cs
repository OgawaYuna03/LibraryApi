using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LibraryApi.Domains.Models;
using LibraryApi.Domains.Repositories;
using LibraryApi.Domains.Exceptions;
using LibraryApi.Infrastructure.Adapters;
using LibraryApi.Infrastructure.Contexts;
using LibraryApi.Presentation.Configs;
namespace LibraryApi.Infrastructure.Tests.Repositories;
/// <summary>
///  ドメインオブジェクト:書籍のCRUD操作インターフェイスの実装の単体テストドライバ
/// </summary>
[TestClass]
[TestCategory("Repositories")]
public class BookRepositoryTests
{
    // MSTestテスト用ログ出力ハンドル
    private static TestContext? _testContext;
    // アプリケーションで利用するDbContextの継承
    private static AppDbContext? _dbContext;
    // テストターゲット
    private static IBookRepository _bookRepository = null!;
    // サービスプロバイダ(DIコンテナ)
    private static ServiceProvider? _provider;
    // スコープドサービス
    private IServiceScope? _scope;

    /// <summary>
    /// テストクラスの初期化
    /// </summary>
    /// <param name="_"></param>
    [ClassInitialize]
    public static void ClassInit(TestContext context)
    {
        // MSTestテスト用ログ出力ハンドルを設定する
        _testContext = context;
        // アプリケーション管理を生成
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
        // サービスプロバイダ(DIコンテナ)の生成 
        _provider = ApplicationDependencyExtensions.BuildAppProvider(config);
    }

    /// <summary>
    /// テストクラスクリーンアップ
    /// </summary>
    [ClassCleanup]
    public static void ClassCleanup()
    {
        // 生成したサービスプロバイダ(DIコンテナ)を破棄する
        _provider?.Dispose();
    }

    /// <summary>
    /// テストの前処理
    /// </summary>
    [TestInitialize]
    public void TestInit()
    {
        // スコープドサービスを取得する
        _scope = _provider!.CreateScope();
        // テストターゲットを取得する
        _bookRepository =
        _scope.ServiceProvider.GetRequiredService<IBookRepository>();
        // AppDbContxetを取得する
        _dbContext =
        _scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    /// <summary>
    /// テストメソッド実行後の後処理
    /// </summary>
    [TestCleanup]
    public void TestCleanup()
    {
        // スコープドサービスを破棄する
        _scope!.Dispose();
    }

    [TestMethod("存在する書籍Idで書籍、書籍在庫、書籍カテゴリを取得できる")]
    public async Task SelectByIdWithBookStockAndCategoryAsync_WhenIdExists_ShouldReturnBookWithStockAndCategory()
    {
        var book = await _bookRepository
        .SelectByIdWithBookStockAndCategoryAsync("64b25512-6dfc-4034-9372-9030f118bdb9");
        // nullでないことを検証する
        Assert.IsNotNull(book);
        // 書籍Idを検証する
        Assert.AreEqual("64b25512-6dfc-4034-9372-9030f118bdb9", book.BookUuid);
        // 図書名を検証する
        Assert.AreEqual("はらぺこあおむし", book.Title);
        // 単価を検証する
        Assert.AreEqual("エリック・カール", book.Author);
        // 書籍在庫がnullでないことを検証する
        Assert.IsNotNull(book.BookStock);
        // 書籍在庫Idを検証する
        Assert.AreEqual("8311a860-c63f-45d5-9b42-3bfd6ef886f3", book.BookStock.StockUuid);
        // 在庫数を検証する
        Assert.AreEqual(10, book.BookStock.Stock);
        // 書籍カテゴリIdを検証する
        Assert.AreEqual("e269c98c-61b7-4ca7-9fae-ecd74234989e", book.Category!.CategoryUuid);
        // 書籍カテゴリ名を検証する
        Assert.AreEqual("児童書", book.Category!.Name);
    }

    [TestMethod("存在しない書籍Idの場合nullが返される")]
    public async Task SelectByIdWithBookStockAndCategoryAsync_WhenIdDoesNotExist_ShouldReturnNull()
    {
        var book = await _bookRepository
        .SelectByIdWithBookStockAndCategoryAsync("8f81a72a-58ef-422b-b472-d982e8665282");
        // nullであることを検証する
        Assert.IsNull(book);
    }
        [TestMethod("書籍と書籍在庫を永続化できる")]
    public async Task CreateAsync_WithStock_ShouldPersistBoth()
    {
        // 登録データを用意する
        var bookCategory = new Category("e269c98c-61b7-4ca7-9fae-ecd74234989e", "児童書");
        var bookStock = new BookStock(Guid.NewGuid().ToString(), 20);
        var book = new Book(Guid.NewGuid().ToString(), "書籍-A","草間彌生");
        book.ChangeStock(bookStock);
        book.ChangeCategory(bookCategory);

        var strategy = _dbContext!.Database.CreateExecutionStrategy();
        await strategy!.ExecuteAsync(async () =>
        {
            // トランザクションを開始する
            await using var tx = await _dbContext!.Database.BeginTransactionAsync();
            try
            {
                // 書籍と書籍在庫を永続化する
                await _bookRepository.CreateAsync(book);
                // 登録された書籍と書籍在庫を取得して値を検証する
                var result = await _bookRepository
                     .SelectByIdWithBookStockAndCategoryAsync(book.BookUuid);
                 // nullでないことを検証する
                 Assert.IsNotNull(result);
                 // 書籍Idを検証する
                 Assert.AreEqual(result.BookUuid, book.BookUuid);
                 // 図書名を検証する
                 Assert.AreEqual(result.Title, book.Title);
                 // 単価を検証する
                 Assert.AreEqual(result.Author, book.Author);
                 // 書籍在庫がnullでないことを検証する
                 Assert.IsNotNull(result.BookStock);
                 // 書籍在庫Idを検証する
                 Assert.AreEqual(result.BookStock.StockUuid, book.BookStock!.StockUuid);
                 // 在庫数を検証する
                 Assert.AreEqual(result.BookStock.Stock, book.BookStock.Stock);
            }
            finally
            {
                tx.Rollback(); // トランザクションをロールバックする
                tx.Dispose();  // トランザクションリソースを開放する
                _testContext!.WriteLine("トランザクションをロールバックしました。");
            }
        });
    }
        [TestMethod("図書名が存在するとtrueが返される")]
    public async Task ExistsByTitle_WhenTitleExists_ShouldReturnTrue()
    {
        var result = await _bookRepository.ExistsByTitleAsync("はらぺこあおむし");
        Assert.IsTrue(result);
    }

    [TestMethod("図書名が存在しないとfalseが返される")]
    public async Task ExistsByTitle_WhenTitleDoesNotExist_ShouldReturnFalse()
    {
        var result = await _bookRepository.ExistsByTitleAsync("かいけつゾロリ");
    Assert.IsFalse(result);
    }
       [TestMethod("存在する書籍のキーワードを指定すると、該当する書籍のリストが返される")]
    public async Task SelectByTitleLikeWithBookStockAndCategoryAsync_WithExistingKeyword_ShouldReturnMatchingBooks()
    {
        var books = await _bookRepository
        .SelectByTitleLikeWithBookStockAndCategoryAsync("はらぺこ");
        // nullでないことを検証する
        Assert.IsNotNull(books);
        // 件数が4件であることを検証する
        Assert.AreEqual(1, books.Count);
    }
    [TestMethod("存在しない書籍のキーワードを指定すると、空の書籍のリストが返される")]
    public async Task SelectByTitleLikeWithBookStockAndCategoryAsync_WithNonExistingKeyword_ShouldReturnEmptyList()
    {
        var books = await _bookRepository
            .SelectByTitleLikeWithBookStockAndCategoryAsync("書籍-X");
        // nullでないことを検証する
        Assert.IsNotNull(books);
        // 件数が0であることを検証する
        Assert.AreEqual(0, books.Count);
    }
        [TestMethod("存在する書籍を変更するとtrueが返される")]
    public async Task UpdateBook_WhenBookExists_ShouldReturnTrue()
    {
        // 変更データを準備する
        var bookStock = new BookStock("8311a860-c63f-45d5-9b42-3bfd6ef886f3", 10);
        var book = new Book("64b25512-6dfc-4034-9372-9030f118bdb9", "はらぺこあおむし", "エリック・カール");
        book.ChangeStock(bookStock);

        var strategy = _dbContext!.Database.CreateExecutionStrategy();
        await strategy!.ExecuteAsync(async () =>
        {
            // トランザクションを開始する
            await using var tx = await _dbContext!.Database.BeginTransactionAsync();
            try
            {
                // 書籍を変更する
                var result = await _bookRepository.UpdateByIdAsync(book);
                // trueであることを検証する
                Assert.IsTrue(result);
                // 変更された書籍を取得する
                var updateResult = await _bookRepository
                    .SelectByIdWithBookStockAndCategoryAsync(book.BookUuid);
                // 図書名を検証する
                Assert.AreEqual(book.Title, updateResult!.Title);
                // 単価を検証する
                Assert.AreEqual(book.Author, updateResult!.Author);
                // 書籍在庫数を検証する
                Assert.AreEqual(book.BookStock!.Stock, updateResult.BookStock!.Stock);
        }
        finally
        {
            tx.Rollback(); // トランザクションをロールバックする
            tx.Dispose();  // トランザクションリソースを開放する
            _testContext!.WriteLine("トランザクションをロールバックしました。");
        } 
    });
}

    [TestMethod("存在しない書籍を変更するとfalseが返される")]
    public async Task UpdateBook_WhenBookDoesNotExist_ShouldReturnFalse()
    {
        // 変更データを準備する
        var bookStock = new BookStock("828fb567-6f6b-11f0-954a-00155d1bd30a", 50);
        var book = new Book("ac413f22-0cf1-490a-9635-7e9ca810e555", "かいけつゾロリ", "草間彌生");
        book.ChangeStock(bookStock);
        // 書籍を変更する
        var result = await _bookRepository.UpdateByIdAsync(book);
        // falseが返されることを検証する
        Assert.IsFalse(result);
    }

}