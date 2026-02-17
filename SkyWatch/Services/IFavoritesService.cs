using SkyWatch.Models;

namespace SkyWatch.Services;

/// <summary>
/// 즐겨찾기 목록을 저장하고 불러오는 서비스 인터페이스
/// </summary>
public interface IFavoritesService
{
    /// <summary>
    /// 저장된 즐겨찾기 목록을 비동기로 러옵니다.
    /// 파일이 없으면 기본 목록을 반환합니다.
    /// </summary>
    Task<List<FavoriteCity>> LoadFavoritesAsync();

    /// <summary>
    /// 즐겨찾기 목록을 비동기로 저장합니다.
    /// </summary>
    Task SaveFavoritesAsync(List<FavoriteCity> favorites);
}
