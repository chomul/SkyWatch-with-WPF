using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using SkyWatch.Messages;
using SkyWatch.Services;

namespace SkyWatch.ViewModels;

/// <summary>
/// 설정 화면 ViewModel — 테마, 단위, 언어 설정
/// </summary>
public partial class SettingsViewModel : ViewModelBase, IRecipient<SettingsChangedMessage>
{
    private readonly ISettingsService _settingsService;

    public SettingsViewModel() : this(new SettingsService())
    {
    }

    public SettingsViewModel(ISettingsService settingsService)
    {
        Title = "설정";
        _settingsService = settingsService;
        LoadCurrentSettings();

        // 메시지 수신 등록
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    public void Receive(SettingsChangedMessage message)
    {
        // 다른 곳(예: 홈 화면)에서 설정이 변경되면 UI 상태 업데이트
        LoadCurrentSettings();
    }

    [ObservableProperty]
    private bool _isCelsius;

    [ObservableProperty]
    private bool _isFahrenheit;

    [ObservableProperty]
    private bool _isKorean;

    [ObservableProperty]
    private bool _isEnglish;

    /// <summary>
    /// 현재 설정 로드 및 UI 상태 초기화
    /// </summary>
    public void LoadCurrentSettings()
    {
        IsCelsius = ApiConfig.Units == "metric";
        IsFahrenheit = !IsCelsius;

        IsKorean = ApiConfig.Lang == "kr";
        IsEnglish = !IsKorean;

        // LocalizationManager 언어 설정 동기화
        LocalizationManager.Instance.SetLanguage(ApiConfig.Lang);
    }

    /// <summary>
    /// 단위 변경 (섭씨/화씨)
    /// </summary>
    [RelayCommand]
    private async Task ChangeUnit(string unit)
    {
        if (ApiConfig.Units == unit) return;

        ApiConfig.Units = unit;
        await _settingsService.SaveSettingsAsync();

        // 변경 알림 (값은 구분용 문자열)
        WeakReferenceMessenger.Default.Send(new SettingsChangedMessage("Unit"));
    }

    /// <summary>
    /// 언어 변경 (한국어/영어)
    /// </summary>
    [RelayCommand]
    private async Task ChangeLanguage(string lang)
    {
        if (ApiConfig.Lang == lang) return;

        ApiConfig.Lang = lang;
        await _settingsService.SaveSettingsAsync();

        // LocalizationManager 업데이트
        LocalizationManager.Instance.SetLanguage(lang);

        // 변경 알림
        WeakReferenceMessenger.Default.Send(new SettingsChangedMessage("Lang"));
    }
}
