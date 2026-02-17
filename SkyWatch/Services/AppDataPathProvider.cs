using System.IO;

namespace SkyWatch.Services;

/// <summary>
/// 로컬 AppData 하위의 SkyWatch 데이터 파일 경로를 제공하는 헬퍼.
/// </summary>
public static class AppDataPathProvider
{
    private const string AppFolderName = "SkyWatch";

    /// <summary>
    /// 지정한 파일명을 가진 전체 경로를 반환하고, 필요한 경우 디렉터리를 생성합니다.
    /// </summary>
    public static string GetFilePath(string fileName)
    {
        var appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            AppFolderName);

        Directory.CreateDirectory(appDataPath);
        return Path.Combine(appDataPath, fileName);
    }
}

