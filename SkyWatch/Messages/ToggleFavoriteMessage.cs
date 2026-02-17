using CommunityToolkit.Mvvm.Messaging.Messages;

namespace SkyWatch.Messages;

/// <summary>
/// 즐겨찾기 추가/제거 요청 메시지
/// 값으로는 현재 날씨(CurrentWeather) 또는 검색 결과(SearchResult) 객체를 전달합니다.
/// </summary>
public class ToggleFavoriteMessage : ValueChangedMessage<object>
{
    public ToggleFavoriteMessage(object value) : base(value)
    {
    }
}
