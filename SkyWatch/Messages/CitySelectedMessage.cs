using CommunityToolkit.Mvvm.Messaging.Messages;
using SkyWatch.Models;

namespace SkyWatch.Messages;

public class CitySelectedMessage : ValueChangedMessage<SearchResult>
{
    public CitySelectedMessage(SearchResult city) : base(city)
    {
    }
}
