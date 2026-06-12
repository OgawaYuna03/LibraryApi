using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LibraryApi.Domains.Models;
using LibraryApi.Domains.Repositories;
using LibraryApi.Application.Usecases.Products.Interfaces;
using LibraryApi.Presentation.Adapters;
using LibraryApi.Presentation.Configs;
using LibraryApi.Presentation.Controllers;
using LibraryApi.Presentation.ViewModels;
namespace LibraryApi.Presentation.Tests.Controllers;
/// <summary>

[TestClass]
[TestCategory("Controllers")]
public class DeleteBookControllerTests
{
    // MSTestテスト用ログ出力ハンドル
    private static TestContext? _testContext;
    // サービスプロバイダ(DIコンテナ)
    private static ServiceProvider? _provider;
    // スコープドサービス
    private IServiceScope? _scope;
    // ユースケース:[図書をキーワード検索する]を実現するインターフェイス
    private IDeleteBookUsecase? _usecase;
    // テストターゲット
    private DeleteBookController? _controller;
    private IBookRepository? _bookRepository;

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

    [TestInitialize]
    public void TestInit()
    {
        // スコープドサービスを取得する
        _scope = _provider!.CreateScope();
        // [図書をキーワード検索する]を実現インターフェイスを取得する
        _usecase = _scope.ServiceProvider.GetRequiredService<IDeleteBookUsecase>();
        // テストターゲットを生成する
        _bookRepository = _scope.ServiceProvider.GetRequiredService<IBookRepository>();
        // services.AddControllers()では、Controllerそのものは登録されないため  
        _controller = new DeleteBookController(_usecase!);
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
    [TestMethod("削除図書の取得:存在しない図書Idの場合、NotFound(404)とエラーが返される")]
    public async Task Delete_ShouldReturnNotFound_WhenBookIdNotExists()
    {
        var bookId = Guid.NewGuid().ToString();


        var response = await _controller!.Delete(bookId);
        // responseをNotFoundObjectResultに変換する
        var notfound = response as NotFoundObjectResult;
        // nullでないことを検証する
        Assert.IsNotNull(notfound);
        // レスポンスボディを取得する
        var val = notfound.Value!;
        var code = (string)val.GetType().GetProperty("code")!.GetValue(val)!;
        var msg = (string)val.GetType().GetProperty("message")!.GetValue(val)!;
        // コードを検証する
        Assert.AreEqual("BOOKID_NOT_FOUND", code);
        // エラーメッセージを検証する
        Assert.AreEqual($"図書Id:{bookId}は既に存在しません。", msg);
    }

    [TestMethod("削除図書の取得:存在する図書Idの場合、NoContent(204)が返される")]
    public async Task Delete_ShouldReturnNoContent_WhenBookExists()
    {
        // テストデータを用意する
        var category = new Category("e269c98c-61b7-4ca7-9fae-ecd74234989e", "児童書");
        var bookStock = new BookStock(Guid.NewGuid().ToString(), 20);
        var book = new Book(Guid.NewGuid().ToString(), "図書-A", "草間彌生");
        book.ChangeCategory(category);
        book.ChangeStock(bookStock);

        await _bookRepository!.CreateAsync(book);
        var response = await _controller!.Delete(book.BookUuid);
        Assert.IsInstanceOfType(response, typeof(NoContentResult));


        // 取得して削除確認
        var result = await _bookRepository!
            .SelectByIdWithBookStockAndCategoryAsync(book.BookUuid);

        // nullであることを確認
        Assert.IsNull(result);

    }


}