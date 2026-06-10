using LibraryApi.Domains.Exceptions;
namespace LibraryApi.Domains.Models;
/// <summary>
/// 分類を表すドメインオブジェクト
/// </summary>
public class Category 
{
    
   
    /// <summary>
    /// 識別Id(UUID形式)
    /// </summary>
    public string CategoryUuid { get; private set; }= string.Empty;
    /// <summary>   
    /// 分類名
    /// </summary>
    public string Name { get; private set; }= string.Empty;
    
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="categoryUuid">識別Id</param>
    /// <param name="name"分類名</param>
    public Category (string categoryUuid, string name)
    {
        // データのドメインルール違反チェック
        ValidatCategory(categoryUuid, name);
        CategoryUuid = categoryUuid;
        ValidateName(name);
        Name = name;
    }
    /// <summary>
    /// コンストラクタ:新しい識別Id(UUID)を生成する
    /// </summary>
    /// <param name="name">分類名</param>
    public Category(string name): this(Guid.NewGuid().ToString(), name){}

    /// <summary>
    /// データのドメインルール違反チェック
    /// </summary>
    /// <param name="categoryUuid">識別Id</param>
    /// <param name="name">分類名</param>
    /// <exception cref="DomainException">引数が無効な場合にスローされる</exception>
    private void ValidatCategory(string categoryUuid, string name)
    {
        if (string.IsNullOrWhiteSpace(categoryUuid))
            throw new DomainException($"識別Idは必須です。{nameof(categoryUuid)}");
        if (!Guid.TryParse(categoryUuid, out _))
            throw new DomainException($"識別IdはUUIDの形式でなければなりません。");
        ValidateName(name);// 分類名のバリデーション処理（共通）
    }   

    /// <summary>
    /// 分類名のバリデーション処理（共通）
    /// </summary>
    /// <param name="name">分類名</param>
    private void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException($"分類名は必須です。");
        if (name.Length > 20)
            throw new DomainException($"分類名は20文字以内で指定してください。現在の長さ: {name.Length}");
    }

    /// <summary>
    /// 分類名を変更する
    /// </summary>
    /// <param name="newName">新しい分類名</param>
    /// <exception cref="DomainException">分類名が無効な場合にスローされる</exception>
    public void ChangeName(string newName)
    {
        // 分類名のバリデーション処理（共通）
        ValidateName(newName);
        Name = newName;
    }

    /// <summary>
    /// 識別子の等価性判定
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        return obj is Category other && CategoryUuid == other.CategoryUuid;
    }
    public override int GetHashCode() => CategoryUuid.GetHashCode();

    /// <summary>
    /// インスタンスの内容
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"{CategoryUuid}: {Name}";
}