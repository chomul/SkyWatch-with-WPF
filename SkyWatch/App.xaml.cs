using System.Configuration;
using System.Data;
using System.Windows;

namespace SkyWatch;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // WPF XamlParseException 등의 정확한 원인(InnerException)을 확인하기 위한 전역 예외 처리기
        this.DispatcherUnhandledException += (s, args) =>
        {
            Exception ex = args.Exception;
            // 가장 깊은 진짜 원인(InnerException) 찾기
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }

            MessageBox.Show($"UI 및 실행 중 오류가 발생했습니다.\n\n[원인]\n{ex.Message}\n\n[위치]\n{ex.StackTrace}",
                            "치명적 오류 발생",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
            args.Handled = true;
        };

        SkyWatch.Services.ApiConfig.Load();
    }
}

