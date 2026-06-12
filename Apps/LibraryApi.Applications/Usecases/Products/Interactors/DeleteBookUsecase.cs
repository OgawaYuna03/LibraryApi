using LibraryApi.Domains.Models;
using LibraryApi.Domains.Repositories;
using LibraryApi.Domains.Exceptions;
using LibraryApi.Application.Usecases.Products.Interfaces;
using System.Runtime.Serialization;
namespace LibraryApi.Application.Usecases.Products.Interactors;

public class DeleteBookUsecase : IDeleteBookUsecase
{
    private readonly IUnitOfWork ?_unitOfWork;
    private readonly IBookRepository ?_bookRepository;
     public DeleteBookUsecase(
        
        IBookRepository bookRepository,
        IUnitOfWork unitOfWork)
    {
       
        _bookRepository = bookRepository;
        _unitOfWork = unitOfWork;
    }
     public async Task ExistsByIdAsync(string bookId)
    {
        // 指定された図書の有無を調べる
        var result = await _bookRepository!.ExistsByIdAsync(bookId);
        if (!result) // 図書が既に存在する
        {
            throw new NotFoundException($"図書Id:{bookId}は既に存在しません。");
        }
    }
    public async Task DeleteByIdAsync(string id)
    {
       // トランザクションを開始する
        await _unitOfWork!.BeginAsync();
        try
        {
           
            // 図書を削除する
            await _bookRepository!.DeleteByIdAsync(id);
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