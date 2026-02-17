# Stage 6: 즐겨찾기 기능 구현 (완료)

## 1. 개요
Stage 6에서는 사용자가 관심 있는 도시를 저장하고 관리할 수 있는 **즐겨찾기(Favorites)** 기능을 구현했습니다. 로컬 JSON 파일을 사용하여 데이터를 영구 저장하며, MVVM 패턴에 맞춰 ViewModel 간의 데이터 동기화 및 메시징을 처리했습니다.

## 2. 주요 변경 사항

### 2.1 Models & Services
- **`FavoriteCity`**: 위도(`Lat`), 경도(`Lon`) 속성을 추가하여 정확한 날씨 조회가 가능하도록 개선.
- **`SearchResult` & `CurrentWeather`**: 즐겨찾기 기능과의 연동을 위해 `CountryCode`, `Lat`, `Lon` 속성 추가 및 매핑 로직 보강.
- **`FavoritesService`**: `favorites.json` 파일을 로드/저장하는 서비스 구현. 앱 시작 시 데이터를 불러오고 변경 시 즉시 저장합니다.
- **`GeocodingService` & `OpenWeatherService`**: API 응답에서 국가 코드 및 좌표를 정확히 추출하도록 로직 업데이트.

### 2.2 ViewModels
- **`MainViewModel`**:
    - `FavoritesService`와 `IWeatherService`를 주입받아 데이터 로드 및 날씨 갱신 처리.
    - `ToggleFavoriteCommand`: 현재 도시가 즐겨찾기에 없으면 추가, 있으면 삭제하는 로직 구현.
    - `FavoriteCities` 컬렉션을 관리하며 `FavoritesViewModel`과 공유.
    - `ToggleFavoriteMessage` 수신 처리.
- **`HomeViewModel`**:
    - `IsFavorite` 속성 추가. `MainViewModel`에서 상태를 업데이트함.
    - `SendToggleFavoriteCommand`: UI에서 버튼 클릭 시 메시지를 보내 `MainViewModel`에 토글 요청.
- **`FavoritesViewModel`**:
    - `MainViewModel`의 `FavoriteCities`를 공유받아 리스트 표시.
    - `SelectFavoriteCommand`: 도시 선택 시 홈 화면으로 이동 및 날씨 로드.
    - `DeleteFavoriteCommand`: 리스트에서 즉시 삭제 기능.

### 2.3 UI (Views)
- **`HomeView.xaml`**: 도시 이름 옆에 즐겨찾기 토글 버튼(★/☆) 추가. `BoolToStarConverter`를 통해 상태 시각화.
- **`FavoritesView.xaml`**: 즐겨찾기 목록을 표시하는 리스트 UI 구현.
- **`MainWindow.xaml`**:
    - 우측 사이드바의 즐겨찾기 목록 아이템을 `Button`으로 변경.
    - 클릭 시 홈 화면으로 이동하며 해당 도시의 날씨를 로드하도록 구현.
    - Glassmorphism 스타일 적용.
    - 국기 이모지 및 일출/일몰 아이콘 색상 수정 (흰색).

### 2.4 데이터 연동 (Data Integration)
- **실제 데이터 사용**: 더미 데이터를 제거하고 OpenWeatherMap API와 완전히 연동.
- **`CountryHelper`**: 국기 이모지 생성 로직을 별도 헬퍼 클래스로 분리하여 공통 사용.
    - `GeocodingService`와 `MainViewModel`에서 활용.
    - 홈 화면에서 추가 시 누락되던 국기 정보를 보완.

## 3. 실행 및 테스트
1. **즐겨찾기 추가**: 홈 화면에서 검색 후 도시 이름 옆의 ☆ 버튼을 클릭하면 ★로 바뀌며 즐겨찾기에 추가됩니다.
2. **즐겨찾기 목록 확인**: 네비게이션 바에서 '즐겨찾기' 탭을 클릭하면 저장된 도시 목록이 표시됩니다.
3. **날씨 확인**: 목록에서 도시를 클릭하면 홈 화면으로 이동하여 해당 도시의 날씨를 로드합니다.
4. **삭제**: 목록의 'X' 버튼을 누르거나 홈 화면에서 ★ 버튼을 다시 누르면 삭제됩니다.
5. **데이터 유지**: 앱을 재시작해도 즐겨찾기 목록이 유지됩니다.

## 4. 다음 단계
- Stage 7: 설정(Settings) 화면 구현 (단위 변경, API 키 관리 등)
