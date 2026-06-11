using LibraryApi.Domains.Models;
namespace LibraryApi.Domains.Repositories;

public interface ICategoryRepository
{
    /// <summary>
    /// すべての図書カテゴリを取得する
    /// </summary>
    /// <returns>ProductCategoryのリスト</returns>
    Task<List<Category>> SelectAllAsync();

    /// <summary>
    /// 指定された図書カテゴリIdの図書カテゴリを取得する
    /// </summary>
    /// <param name="id">図書カテゴリId</param>
    /// <returns>ProductCategory または null</returns>
    Task<Category?> SelectByIdAsync(string id);
}