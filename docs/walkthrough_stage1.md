# ✅ Stage 1 완료 — 프로젝트 셋업 & 앱 껍데기

## 생성된 프로젝트 구조

```
SkyWatch/
├── App.xaml                     ← DarkTheme 연동
├── MainWindow.xaml/.cs          ← 커스텀 타이틀바 + 사이드바 + ContentControl
├── Models/                      ← (비어있음, Stage 2에서 사용)
├── ViewModels/
│   ├── ViewModelBase.cs         ← ObservableObject 기반 공통 베이스
│   ├── MainViewModel.cs         ← 네비게이션 커맨드 (Home/Search/Favorites/Settings)
│   ├── HomeViewModel.cs         ← Placeholder
│   ├── SearchViewModel.cs       ← Placeholder
│   ├── FavoritesViewModel.cs    ← Placeholder
│   └── SettingsViewModel.cs     ← Placeholder
├── Views/
│   ├── HomeView.xaml/.cs        ← Placeholder "🏠 홈"
│   ├── SearchView.xaml/.cs      ← Placeholder "🔍 검색"
│   ├── FavoritesView.xaml/.cs   ← Placeholder "⭐ 즐겨찾기"
│   └── SettingsView.xaml/.cs    ← Placeholder "⚙️ 설정"
├── Services/                    ← (비어있음, Stage 2에서 사용)
├── Converters/                  ← (비어있음, Stage 2에서 사용)
├── Themes/
│   └── DarkTheme.xaml           ← 목업 기반 다크 테마 색상 + NavButtonStyle
└── Assets/Icons/, Backgrounds/  ← (비어있음)
```

## 핵심 구현 내용

| 항목 | 설명 |
|------|------|
| **프레임워크** | .NET 8 WPF + CommunityToolkit.Mvvm |
| **MVVM 패턴** | DataTemplate 기반 ViewModel→View 자동 매핑 |
| **커스텀 타이틀바** | 드래그 이동, 더블클릭 최대화, 최소화/최대화/닫기 |
| **사이드바** | RadioButton 기반 네비게이션 (홈/검색/즐겨찾기/설정) |
| **다크 테마** | 목업의 `#050D1A` ~ `#0A1628` 기반 글래스모피즘 스타일 |

## 빌드 결과
✅ `dotnet build` — **에러 0, 경고 0** 빌드 성공

## 실행 방법
```powershell
cd SkyWatch
dotnet run
```
