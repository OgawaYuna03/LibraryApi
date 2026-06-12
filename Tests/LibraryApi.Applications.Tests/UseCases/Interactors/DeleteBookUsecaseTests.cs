using LibraryApi.Domains.Repositories;
using LibraryApi.Application.Usecases.Products.Interfaces;
using Microsoft.Extensions.Configuration;
using LibraryApi.Presentation.Configs;
using Microsoft.Extensions.DependencyInjection;
using LibraryApi.Domains.Exceptions;
using LibraryApi.Domains.Models;
namespace LibraryApi.Application.Tests.Usecase.Products.Interactors;
/// <summary>
/// ユースケース:[新図書を登録する]を実現するインターフェイスの実装のテストドライバ
/// </summary>
[TestClass]
[TestCategory("Usecase/Products/Interactor")]
public class DeleteBookUsecaseTests
{
    // MSTestテスト用ログ出力ハンドル
    private static TestContext? _testContext;
    // サービスプロバイダ(DIコンテナ)
    private static ServiceProvider? _provider;
    // スコープドサービス
    private IServiceScope? _scope;
    // テストターゲット
    private static IDeleteBookUsecase? _usecase;
    // 図書リポジトリ
    private static IBookRepository? _bookRepository;

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
        _scope.ServiceProvider.GetRequiredService<IDeleteBookUsecase>();
        // 図書リポジトリを取得する
        _bookRepository =
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



    [TestMethod("存在する図書Idを指定すると例外はスローされない")]
    public async Task ExistsByIdAsync_ShouldNotThrow_WhenIdExists()
    {
        await _usecase!.ExistsByIdAsync("64b25512-6dfc-4034-9372-9030f118bdb9");
        //Assert.IsTrue(true);
    }
    [TestMethod("存在しない図書Idを指定するとNotFoundExceptionがスローされる")]
    public async Task ExistsByIdAsync_ShouldThrowNotFoundException_WhenIdDoesNotExist()
    {
        var ex = await Assert.ThrowsExceptionAsync<ExistsException>(async () =>
        {
            await _usecase!.ExistsByIdAsync("64b25512-6dfc-4034-9372-9030f118bdb9");
        });
        Assert.AreEqual("図書Id:64b25512-6dfc-4034-9372-9030f118bdb9は既に存在します。", ex.Message);
    }
    [TestMethod("図書を削除できる")]
    public async Task DeleteByIdAsync_ShouldDeleteBook()
    {
        // テストデータを用意する
         var category = new Category("e269c98c-61b7-4ca7-9fae-ecd74234989e", "児童書");
         var bookStock = new BookStock(Guid.NewGuid().ToString(), 20);
         var book = new Book(Guid.NewGuid().ToString(), "図書-A", "草間彌生");
         book.ChangeCategory(category);
         book.ChangeStock(bookStock);
        
        await _bookRepository!.CreateAsync(book);

        // 削除する
        await _usecase!.DeleteByIdAsync(book.BookUuid);

        // 取得して削除確認
        var result = await _bookRepository!
            .SelectByIdWithBookStockAndCategoryAsync(book.BookUuid);

        // nullであることを確認
        Assert.IsNull(result);
    }
}