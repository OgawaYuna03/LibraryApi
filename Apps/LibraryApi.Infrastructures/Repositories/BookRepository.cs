using Microsoft.EntityFrameworkCore;
using LibraryApi.Domains.Models;
using LibraryApi.Domains.Repositories;
using LibraryApi.Domains.Exceptions;
using LibraryApi.Infrastructure.Adapters;
using LibraryApi.Infrastructure.Contexts;
namespace LibraryApi.Infrastructure.Repositories;
/// <summary>
///  ドメインオブジェクト:商品のCRUD操作インターフェイスの実装
/// </summary>
public class BookRepository : IBookRepository
{
    private readonly AppDbContext _context;
    private readonly BookFactory _factory;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="context">アプリケーション用データベースコンテキスト</param>
    /// <param name="factory">商品、商品カテゴリ、商品在庫オブジェクトの相互変換Factoryクラス</param>
    public BookRepository(AppDbContext context, BookFactory factory)
    {
        _context = context;
        _factory = factory;
    }

    /// <summary>
    /// 商品を永続化する
    /// </summary>
    /// <param name="product">永続化する商品</param>
    /// <returns>なし</returns>
    public async Task CreateAsync(Book book)
    {
        try
        {
            // 登録する商品の商品カテゴリを取得する
            var category = await _context.Categories
                .SingleOrDefaultAsync(c => c.CategoryUuid == book.Category!.CategoryUuid);
            if (category is null)
            {
                throw new Exception($"Id:{book.Category!.CategoryUuid}の商品カテゴリは存在しません。");
            }
            // ProductをProductEntityに変換する
            var entity = await _factory.ConvertAsync(book);
            // 商品カテゴリの外部キーを設定する
            entity.Category = null;
            entity.CategoryId =category.Id;
            // 商品を登録する
            await _context.Books.AddAsync(entity);
            // 登録した商品をデータベースに永続化する
            await _context.SaveChangesAsync();
        }
        catch (DomainException)
        {
            throw; // DomainException例外はそのまま再スローする
        }
        catch (Exception ex)
        {
            // InternalExceptionにラップしてスローする
            throw new InternalException("書籍の永続化中に予期しないエラーが発生しました。", ex);
        }
    }

    /// <summary>
    /// 指定された商品Idの商品と在庫、商品カテゴリを返す
    /// </summary>
    /// <param name="id">商品Id</param>
    /// <returns>Product または null</returns>
    public async Task<Book?> SelectByIdWithBookStockAndCategoryAsync(string id)
    {
        try
        {
            // 商品Id(UUID)で商品と在庫、商品カテゴリをジョインして取得する
            var entity = await _context.Books
                .AsNoTracking()
                .Include(b => b.Category)
                .Include(b => b.BookStock)
                .SingleOrDefaultAsync(p => p.BookUuid == id);
            if (entity is null)
            {
                return null; // 該当商品が存在しない場合はnullを返す
            }
            // ProductEntityの集約からProductの集約に復元する
            var product = await _factory.RestoreAsync(entity);
            return product;
        }
        catch (DomainException)
        {
            throw; // DomainException例外はそのまま再スローする
        }
        catch (Exception ex)
        {
            // InternalExceptionにラップしてスローする
            throw new InternalException($"Id:{id}の書籍取得時に予期しないエラーが発生しました。", ex);
        }
    }

    /// <summary>
    /// 指定されたキーワードで商品を部分一致検索して商品と在庫、商品カテゴリを取得する
    /// </summary>
    /// <param name="keyword">検索キーワード</param>
    /// <returns>Prodyctのリスト</returns>
    public async Task<List<Book>> SelectByNameLikeWithBookStockAndCategoryAsync(string keyword)
    {
        try
        {
            // 引数のキーワードで商品と在庫を部分一致検索する
            var entities = await _context.Books
                .AsNoTracking()
                .Include(b => b.BookStock)
                .Include(b => b.Category)
                .Where(b => EF.Functions.Like(b.Title, $"%{keyword}%"))
                .ToListAsync();
            // List<ProductEntity>からList<Product>を復元する
            var books = await _factory.RestoreAsync(entities);
            return books;
        }
        catch (DomainException)
        {
            throw; // DomainException例外はそのまま再スローする
        }
        catch (Exception ex)
        {
            // InternalExceptionにラップしてスローする
            throw new InternalException($"キーワード:{keyword}の書籍取得時に予期しないエラーが発生しました。", ex);
        }
    }

    /// <summary>
    /// 商品を更新する
    /// </summary>
    /// <param name="book">更新対象の商品</param>
    /// <returns>true:更新成功 false:更新失敗</returns>
    public async Task<bool> UpdateByIdAsync(Book book)
    {
        try
        {
            var entity = await _context.Books
            .Include(b => b.BookStock)
            .SingleOrDefaultAsync(b => b.BookUuid == book.BookUuid);
            if (entity is null)
            {
                return false;
            }
            // 商品名と単価を変更する
            entity.Title = book.Title;
            entity.Author = book.Author;
            // 在庫数を変更する
            entity.BookStock!.Stock = book.BookStock!.Stock;
            // 変更データをデータベースに永続化する
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            // InternalExceptionにラップしてスローする
            throw new InternalException($"Id:{book.BookUuid}の書籍変更中に予期しないエラーが発生しました。", ex);
        }
    }

    /// <summary>
    /// 商品を削除する
    /// </summary>
    /// <param name="id">削除対象の商品Id(UUID)</param>
    /// <returns>true:削除成功 false:削除失敗</returns>
    public async Task<bool> DeleteByIdAsync(string id)
    {
        try
        {
            // 削除対象の書籍を取得する
            var entity = await _context.Books.SingleOrDefaultAsync(b => b.BookUuid == id);
            if (entity is null)
            {
                return false; // 該当商品が存在しない場合はfalseを返す
            }
            // 書籍を削除する
            _context.Books.Remove(entity);
            // 削除結果をデータベースに反映させる
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            // InternalExceptionにラップしてスローする
            throw new InternalException($"Id:{id}の書籍削除中に予期しないエラーが発生しました。", ex);
        }
    }

    /// <summary>
    /// 指定された商品名の存在有無を返す
    /// </summary>
    /// <param name="Title">商品名</param>
    /// <returns>true:存在する false:存在しない</returns> 
    public async Task<bool> ExistsByTitleAsync(string title)
    {
        try
        {
            return await _context.Books
            .AsNoTracking()
            .AnyAsync(b => b.Title == title);
        }
        catch (Exception ex)
        {
            // InternalExceptionにラップしてスローする
            throw new InternalException($"Title:{title}の書籍有無取得時に予期しないエラーが発生しました。", ex);
        }
    }
}