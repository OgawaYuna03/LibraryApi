using Microsoft.EntityFrameworkCore;
using LibraryApi.Domains.Models;
using LibraryApi.Domains.Repositories;
using LibraryApi.Domains.Exceptions;
using LibraryApi.Infrastructure.Adapters;
using LibraryApi.Infrastructure.Contexts;
namespace LibraryApi.Infrastructure.Repositories;
/// <summary>
///  ドメインオブジェクト:図書カテゴリのCRUD操作インターフェイスの実装
/// </summary>
public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;
    private readonly CategoryEntityAdapter _adapter;
    /// <summary>
    /// コンストラクタ 
    /// </summary>
    /// <param name="context">アプリケーション用データベースコンテキスト</param>
    /// <param name="adapter">ドメインオブジェクト:BookCategoryとBookCategoryEntityの相互変換クラス</param> 
    public CategoryRepository(
        AppDbContext context,
        CategoryEntityAdapter adapter)
    {
        _context = context;
        _adapter = adapter;
    }

    /// <summary>
    /// すべての図書カテゴリを取得する
    /// </summary>
    /// <returns>BookCategoryのリスト</returns>
    public async Task<List<Category>> SelectAllAsync()
    {
        try
        {
            // すべての図書カテゴリを取得する
            var entities = await _context.Categories
                .AsNoTracking().ToListAsync();
            // Categoryのリストを生成する
            var categories = new List<Category>();
            foreach (var entity in entities)
            {
                // CategoryEntityからCategoryを復元する
                categories.Add(await _adapter.RestoreAsync(entity));
            }
            return categories;
        }
        catch (DomainException)
        {
            throw; // DomainException例外はそのまま再スローする
        }
        catch (Exception ex)
        {
            // InternalExceptionにラップしてスローする
            throw new InternalException("分類取得時に予期しないエラーが発生しました。", ex);
        }
    }

    /// <summary>
    /// 指定された図書カテゴリIdの図書カテゴリを取得する
    /// </summary>
    /// <param name="id">図書カテゴリId</param>
    /// <returns>BookCategory または null</returns>
    public async Task<Category?> SelectByIdAsync(string id)
    {
        try
        {
            // 引数のUUIDで図書カテゴリを取得する
            var entity = await _context.Categories
                .SingleOrDefaultAsync(c => c.CategoryUuid == id);
            if (entity is null)
            {
                return null; // 存在しない場合はnullを返す
            }
            // BookCategoryEntityからBookCategoryを復元する
            var category = await _adapter.RestoreAsync(entity);
            return category;
        }
        catch (DomainException)
        {
            throw; // DomainException例外はそのまま再スローする
        }
        catch (Exception ex)
        {
            // InternalExceptionにラップしてスローする
            throw new InternalException($"Id:{id}の分類取得時に予期しないエラーが発生しました。", ex);
        }
    }
}