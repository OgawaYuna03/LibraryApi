using LibraryApi.Domains.Models;
using LibraryApi.Domains.Exceptions;
namespace LibraryApi.Application.Applications.Tests.Domains.Models;
/// <summary>
/// ProductCategoryクラスの単体テストドライバ
/// </summary>
[TestClass]
[TestCategory("Domains/Models")]
public class CategoryTests
{
    [TestMethod("コンストラクタに正常値を指定するとインスタンス生成される")]
    public void Constructor_WithValidValues_ShouldCreateInstance()
    {
        // データを準備する
        var uuid = Guid.NewGuid().ToString();
        var name = "小説";
        // インスタンスを生成する
        var categoryUuid = new Category(uuid, name);
        // nullでないことを検証する
        Assert.IsNotNull(categoryUuid);
        // 商品カテゴリIdを検証する
        Assert.AreEqual(uuid, categoryUuid.CategoryUuid);
        // 商品カテゴリ名を検証する
        Assert.AreEqual(name, categoryUuid.Name);
    }

    [TestMethod("新規作成の場合UUIDが自動生成される")]
    public void NewInstance_ShouldGenerateUuidAutomatically()
    {
        // データを用意する
        var name = "教科書";
        // インスタンスを生成する
        var category = new Category(name);
        // 識別IdがUUID形式かどうかを検証する
        Assert.IsTrue(Guid.TryParse(category.CategoryUuid, out _));
        // 分類名を検証する
        Assert.AreEqual(name, category.Name);
    }

    [TestMethod("不正なUUIDの場合、DomainExceptionがスローされる")]
    public void InvalidUuid_ShouldThrowDomainException()
    {
        // 不正なUUID
        var invalidUuid = "abcde"; 
        var name = "カテゴリ";
        var ex = Assert.ThrowsException<DomainException>(() =>
        {
            _ = new Category(invalidUuid , name); // インスタンスを生成する
        });
        // 例外メッセージを検証する
        Assert.AreEqual("識別IdはUUIDの形式でなければなりません。", ex.Message);
    }

    [TestMethod("カテゴリ名が空白の場合、DomainExceptionがスローされる")]
    public void EmptyCategoryName_ShouldThrowDomainException()
    {
        // データを準備する
        var categoryuuid = Guid.NewGuid().ToString();
        var name = "  ";
        var ex = Assert.ThrowsException<DomainException>(() =>
        {
            _ = new Category(categoryuuid, name); // インスタンスを生成する
        });
        // 例外メッセージを検証する
        Assert.AreEqual("分類名は必須です。", ex.Message);
    }

    [TestMethod("分類名が21文字以上の場合、DomainExceptionがスローされる")]
    public void CategoryNameLongerThan20Chars_ShouldThrowDomainException()
    {
        // Arrange
        var name = new string('あ', 21); // 21文字
        var ex = Assert.ThrowsException<DomainException>(() =>
        {
            _ = new Category(Guid.NewGuid().ToString(), name); // インスタンスを生成する
        });
        // 例外メッセージを検証する
        Assert.AreEqual("分類名は20文字以内で指定してください。現在の長さ: 21", ex.Message);
    }

    [TestMethod("有効な分類名に変更できる")]
    public void ChangeName_WithValidValue_ShouldSucceed()
    {
        // インスタンスを生成する
        var category = new Category("ビジネス書");
        var newName = "絵本";
        // 商品カテゴリ名を変更する
        category.ChangeName(newName);
        // 変更結果を検証する
        Assert.AreEqual(newName, category.Name);
    }

    [TestMethod("空白で分類名を変更すると、DomainExceptionがスローされる")]
    public void ChangeName_WithWhitespace_ShouldThrowDomainException()
    {
        // インスタンスを生成する
        var category = new Category("児童書");
        var ex = Assert.ThrowsException<DomainException>(() =>
        {
            category.ChangeName("");// 空白で商品カテゴリ名を変更する
        });
        // 例外メッセージを検証する
        Assert.AreEqual("分類名は必須です。", ex.Message);
    }

    [TestMethod("UUIDで等価と判定される")]
    public void Equals_WithSameUuid_ShouldReturnTrue()
    {
        // インスタンスを用意する
        var uuid = Guid.NewGuid().ToString();
        var category1 = new Category(uuid, "A");
        var category2 = new Category(uuid, "B");
        // 等価性を検証する
        var result = category1.Equals(category2);
        // 検証結果を評価する
        Assert.IsTrue(result);
    }

    [TestMethod("異なるUUIDで非等価と判定される")]
    public void Equals_WithDifferentUuid_ShouldReturnFalse()
    {
        // インスタンスを用意する
        var category1 = new Category("小説");
        var category2 = new Category("漫画");
        // 等価性を検証する
        var result = category1.Equals(category2);
        // 非等価であることを評価する
        Assert.IsFalse(result);
    }
}