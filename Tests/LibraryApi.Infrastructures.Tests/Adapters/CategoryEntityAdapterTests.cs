using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LibraryApi.Domains.Models;
using LibraryApi.Domains.Exceptions;
using LibraryApi.Infrastructure.Adapters;
using LibraryApi.Infrastructure.Entities;
using LibraryApi.Presentation.Configs;
namespace LibraryApi.Infrastructure.Tests.Adapters;
/// <summary>
/// ドメインオブジェクト:CategoryとCategoryEntityの相互変換クラスの単体テストドライバ
/// </summary>
[TestClass]
[TestCategory("Adapters")]
public class CategoryEntityAdapterTests
{
    // テストターゲット
    private CategoryEntityAdapter _adapter = null!;
    // サービスプロバイダ(DIコンテナ)    
    private static ServiceProvider? _provider;
    // スコープドサービス
    private IServiceScope? _scope;

    /// <summary>
    /// テストクラスの初期化
    /// </summary>
    /// <param name="_"></param>
    [ClassInitialize]
    public static void ClassInit(TestContext _)
    {
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
        // コープドサービスを取得する
        _scope = _provider!.CreateScope();
        // テストターゲットを取得する
        _adapter =
        _scope.ServiceProvider.GetRequiredService<CategoryEntityAdapter>();  
    }

    /// <summary>
    /// テストメソッド実行後の後処理
    /// </summary>
    [TestCleanup]
    public void TestCleanup()
    {
        // コープドサービスを破棄する
        _scope!.Dispose();
    }

    [TestMethod("CategoryからCategoryEntityに変換できる")]
    public async Task ConvertAsync_Should_MapPropertiesCorrectly()
    {
        // 変換対象を生成する
        var uuid = Guid.NewGuid().ToString();
        var domain = new Category(uuid, "児童書");
        // CategroyをCategoryEntityに変換する
        var entity = await _adapter.ConvertAsync(domain);
        // nullでないことを検証する
        Assert.IsNotNull(entity);
        // カテゴリIdが一致していることを検証する
        Assert.AreEqual(uuid, entity.CategoryUuid);
        // カテゴリ名が児童書であることを検証する
        Assert.AreEqual("児童書", entity.Name);
    }

    [TestMethod("ConvertAsync()メソッドにnullを渡すとInternalExceptionをスローする")]
    public async Task ConvertAsync_Should_ThrowException_When_Null()
    {
        var exception = await Assert.ThrowsExceptionAsync<InternalException>(async () =>
        {
            _ = await _adapter.ConvertAsync(null!);
        });
        Assert.AreEqual("引数domainがnullです。", exception.Message);
    }

    [TestMethod("CategoryEntityからCategoryを復元できる")]
    public async Task RestoreAsync_Should_MapPropertiesCorrectly()
    {
        // 復元対象を生成する
        var uuid = Guid.NewGuid().ToString();
        var entity = new CategoryEntity { CategoryUuid = uuid, Name = "家電" };
        // CategoryEntityからCategoryを復元する
        var domain = await _adapter.RestoreAsync(entity);

        // nullでないことを検証する
        Assert.IsNotNull(domain);
        // カテゴリIdが一致していることを検証する
        Assert.AreEqual(uuid, domain.CategoryUuid);
        // カテゴリ名が家電であることを検証する
        Assert.AreEqual("家電", domain.Name);
    }

    [TestMethod("RestoreAsync()メソッドにnullを渡すとInternalExceptionをスローする")]
    public async Task RestoreAsync_Should_ThrowException_When_Null()
    {
        var exception = await Assert.ThrowsExceptionAsync<InternalException>(async () =>
        {
            _ = await _adapter.RestoreAsync(null!);
        });
        Assert.AreEqual("引数targetがnullです。", exception.Message);
    }
}