# Stage 7: 설정 및 UI 다듬기 (완료)

## 1. 개요
Stage 7에서는 사용자가 앱의 환경을 개인화할 수 있는 **설정(Settings)** 기능을 구현했습니다. 온도 단위(섭씨/화씨)와 언어(한국어/영어)를 변경하면 앱 전체에 즉시 반영되며, 설정값은 영구적으로 저장됩니다.

## 2. 주요 변경 사항

### 2.1 설정 저장 및 관리 (`SettingsService`)
- **JSON 기반 저장**: `Users/.../AppData/Local/SkyWatch/settings.json` 파일에 설정을 저장합니다.
- **`ApiConfig` 연동**: 앱 시작 시 저장된 값을 `ApiConfig.Units`와 `ApiConfig.Lang`에 로드하여 API 호출 시 반영합니다.

### 2.2 설정 화면 UI (`SettingsView`)
- **단위 설정**: 라디오 버튼을 통해 섭씨(°C)와 화씨(°F)를 선택할 수 있습니다.
- **언어 설정**: 한국어와 영어를 지원하며, 선택 시 즉시 앱 내 문자열(요일, 날씨 설명, 국가명 등)이 변경됩니다.
- **Glassmorphism**: 기존 디자인 언어를 유지하며 반투명 카드 스타일을 적용했습니다.

### 2.3 다국어 지원 (Localization)
- **`OpenWeatherService`**: `ApiConfig.Lang`에 따라 API에 `lang` 파라미터를 전달하고, 요일/풍향 등을 해당 언어로 변환하여 반환합니다.
- **`GeocodingService`**: 검색 시 도시 이름을 선택한 언어 우선으로 가져옵니다 (예: 한국어 설정 시 "London" 대신 "런던").
- **`CountryHelper`**: 국가 이름(KR -> 대한민국/South Korea)도 설정된 언어에 맞춰 변환됩니다.

### 2.4 실시간 양방향 동기화 (`SettingsChangedMessage`)
- 설정 변경 시 **`WeakReferenceMessenger`**를 통해 `SettingsChangedMessage`를 전송합니다.
- **`HomeViewModel`** 등 수신자는 즉시 데이터를 새로고침하고 UI 텍스트(단위 라벨 등)를 업데이트합니다.
- **`SettingsViewModel`** 또한 메시지를 수신하여, 홈 화면에서 단위를 변경했을 때 설정 화면의 라디오 버튼 상태를 자동으로 동기화합니다.

## 3. 실행 및 테스트
1. **단위 변경**: 설정 탭에서 '화씨'를 선택하면 홈 화면의 온도가 °F로 바뀌고 수치가 변환됩니다.
2. **언어 변경**: 'English'를 선택하면 요일(월/Tue), 날씨 설명(맑음/Clear sky) 등이 영어로 표시됩니다.
3. **재시작 유지**: 앱을 껐다 켜도 마지막으로 선택한 설정이 유지됩니다.
