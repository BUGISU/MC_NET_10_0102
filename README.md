# 💧 물의 상태 변화 관찰하기 - Unity 3D 과학 시뮬레이션 콘텐츠

> **대상**: 초등학교 1학년  
> **목표**: 물의 상태 변화(고체-액체-기체) 과정을 시각적으로 관찰하고 실험적으로 체험할 수 있도록 제작된 교육용 Unity 콘텐츠  
> **플랫폼**: Unity (URP + Leia Lume Pad 2)  
> **주요 기능**: 상태 변화 시뮬레이션, 나레이션, 퀴즈, 요약 정리, 설정 조절, 순환 사이클

---

## ✨ 콘텐츠 주요 특징

| 항목 | 내용 |
|------|------|
| 🔁 **상태 변화** | 고체 → 액체 → 기체 → 액체 → 고체의 과정을 버튼 클릭을 통해 시각적으로 체험 |
| 🎙️ **음성 나레이션** | 각 상태 변화 과정마다 음성 및 설명 텍스트 제공 |
| 🎮 **직접 조작** | 실험 버튼, 슬라이더, 팝업 등을 통해 아이가 직접 탐구 |
| 📘 **요약 보기** | 모든 실험이 끝난 후 요약 팝업으로 복습 가능 |
| ❓ **개념 퀴즈** | 선택형 과학 퀴즈 제공 (정답/오답 피드백 포함) |
| 🌫️ **물의 순환** | 증발-응결-강수-모임 등 순환 과정을 카메라 이동과 팝업으로 설명 |
| ⚖️ **무게와 부피** | 상태에 따라 변화하는 물질의 부피와 무게에 대한 비교 정보 제공 |

---

## 🧪 콘텐츠 흐름 구성

```

인트로 ▶ 상태 변화 실험 ▶ 퀴즈 ▶ 물의 순환 ▶ 무게와 부피 ▶ 종료 요약

```

각 씬(Scene)은 다음 키로 구분됩니다:
- `00IntroScene`
- `01WaterStateChangeScene`
- `02MassVolumeScene`
- `03WaterCycleScene`
- `04QuizScene` :contentReference[oaicite:0]{index=0}

---

## 🧩 핵심 기능별 구조

### 1️⃣ 상태 변화 실험

| 구성 요소 | 스크립트 | 기능 설명 |
|-----------|-----------|------------|
| 상태 변화 제어 | `ExperimentalController.cs` | 고체 → 액체 → 기체 등 단계별 실험 실행, 파티클/애니메이션 동기화 :contentReference[oaicite:1]{index=1} |
| UI 버튼 연동 | `WaterStateChange_UIManager.cs` | 실험 버튼 활성화/비활성화 관리, 요약 버튼 연결, 실험 완료 이벤트 처리 :contentReference[oaicite:2]{index=2} |
| 개념 설명 | `NarrationController.cs` | 각 상태 변화마다 RichText 기반 설명 및 음성 출력, 타이핑 연출 및 스킵 지원 :contentReference[oaicite:3]{index=3} |
| 요약 보기 | `SummaryPopupController.cs` + `PopupPageData.cs` | 마지막에 정리된 설명을 페이지별로 보여주는 팝업 UI 구현 :contentReference[oaicite:4]{index=4}:contentReference[oaicite:5]{index=5} |

---

### 2️⃣ 퀴즈 콘텐츠

| 구성 요소 | 스크립트 | 기능 설명 |
|-----------|-----------|------------|
| 퀴즈 제어 | `QuizSettingController.cs` | 보기 선택, 정답 체크, 오답 피드백, 정답 강조, 문제 넘기기, 마지막 효과 애니메이션 포함 :contentReference[oaicite:6]{index=6} |

---

### 3️⃣ 물의 순환 시각화

| 구성 요소 | 스크립트 | 기능 설명 |
|-----------|-----------|------------|
| UI/카메라 이동 | `WaterCycle_UIManager.cs` | 버튼 클릭 시 카메라 이동 및 팝업 활성화, 자연음 재생 포함 :contentReference[oaicite:7]{index=7} |
| 애니메이션 요소 | `CloudMoveLoop.cs`, `CycleArrowFlow.cs` | 구름 이동 및 순차적 화살표 흐름 연출 :contentReference[oaicite:8]{index=8}:contentReference[oaicite:9]{index=9} |

