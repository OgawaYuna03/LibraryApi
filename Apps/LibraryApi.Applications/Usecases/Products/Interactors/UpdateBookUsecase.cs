using LibraryApi.Domains.Models;
using LibraryApi.Domains.Repositories;
using LibraryApi.Domains.Exceptions;
using LibraryApi.Application.Usecases.Products.Interfaces;
namespace LibraryApi.Application.Usecases.Products.Interactors;
/// <summary>
/// ユースケース:[図書を変更する]を実現するインターフェイスの実装
/// </summary>
public class UpdateBookUsecase : IUpdateBookUsecase
{
    private readonly IBookRepository _bookRepository;
    private readonly IUnitOfWork _unitOfWork;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="productRepository">図書CRUD操作リポジトリ</param>
    /// <param name="unitOfWork">トランザクション制御機能</param>
    public UpdateBookUsecase(
        IBookRepository productRepository, IUnitOfWork unitOfWork)
    {
        _bookRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// 指定ざれた図書の存在有無を調べる
    /// </summary>
    /// <param name="productName">図書目</param>
    /// <returns>なし</returns>
    /// <exception cref="ExistsException">同一図書名が存在する場合にスローされる</exception>
    public async Task ExistsByTitleNameAsync(string productName)
    {
        // 指定された図書の有無を調べる
        var result = await _bookRepository.ExistsByTitleAsync(productName);
        if (result) // 図書が既に存在する
        {
            throw new ExistsException($"図書名:{productName}は既に存在します。");
        }
    }

    /// <summary>
    /// 指定された図書Idの図書を取得する
    /// クライアント側の[入力画面]で利用するため
    /// </summary>
    /// <param name="id">図書Id</param>
    /// <returns>該当図書、図書在庫、図書カテゴリ</returns>
    /// <exception cref="NotFoundException">該当データが存在しない場合にスローされる</exception>
    public async Task<Book> GetBookByIdAsync(string id)
    {
        var result = await _bookRepository
            .SelectByIdWithBookStockAndCategoryAsync(id);
        if (result is null)
        {
            throw new NotFoundException($"図書Id:{id}の図書は存在しません。");
        }
        return result;
    }

    /// <summary>
    /// 図書を変更するする
    /// </summary>
    /// <param name="product">変更対象対象図書</param>
    /// <returns>なし</returns>
    /// <exception cref="NotFoundException">図書が存在しない場合にスローされる</exception>
    public async Task UpdateBookAsync(Book book)
    {
        // トランザクションを開始する
        await _unitOfWork.BeginAsync();
        try
        {
            var result = await _bookRepository.UpdateByIdAsync(book);
            if (result == false)
            {
                throw new NotFoundException($"図書Id:{book.BookUuid}の図書は存在しないため変更できません。");
            }
            // トランザクションをコミットする
            await _unitOfWork.CommitAsync();
        }
        catch
        {
            // トランザクションをロールバックする
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}