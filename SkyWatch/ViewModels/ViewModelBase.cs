using CommunityToolkit.Mvvm.ComponentModel;

namespace SkyWatch.ViewModels;

/// <summary>
/// 모든 ViewModel의 기본 클래스
/// </summary>
public abstract class ViewModelBase : ObservableObject
{
    private string _title = string.Empty;
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }
}
