using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LibraryApi.Domains.Exceptions;
using LibraryApi.Presentation.Adapters;
using LibraryApi.Presentation.Configs;
using LibraryApi.Presentation.ViewModels;

namespace LibraryApi.Presentation.Tests.Adapters;
/// <summary>
/// RegisterBookViewModelAdapterのテストドライバ
/// </summary>
[TestClass]
[TestCategory("Adapters")]
public class RegisterBookViewModelAdapterTests
{
    // MSTestテスト用ログ出力ハンドル
    private static TestContext? _testContext;
    // サービスプロバイダ(DIコンテナ)
    private static ServiceProvider? _provider;
    // スコープドサービス
    private IServiceScope? _scope;
    // テストターゲット
    private RegisterBookViewModelAdapter? _adapter;

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
    /// テストメソッド実行の前処理
    /// </summary>
    [TestInitialize]
    public void TestInit()
    {
        // スコープドサービスを取得する
        _scope = _provider!.CreateScope();
        // テストターゲットを取得する
        _adapter = _scope.ServiceProvider
            .GetRequiredService<RegisterBookViewModelAdapter>();
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

    [TestMethod("ViewModelからBookを復元でき、商品Idと商品在庫Idが自動生成される")]
    public async Task RestoreAsync_ShouldMapVmToDomain_AndGenerateUuids()
    {
        // ViewModelを用意する
        var viewModel = new RegisterBookViewModel
        {
            Title = "はらぺこあおむし",
            Author = "エリック・カール",
            Stock = 10,
            CategoryId = "e269c98c-61b7-4ca7-9fae-ecd74234989e",
            Category = "児童書"
        };
        // ViewModelからBookを復元する
        var book = await _adapter!.RestoreAsync(viewModel);
        // 書籍名を検証する
        Assert.AreEqual(viewModel.Title, book.Title);
        // 著書名を検証する
        Assert.AreEqual(viewModel.Author, book.Author);
        // 商品Idが生成されていることを検証する
        Assert.IsFalse(string.IsNullOrWhiteSpace(book.BookUuid));
        Assert.IsTrue(Guid.TryParse(book.BookUuid, out _));
        // 商品カテゴリがnullでないことを検証する
        Assert.IsNotNull(book.Category);
        // 商品カテゴリIdを検証する
        Assert.AreEqual(viewModel.CategoryId, book.Category!.CategoryUuid);
        // 商品カテゴリ名を検証する
        Assert.AreEqual(viewModel.Category, book.Category.Name);
        // 商品在庫がnullでないことを検証する
        Assert.IsNotNull(book.BookStock);
        // 商品在庫を検証する
        Assert.AreEqual(viewModel.Stock, book.BookStock!.Stock);
        // 商品在庫Idが生成されていることを検証する
        Assert.IsFalse(string.IsNullOrWhiteSpace(book.BookStock.StockUuid));
        Assert.IsTrue(Guid.TryParse(book.BookStock.StockUuid, out _));
    }

    [TestMethod("不正な商品Idの場合、DomainExceptionがスローされる")]
    public async Task RestoreAsync_ShouldThrow_WhenCategoryIdIsInvalidUuid()
    {
        var viewModel = new RegisterBookViewModel
        {
            Title = "はらぺこあおむし",
            Author = "エリック・カール",
            Stock = 10,
            CategoryId = "NOT-A-UUID",
            Category = "児童書"
        };
        // 例外がスローされたことを検証する
        var ex = await Assert.ThrowsExceptionAsync<DomainException>(
            () => _adapter!.RestoreAsync(viewModel));
        // エラーメッセージを検証する
        Assert.AreEqual("識別IdはUUIDの形式でなければなりません。", ex.Message);
    }

    [TestMethod("書籍名が空白の場合、DomainExceptionがスローされる")]
    public async Task RestoreAsync_WhenTitleBlank_ShouldThrowDomainException()
    {
        var viewModel = new RegisterBookViewModel
        {
            Title = " ",
            Author = "エリック・カール",
            Stock = 10,
            CategoryId = "e269c98c-61b7-4ca7-9fae-ecd74234989e",
            Category = "児童書"
        };
        // 例外がスローされたことを検証する
        var ex = await Assert.ThrowsExceptionAsync<DomainException>(
            () => _adapter!.RestoreAsync(viewModel));
        // エラーメッセージを検証する
        Assert.AreEqual("書名は必須です。", ex.Message);
    }

    [TestMethod("書籍名が51文字の場合、DomainExceptionがスローされる")]
    public async Task RestoreAsync_WhenTitleOver30_ShouldThrowDomainException()
    {
        var viewModel = new RegisterBookViewModel
        {
            Title = new string('A', 51),
            Author = "エリック・カール",
            Stock = 10,
            CategoryId = "e269c98c-61b7-4ca7-9fae-ecd74234989e",
            Category = "児童書"
        };
        // 例外がスローされたことを検証する
        var ex = await Assert.ThrowsExceptionAsync<DomainException>(
            () => _adapter!.RestoreAsync(viewModel));
        // エラーメッセージを検証する
        Assert.AreEqual("書名は50文字以内である必要があります。", ex.Message);
    }
    [TestMethod("著者名が31文字の場合、DomainExceptionがスローされる")]
    public async Task AuthorAsync_WhenTitleOver30_ShouldThrowDomainException()
    {
        var viewModel = new RegisterBookViewModel
        {
            Title = "はらぺこあおむし",
            Author = new string('A', 31),
            Stock = 10,
            CategoryId = "e269c98c-61b7-4ca7-9fae-ecd74234989e",
            Category = "児童書"
        };
        // 例外がスローされたことを検証する
        var ex = await Assert.ThrowsExceptionAsync<DomainException>(
            () => _adapter!.RestoreAsync(viewModel));
        // エラーメッセージを検証する
        Assert.AreEqual("著書名は30文字以内である必要があります。", ex.Message);
    }

    [TestMethod("カテゴリIdが空文字の場合、DomainExceptionがスローされる")]
    public async Task RestoreAsync_ShouldThrow_WhenCategoryIdIsEmpty()
    {
        var viewModel = new RegisterBookViewModel
        {
            Title = "はらぺこあおむし",
            Author = "エリック・カール",
            Stock = 10,
            CategoryId = "", // 空文字
            Category = "児童書"
        };
        // 例外がスローされたことを検証する
        var ex = await Assert.ThrowsExceptionAsync<DomainException>(
            () => _adapter!.RestoreAsync(viewModel));
        // エラーメッセージを検証する
        Assert.AreEqual("識別Idは必須です。categoryUuid", ex.Message);
    }

    [TestMethod("在庫数がマイナスの場合、DomainExceptionがスローされる")]
    public async Task RestoreAsync_ShouldThrow_WhenStockIsNegative()
    {
        var viewModel = new RegisterBookViewModel
        {
            Title = "はらぺこあおむし",
            Author = "エリック・カール",
            Stock = -1, // マイナス
            CategoryId = "e269c98c-61b7-4ca7-9fae-ecd74234989e",
            Category = "児童書"
        };
        // 例外がスローされたことを検証する
        var ex = await Assert.ThrowsExceptionAsync<DomainException>(
            () => _adapter!.RestoreAsync(viewModel));
        // エラーメッセージを検証する
        Assert.AreEqual("蔵書数は0以上である必要があります。", ex.Message);
    }

   
}