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
///  ドメインオブジェクト:商品のCRUD操作インターフェイスの実装の単体テストドライバ
/// </summary>
[TestClass]
[TestCategory("Repositories")]
public class ProductRepositoryTests
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

    [TestMethod("存在する商品Idで商品、商品在庫、商品カテゴリを取得できる")]
    public async Task SelectByIdWithBookStockAndCategoryAsync_WhenIdExists_ShouldReturnBookWithStockAndCategory()
    {
        var book = await _bookRepository
        .SelectByIdWithBookStockAndCategoryAsync("64b25512-6dfc-4034-9372-9030f118bdb9");
        // nullでないことを検証する
        Assert.IsNotNull(book);
        // 商品Idを検証する
        Assert.AreEqual("64b25512-6dfc-4034-9372-9030f118bdb9", book.BookUuid);
        // 商品名を検証する
        Assert.AreEqual("はらぺこあおむし", book.Title);
        // 単価を検証する
        Assert.AreEqual("エリック・カール", book.Author);
        // 商品在庫がnullでないことを検証する
        Assert.IsNotNull(book.BookStock);
        // 商品在庫Idを検証する
        Assert.AreEqual("8311a860-c63f-45d5-9b42-3bfd6ef886f3", book.BookStock.StockUuid);
        // 在庫数を検証する
        Assert.AreEqual(10, book.BookStock.Stock);
        // 商品カテゴリIdを検証する
        Assert.AreEqual("e269c98c-61b7-4ca7-9fae-ecd74234989e", book.Category!.CategoryUuid);
        // 商品カテゴリ名を検証する
        Assert.AreEqual("児童書", book.Category!.Name);
    }

    [TestMethod("存在しない商品Idの場合nullが返される")]
    public async Task SelectByIdWithBookStockAndCategoryAsync_WhenIdDoesNotExist_ShouldReturnNull()
    {
        var product = await _bookRepository
        .SelectByIdWithBookStockAndCategoryAsync("8f81a72a-58ef-422b-b472-d982e8665282");
        // nullであることを検証する
        Assert.IsNull(product);
    }
}