---

### 4️⃣ 질량과 부피 비교 실험

| 구성 요소 | 스크립트 | 기능 설명 |
|-----------|-----------|------------|
| 상태 시뮬레이션 | `MassVolumeController.cs` | 상태 변화에 따른 물체 시각 연출, 무게/부피 비교 텍스트 출력, DOTween 애니메이션 적용 :contentReference[oaicite:10]{index=10} |

---

### 5️⃣ 시스템 UI 및 관리

| 구성 요소 | 스크립트 | 기능 설명 |
|-----------|-----------|------------|
| 씬 관리 | `GameManager.cs` | 공통 팝업 토글, 씬 로딩, PlayerPrefs에 저장된 설정 초기화 :contentReference[oaicite:11]{index=11} |
| 일시정지 팝업 | `PausePopupController.cs` | 게임 일시정지, 재시작, 설정, 종료 기능 UI 연결 :contentReference[oaicite:12]{index=12} |
| 로고 애니메이션 | `SplashLogoController.cs` | 시작 시 로고 출력 애니메이션 (DOTween) 및 사운드 재생 :contentReference[oaicite:13]{index=13} |

---

## 🎵 사운드 시스템

| 구성 요소 | 스크립트 | 기능 설명 |
|-----------|-----------|------------|
| 사운드 싱글톤 | `SoundManager.cs` | SFX / BGM / 자연 소리 / VOICE 4채널 분리, PlayerPrefs 저장, Play/Stop 메서드 구성 :contentReference[oaicite:14]{index=14} |
| 사운드 설정 UI | `SettingManager.cs`, `UpdateSettingSliderLabel.cs` | 슬라이더 및 토글을 통한 볼륨, 3D 효과 조절 및 실시간 반영 :contentReference[oaicite:15]{index=15}:contentReference[oaicite:16]{index=16} |

---

## 🛠️ 기술 스택 및 라이브러리

| 항목 | 사용 도구 |
|------|-----------|
| 엔진 | Unity 2022.3 LTS |
| 3D SDK | Leia SDK (Lume Pad 2 디바이스용 입체 시각화) |
| UI | TextMeshPro, Unity UI |
| 애니메이션 | DOTween (카메라, 오브젝트, 파티클 등 모든 애니메이션에 적용) |
| 데이터 저장 | PlayerPrefs |
| 오디오 | AudioSource 기반의 4채널 제어 (BGM, 효과음, 자연음, 음성) |
| 기타 | ScriptableObject로 설명 페이지 구성

---

## 📂 프로젝트 디렉토리 구조

```

Assets/
┣ Scripts/
┃ ┣ Core/
┃ ┣ UI/
┃ ┣ WaterStateChange/
┃ ┣ WaterCycle/
┃ ┣ MassVolume/
┃ ┗ Shared/
┣ Resources/
┃ ┗ Sounds/
┣ Scenes/
┣ Prefabs/
┗ SO/    ← PopupPageData 저장 폴더

```

---

## 📌 주의 사항

- 본 프로젝트는 **Leia Lume Pad 2 디바이스**를 전제로 개발되었습니다. 일반 디스플레이 환경에서는 입체 효과를 확인할 수 없습니다.
- 일부 기능은 LeiaDisplay 또는 DOTween 패키지가 설치되어 있어야 정상 작동합니다.

---

## 👩‍🏫 교육적 의의

이 콘텐츠는 단순히 상태 변화를 보여주는 것이 아니라 **직접 조작하면서 자연스럽게 개념을 체득**하도록 설계되었습니다.  
특히:
- 상태 변화의 순서와 의미를 반복해서 경험
- 상태에 따라 달라지는 부피/질량 개념 이해
- 순환적 흐름(물의 순환)을 통해 환경 개념까지 연결
- 퀴즈와 요약을 통해 학습 마무리까지 완성

---
