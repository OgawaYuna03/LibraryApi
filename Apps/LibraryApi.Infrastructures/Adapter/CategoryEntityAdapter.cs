using LibraryApi.Domains.Adapters;
using LibraryApi.Domains.Models;
using LibraryApi.Domains.Exceptions;
using LibraryApi.Infrastructure.Entities;
namespace LibraryApi.Infrastructure.Adapters;
/// <summary>
/// ドメインオブジェクト:CategoryとCategoryEntityの相互変換クラス
/// </summary> 
/// <typeparam name="Category">ドメインオブジェクト:Category</typeparam>
/// <typeparam name="CategoryEntity">EFCore:CategoryEntity</typeparam>
public class CategoryEntityAdapter :
IConverter<Category, CategoryEntity>, IRestorer<Category, CategoryEntity>
{
    /// <summary>
    /// ドメインオブジェクト:CategoryをCategoryEntityに変換する
    /// </summary>
    /// <param name="domain">ドメインオブジェクト:Category</param>
    /// <returns>EFCore:CategoryEntity</returns>
    public Task<CategoryEntity> ConvertAsync(Category domain)
    {
        // 引数domainがnullの場合
        _ = domain ?? throw new InternalException("引数domainがnullです。");
        // ドメインオブジェクト:CategoryをCategoryEntityに変換する
        var entity = new CategoryEntity();
        entity.CategoryUuid = domain.CategoryUuid;
        entity.Name = domain.Name;
        return Task.FromResult(entity);
    }

    /// <summary>
    /// BookCategoryEntityからドメインオブジェクト:BookCategoryを復元する
    /// </summary>
    /// <param name="target">>EFCore:BookCategoryEntity</param>
    /// <returns>ドメインオブジェクト:BookCategory</returns>
    public Task<Category> RestoreAsync(CategoryEntity target)
    {
        // 引数targetがnullの場合
        _ = target ?? throw new InternalException("引数targetがnullです。");
        // BookCategoryEntityからドメインオブジェクト:BookCategoryを復元する
        var domain = new Category(target.CategoryUuid, target.Name);
        return Task.FromResult(domain);
    }
}