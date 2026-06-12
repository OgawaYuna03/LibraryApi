using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LibraryApi.Domains.Models;
using LibraryApi.Domains.Repositories;
using LibraryApi.Domains.Exceptions;
using LibraryApi.Application.Usecases.Products.Interfaces;
using LibraryApi.Presentation.Configs;
namespace LibraryApi.Application.Tests.Usecase.Products.Interactors;
/// <summary>
/// ユースケース:[図書を変更する]を実現するインターフェイスの実装のテストドライバ
/// </summary>
[TestClass]
[TestCategory("Usecase/Books/Interactor")]
public class UpdateBookUsecaseTests
{
    // MSTestテスト用ログ出力ハンドル
    private static TestContext? _testContext;
    // サービスプロバイダ(DIコンテナ)
    private static ServiceProvider? _provider;
    // スコープドサービス
    private IServiceScope? _scope;
    // テストターゲット
    private static IUpdateBookUsecase? _usecase;
    // 図書リポジトリ
    private static IBookRepository? _repository;

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
            .AddJsonFile("appsettings.json", optional: false).Build();
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
        _usecase =
        _scope.ServiceProvider.GetRequiredService<IUpdateBookUsecase>();
        // 図書リポジトリを取得する
        _repository =
        _scope.ServiceProvider.GetRequiredService<IBookRepository>();
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

    [TestMethod("存在する図書Idで図書を取得できる")]
    public async Task GetBookByIdAsync_ShouldReturnBook_WhenIdExists()
    {
        var result = await _usecase!.GetBookByIdAsync("64b25512-6dfc-4034-9372-9030f118bdb9");
        // nullでないことを検証する
        Assert.IsNotNull(result);
        // 図書Idを検証する
        Assert.AreEqual("64b25512-6dfc-4034-9372-9030f118bdb9", result.BookUuid);
        // 図書名を検証する
        Assert.AreEqual("はらぺこあおむし", result.Title);
        // 単価を検証する
        Assert.AreEqual("エリック・カール", result.Author);
        // 図書在庫Idを検証する
        Assert.AreEqual("8311a860-c63f-45d5-9b42-3bfd6ef886f3", result.BookStock!.StockUuid);
        // 図書在庫数を検証する
        Assert.AreEqual(10, result.BookStock!.Stock);
        // 図書カテゴリIdを検証する
        Assert.AreEqual("e269c98c-61b7-4ca7-9fae-ecd74234989e", result.Category!.CategoryUuid);
        // 図書カテゴリ名を検証する
        Assert.AreEqual("児童書", result.Category!.Name);
    }
    [TestMethod("存在しない図書Idの場合、NotFoundExceptionがスローされる")]
public async Task GetBookByIdAsync_ShouldThrowNotFoundException_WhenIdDoesNotExist()
{
    var ex = await Assert.ThrowsExceptionAsync<NotFoundException>(async () =>
    {
        await _usecase!.GetBookByIdAsync("79023e82-9197-40a5-b236-26487f404be5");
    });
    // nullでないことを検証する
    Assert.IsNotNull(ex);
    // 例外メッセージを検証する
    Assert.AreEqual("図書Id:79023e82-9197-40a5-b236-26487f404be5の図書は存在しません。", ex.Message);
 }
 [TestMethod("存在しない図書名を指定すると例外はスローされない")]
public async Task ExistsByTitleNameAsync_ShouldNotThrow_WhenNameExists()
{
    await _usecase!.ExistsByTitleNameAsync("かいけつゾロリ");
    Assert.IsTrue(true);
}
[TestMethod("存在する図書名を指定するとExistsExceptionがスローされる")]
public async Task ExistsByTitleNameAsync_ShouldThrowExistsException_WhenNameDoesNotExist()
{
    var ex = await Assert.ThrowsExceptionAsync<ExistsException>(async () =>
    {
        await _usecase!.ExistsByTitleNameAsync("はらぺこあおむし");
    });
    Assert.AreEqual("図書名:はらぺこあおむしは既に存在します。", ex.Message);
}
    [TestMethod("図書の変更:存在する図書の場合、図書を変更できる")]
    public async Task UpdateBookAsync_ShouldUpdateBook_WhenBookExists()
    {
        const string id = "64b25512-6dfc-4034-9372-9030f118bdb9";
        // 変更データを用意する
        var book = new Book(id, "はらぺこあおむし", "エリック・カール");
        var bookStock = new BookStock("8311a860-c63f-45d5-9b42-3bfd6ef886f3", 10);
        book.ChangeStock(bookStock);

        // 図書を変更する
        await _usecase!.UpdateBookAsync(book);

        // 変更データを取得する
        var changeBook = await _repository!
            .SelectByIdWithBookStockAndCategoryAsync(id);
        // 図書名を検証する
        Assert.AreEqual("はらぺこあおむし", changeBook!.Title);
        // 単価を検証する
        Assert.AreEqual("エリック・カール", changeBook!.Author);
        // 図書在庫を検証する
        Assert.AreEqual(10, changeBook.BookStock!.Stock);

        // クリーニング：変更データを復元する
        book.ChangeTitle("はらぺこあおむし");
        book.ChangeAuthor("エリック・カール");
        book.BookStock!.ChangeStock(10);
        await _usecase.UpdateBookAsync(book);
    }
        [TestMethod("図書の変更:存在しない図書Idの場合、NotFoundExceptionがスローされる")]
    public async Task UpdateBookAsync_ShouldThrowNotFoundException_WhenIdDoesNotExist()
    {
        const string id = "79023e82-9197-40a5-b236-26487f404be5";
        // 変更データを用意する
        var book = new Book(id, "はらぺこあおむし", "エリック・カール");
        var bookStock = new BookStock("8311a860-c63f-45d5-9b42-3bfd6ef886f3", 150);
        book.ChangeStock(bookStock);
        var ex = await Assert.ThrowsExceptionAsync<NotFoundException>(async () =>
        {
            // 図書を変更する
            await _usecase!.UpdateBookAsync(book);
        });
        // nullでないことを検証する
        Assert.IsNotNull(ex);
        // 例外メッセージを検証する
        Assert.AreEqual("図書Id:79023e82-9197-40a5-b236-26487f404be5の図書は存在しないため変更できません。", ex.Message);
    }
}