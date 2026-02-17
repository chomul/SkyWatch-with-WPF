using CommunityToolkit.Mvvm.Messaging.Messages;

namespace SkyWatch.Messages;

/// <summary>
/// 설정(단위/언어) 변경 시 발생하는 메시지
/// </summary>
public class SettingsChangedMessage : ValueChangedMessage<string>
{
    public SettingsChangedMessage(string value) : base(value)
    {
    }
}
