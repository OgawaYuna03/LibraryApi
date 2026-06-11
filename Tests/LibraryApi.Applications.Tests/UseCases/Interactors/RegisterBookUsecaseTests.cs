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
public class RegisterBookUsecaseTests
{
    // MSTestテスト用ログ出力ハンドル
    private static TestContext? _testContext;
    // サービスプロバイダ(DIコンテナ)
    private static ServiceProvider? _provider;
    // スコープドサービス
    private IServiceScope? _scope;
    // テストターゲット
    private static IRegisterBookUsecase? _uscase;
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
        _uscase =
        _scope.ServiceProvider.GetRequiredService<IRegisterBookUsecase>(); 
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

    [TestMethod("すべての図書カテゴリを取得できる")]
    public async Task GetCategoriesAsync_ShouldReturnAllCategories()
    {
        // すべてのカテゴリを取得する
        var categories = await _uscase!.GetCategoriesAsync();
        // nullでないことを検証する
        Assert.IsNotNull(categories);
        // 件数が3件であることを検証する
        Assert.AreEqual(6, categories.Count());
        // 取得内容を検証する
        Assert.AreEqual("18836923-5194-47f1-bf4c-e09eb5fa8fef", categories[0].CategoryUuid);
        Assert.AreEqual("技術書", categories[0].Name);
        Assert.AreEqual("1c7dc46b-5618-4d9b-ad4a-0a805e7032d6", categories[1].CategoryUuid);
        Assert.AreEqual("小説", categories[1].Name);
        Assert.AreEqual("e269c98c-61b7-4ca7-9fae-ecd74234989e", categories[2].CategoryUuid);
        Assert.AreEqual("児童書", categories[2].Name);
        Assert.AreEqual("9dd9db1f-14fe-42e5-879d-e1a2c74223d8", categories[3].CategoryUuid);
        Assert.AreEqual("ビジネス書", categories[3].Name);
        Assert.AreEqual("51e7f90e-5d61-4546-aa42-e85d98fbe542", categories[4].CategoryUuid);
        Assert.AreEqual("漫画", categories[4].Name);
        Assert.AreEqual("d652b797-d71a-4c4c-9539-65049819d942", categories[5].CategoryUuid);
        Assert.AreEqual("雑誌", categories[5].Name);
        foreach (var category in categories)
        {
            _testContext?.WriteLine(category.ToString());
        }
    }
        [TestMethod("存在する図書カテゴリIdで図書カテゴリを取得できる")]
    public async Task GetCategoryByIdAsync_ShouldReturnCategory_WhenIdExists()
    {
        // 図書カテゴリ雑貨を取得する
        var category = await _uscase!.GetCategoryByIdAsync("e269c98c-61b7-4ca7-9fae-ecd74234989e");
        // nullでないことを検証する
        Assert.IsNotNull(category);
        // 図書カテゴリIdと図書カテゴリ名を検証する
        Assert.AreEqual("e269c98c-61b7-4ca7-9fae-ecd74234989e", category.CategoryUuid);
        Assert.AreEqual("児童書", category.Name);
    }
    [TestMethod("存在しない図書カテゴリIdを指定するとNotFoundExceptionがスローされる")]
public async Task GetCategoryByIdAsync_ShouldThrowNotFoundException_WhenIdDoesNotExist()
{
    var ex = await Assert.ThrowsExceptionAsync<NotFoundException>(async () =>
    {
        // 存在しない図書カテゴリIdでカテゴリを取得する
        await _uscase!.GetCategoryByIdAsync("2f5016b6-6f6b-11f0-954a-00155d1bd30a");
    });
    Assert.AreEqual("分類識別Id:2f5016b6-6f6b-11f0-954a-00155d1bd30aの分類名は存在しません。", ex.Message);
}
[TestMethod("存在しない図書名を指定すると例外はスローされない")]
public async Task ExistsByTitleAsync_ShouldNotThrow_WhenNameExists()
{
    await _uscase!.ExistsByTitleAsync("かいけつゾロリ");
    Assert.IsTrue(true);
}
[TestMethod("存在する図書名を指定するとExistsExceptionがスローされる")]
public async Task ExistsByTitleAsync_ShouldThrowExistsException_WhenNameDoesNotExist()
{
    var ex = await Assert.ThrowsExceptionAsync<ExistsException>(async () =>
    {
        await _uscase!.ExistsByTitleAsync("はらぺこあおむし");
    });
    Assert.AreEqual("図書名:はらぺこあおむしは既に存在します。", ex.Message);
}
    [TestMethod("新図書を登録できる")]
    public async Task RegisterTitleAsync_ShouldCreateNewProduct()
    {
        // テストデータを用意する
        var category = new Category("e269c98c-61b7-4ca7-9fae-ecd74234989e", "児童書");
        var bookStock = new BookStock(Guid.NewGuid().ToString(), 20);
        var book = new Book(Guid.NewGuid().ToString(), "図書-A", "草間彌生");
        book.ChangeCategory(category);
        book.ChangeStock(bookStock);
        // 新図書を登録する
        await _uscase!.RegisterBookAsync(book);
        // 登録された図書を取得する
        var result = await _bookRepository!
            .SelectByIdWithBookStockAndCategoryAsync(book.BookUuid);
        // nullでないことを検証する
        Assert.IsNotNull(result);
        // 図書Idを検証する
        Assert.AreEqual(book.BookUuid, result.BookUuid);
        // 図書名を検証する
        Assert.AreEqual(book.Title, result.Title);
        // 単価を検証する
        Assert.AreEqual(book.Author, result.Author);
        // 図書在庫Idを検証する
        Assert.AreEqual(book.BookStock!.StockUuid, result.BookStock!.StockUuid);
        // 図書在庫数を検証する
        Assert.AreEqual(book.BookStock!.Stock, result.BookStock!.Stock);
        // 追加したデータをクリーニングする
        await _bookRepository!.DeleteByIdAsync(book.BookUuid);
    }
}