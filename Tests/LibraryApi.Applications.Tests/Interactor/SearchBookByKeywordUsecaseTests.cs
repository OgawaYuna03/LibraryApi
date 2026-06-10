using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LibraryApi.Application.Usecases.Products.Interfaces;
using LibraryApi.Presentation.Configs;
using LibraryApi.Domains.Exceptions;
namespace LibraryApi.Application.Tests.Usecase.Products.Interactors;
/// <summary>
/// ユースケース:[商品をキーワード検索する]を実現するインターフェイスの実装のテストドライバ
/// </summary>
[TestClass]
[TestCategory("Usecase/Books/Interactor")]
public class SearchBookByKeywordUsecaseTests
{
    private static TestContext? _testContext;
    private static ServiceProvider? _provider;
    private IServiceScope? _scope;
    // テストターゲット
    private static ISearchBookByKeywordUsecase? _usecase;

    /// <summary>
    /// テストクラスの初期化
    /// </summary>
    /// <param name="_"></param>
    [ClassInitialize]
    public static void ClassInit(TestContext context)
    {
        // MSTestテスト用ログ出力ハンドルを設定する
        _testContext = context;
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
        _provider = ApplicationDependencyExtensions.BuildAppProvider(config);
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        _provider?.Dispose();
    }

    /// <summary>
    /// テストの前処理
    /// </summary>
    [TestInitialize]
    public void TestInit()
    {
        _scope = _provider!.CreateScope();
        _usecase =
        _scope.ServiceProvider.GetRequiredService<ISearchBookByKeywordUsecase>();
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _scope!.Dispose();
    }

    [TestMethod("存在する商品キーワードで商品を取得できる")]
    public async Task ExecuteAsync_ShouldReturnBooks_WhenKeywordExists()
    {
        var results = await _usecase!.ExecuteAsync("第1巻");
        // nullでないことを検証する
        Assert.IsNotNull(results);
        // 件数が3件であること検証する
        Assert.AreEqual(3, results.Count);
        // 商品Idを検証する
        Assert.AreEqual("9e393859-8db2-4d72-b1d7-5f385acc9497", results[0].BookUuid);
        // 商品名を検証する
        Assert.AreEqual("ONE PIECE 第1巻", results[0].Title);
        // 単価を検証する
        Assert.AreEqual("尾田栄一郎", results[0].Author);
        // 在庫数を検証する
        Assert.AreEqual(7, results[0].BookStock!.Stock);
        // 商品カテゴリIdを検証する
        Assert.AreEqual(
        "51e7f90e-5d61-4546-aa42-e85d98fbe542", results[0].Category!.CategoryUuid);
        // 商品カテゴリ名を検証する
        Assert.AreEqual("漫画", results[0].Category!.Name);

        // 商品Idを検証する
        Assert.AreEqual("386cdc7f-caa1-4cc5-bfb2-156fc797cb0f", results[1].BookUuid);
        // 商品名を検証する
        Assert.AreEqual("鬼滅の刃 第1巻", results[1].Title);
        // 単価を検証する
        Assert.AreEqual("吾峠呼世晴", results[1].Author);
        // 在庫数を検証する
        Assert.AreEqual(5, results[1].BookStock!.Stock);
        // 商品カテゴリIdを検証する
        Assert.AreEqual(
        "51e7f90e-5d61-4546-aa42-e85d98fbe542", results[1].Category!.CategoryUuid);
        // 商品カテゴリ名を検証する
        Assert.AreEqual("漫画", results[1].Category!.Name);

        // 商品Idを検証する
        Assert.AreEqual("e07c34cc-7764-4e28-bd2f-64ae386fbd42", results[2].BookUuid);
        // 商品名を検証する
        Assert.AreEqual("SPY×FAMILY 第1巻", results[2].Title);
        // 単価を検証する
        Assert.AreEqual("遠藤達哉", results[2].Author);
        // 在庫数を検証する
        Assert.AreEqual(3, results[2].BookStock!.Stock);
        // 商品カテゴリIdを検証する
        Assert.AreEqual(
        "51e7f90e-5d61-4546-aa42-e85d98fbe542", results[2].Category!.CategoryUuid);
        // 商品カテゴリ名を検証する
        Assert.AreEqual("漫画", results[2].Category!.Name);

    }

    [TestMethod("存在しない商品キーワードの場合、空のリストが返される")]
    public async Task ExecuteAsync_ShouldThrowNotFoundException_WhenKeywordDoesNotExist()
    {
        var result = await _usecase!.ExecuteAsync("ゴム");
        // nullでないことを検証する
        Assert.IsNotNull(result);
        // 件数が0件であることを検証する
        Assert.AreEqual(0, result.Count);
    }
}