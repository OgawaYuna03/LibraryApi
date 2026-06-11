using LibraryApi.Domains.Models;
namespace LibraryApi.Application.Usecases.Products.Interfaces;

/// <summary>
/// ユースケース:[書籍を削除する]を実現するインターフェイス
/// </summary>
public interface IDeleteBookUsecase
{
    Task<List<Category>> GetCategoriesAsync();

    

}