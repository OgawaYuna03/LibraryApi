using LibraryApi.Domains.Models;
namespace LibraryApi.Domains.Repositories;

public interface ICategoryRepository
{
    /// <summary>
    /// すべての商品カテゴリを取得する
    /// </summary>
    /// <returns>ProductCategoryのリスト</returns>
    Task<List<Category>> SelectAllAsync();

    /// <summary>
    /// 指定された商品カテゴリIdの商品カテゴリを取得する
    /// </summary>
    /// <param name="id">商品カテゴリId</param>
    /// <returns>ProductCategory または null</returns>
    Task<Category?> SelectByIdAsync(string id);
}