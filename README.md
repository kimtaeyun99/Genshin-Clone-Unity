# Genshin Impact Clone

캐릭터 교체와 원소 전투를 구현한 Unity 기반 3D 액션 RPG 모작 프로젝트입니다.

플레이어와 몬스터의 행동을 FSM으로 관리하고, 캐릭터의 상태 데이터와 화면 표현을 분리했습니다. 공통 로직의 재사용, 캐릭터별 상태 유지, 이벤트 기반 UI 갱신과 저장 데이터 복구를 중점적으로 구현했습니다.

## 바로가기

- 🎮 [Windows 빌드 다운로드](https://drive.google.com/file/d/144Dv7vXejf76KfAgQ0qCvyvv5m9bW7_Y/view?usp=sharing)
- ▶️ [시연영상 보기](https://youtu.be/dDiDgoJTKw8)
- 💻 [GitHub 저장소](https://github.com/kimtaeyun99/Genshin-Clone-Unity)

## 프로젝트 정보

| 항목 | 내용 |
| --- | --- |
| 개발자 | 김태윤 |
| 개발 형태 | 개인 프로젝트 |
| 개발 기간 | 2026.09.14 ~ 2026.10.16 |
| 장르 | 3D 액션 RPG |
| 플랫폼 | PC / Windows |
| 엔진 | Unity 6000.3.7f1 |
| 언어 | C# |
| 렌더 파이프라인 | URP 17.3.0 |
| 주요 패키지 | Input System 1.18.0, AI Navigation 2.0.9 |
| 버전 관리 | Git / GitHub / Git LFS |

## 주요 기능

### 이동과 스태미나

- 카메라 방향을 기준으로 이동하며 점프, 공중 이동, 낙하와 착지를 처리합니다.
- 회피 시작 시 스태미나를 소모하고, 회피 중에는 무적 시간을 적용합니다.
- 이동하며 회피 입력을 유지하면 대시로 전환하고 스태미나를 지속적으로 소모합니다.
- 스태미나 소모가 멈추면 일정 시간 후 자동으로 회복합니다.
- 스태미나가 행동 시작 기준 미만이면 회피와 대시를 새로 시작할 수 없습니다.
- 이미 진행 중인 대시는 스태미나가 소진되면 종료됩니다.
- 카메라와 지형 사이의 충돌을 감지해 카메라 거리를 조정합니다.

### 파티 교체

- 다이루크와 바바라를 교체하며 전투할 수 있습니다.
- 캐릭터별 HP, 원소 에너지, 스킬·원소폭발 쿨다운을 독립적으로 관리합니다.
- 대기 중인 캐릭터의 쿨다운도 계속 감소합니다.
- 교체 시 캐릭터 모델과 애니메이션을 전환하고 파티 선택 UI를 갱신합니다.
- 교체 쿨다운과 짧은 무적 시간을 적용합니다.
- 사망한 캐릭터로의 교체와 스킬·원소폭발 진행 중 교체를 제한합니다.

### 캐릭터별 전투

| 캐릭터 | 일반 공격 | 원소 스킬 | 원소폭발 |
| --- | --- | --- | --- |
| 다이루크 | 추가 입력으로 연결하는 3단계 물리 공격 | 추가 입력으로 연결하는 3단계 불 원소 공격. 각 타격 단계의 최초 명중 시 에너지 획득 | 에너지를 소모해 전방으로 불 원소 피닉스 투사체 발사 |
| 바바라 | 추가 입력으로 연결하는 4단계 물 원소 탄환 공격 | 에너지 50 획득 및 물고리 소환. 현재 캐릭터를 주기적으로 회복하고 주변에 물 원소 부착 | 에너지를 소모해 생존한 파티원 전체 회복 |

- 공격·스킬·원소폭발 상태에서는 범위 내의 가까운 적을 자동으로 바라봅니다.
- 애니메이션 이벤트로 히트박스 활성화, 투사체 생성과 회복 시점을 연결합니다.
- 바바라의 물고리는 캐릭터를 교체해도 유지되며, 현재 조작 중인 캐릭터를 따라 이동하고 회복시킵니다.

### 원소 반응

대상에게 부착된 원소와 새로 들어온 공격의 원소를 비교해 반응을 판정합니다.

현재 플레이 가능한 두 캐릭터의 물·불 공격으로 증발 반응을 시연할 수 있습니다. 증발을 일으킨 공격의 피해량은 원소 적용 순서와 관계없이 **2배**로 계산합니다.

`ElementReaction`에는 융해·과부하·빙결·감전의 조합 판정도 정의되어 있습니다. 별도의 피해 배율이 적용된 반응은 현재 증발이며, 다른 반응의 지속 피해나 행동 제어 효과는 구현 범위에 포함하지 않습니다.

### 몬스터 AI와 웨이브

- 대기·순찰·추적·공격·사망 상태를 분리합니다.
- NavMeshAgent로 이동하며 탐지 범위, 추적 범위, 공격 거리와 대기 시간에 따라 행동을 전환합니다.
- 근접 몬스터는 단타·연속 공격 패턴을 수행합니다.
- 원거리 몬스터는 위치 이동과 화살 공격을 수행합니다.
- 보스는 주먹·발차기·점프 공격 중 하나를 선택합니다.
- 필드 몬스터는 그룹 전체가 사망한 후 리스폰 대기를 시작합니다.
- 보스 웨이브는 현재 웨이브의 마지막 몬스터가 사망하면 다음 웨이브 대기를 시작합니다.
- 살아 있는 몬스터가 추적을 포기하면 HP와 부착 원소를 초기화합니다.

### 사망과 재시작

- 현재 캐릭터가 사망하면 사망 애니메이션 이후 생존한 파티원으로 전환합니다.
- 모든 파티원이 사망하면 게임오버 화면을 표시합니다.
- 다시 도전하면 파티원의 HP·에너지·쿨다운, 플레이어 위치와 몬스터 스포너를 초기화합니다.

### UI

- 현재 캐릭터의 HP·스태미나·원소 에너지·스킬 쿨다운을 표시합니다.
- 파티원의 HP, 선택 상태와 교체 쿨다운을 표시합니다.
- 몬스터·보스의 HP와 부착 원소를 표시합니다.
- 피격 직후 현재 HP를 반영하고, 지연 체력바가 잠시 후 따라오도록 연출합니다.
- 피해량과 원소 반응 텍스트를 표시합니다.

## 조작법

| 입력 | 동작 |
| --- | --- |
| W / A / S / D | 이동 |
| 마우스 이동 | 카메라 회전 |
| Space | 점프 |
| Left Shift 또는 마우스 오른쪽 버튼 | 회피. 이동하며 입력을 유지하면 대시 |
| 마우스 왼쪽 버튼 | 일반 공격. 추가 입력 시 연계 |
| E | 원소 스킬. 다이루크는 추가 입력 시 연계 |
| Q | 원소폭발 |
| 1 ~ 4 | 등록된 파티 슬롯 선택 |
| Ctrl | 마우스 커서 잠금·해제 |
| Esc | 메인 화면 복귀 확인창 |

현재 캐릭터 데이터는 다이루크·바바라 2종입니다. 1~4번 입력 중 실제 파티원이 등록된 슬롯만 선택할 수 있습니다.

## 주요 설계

### 1. 제네릭 FSM으로 상태 전환 절차 재사용

플레이어와 몬스터는 상태 종류가 다르지만 기존 상태를 종료하고 다음 상태에 진입하는 절차는 같습니다. `StateMachine<TKey>`가 상태 열거형을 제네릭으로 받아 이 절차를 공유하도록 구성했습니다.

| 구성 | 책임 |
| --- | --- |
| `State` | 상태의 `Enter`, `Update`, `Exit` 정의 |
| `StateMachine<TKey>` | 상태 등록과 현재 상태 갱신, 전환 순서 관리 |
| `PlayerStateBase` / `MonsterStateBase` | 각 대상의 상태에 필요한 공통 참조와 동작 제공 |
| 개별 상태 클래스 | 해당 상태의 행동과 전환 조건 처리 |

전환 요청을 받으면 현재 상태의 `Exit()`를 호출한 뒤 새 상태로 교체하고 `Enter()`를 실행합니다. 이후 상태머신의 갱신 호출은 새 상태의 `Update()`로 전달됩니다.

관련 코드:
[StateMachine](Assets/Scripts/Common/StateMachine.cs) ·
[PlayerController](Assets/Scripts/Player/Core/PlayerController.cs) ·
[MonsterController](Assets/Scripts/Monster/Core/MonsterController.cs)

### 2. 캐릭터 데이터와 화면 표현 분리

모델이 비활성화되어도 캐릭터의 HP·에너지·쿨다운은 유지되어야 하므로, 상태를 모델 컴포넌트와 분리했습니다.

| 구성 | 책임 |
| --- | --- |
| `CharacterData` | ScriptableObject 기반 기본 능력치, 배율, 아이콘과 모델 참조 |
| `CharacterRunTime` | 일반 C# 객체로 HP·에너지·쿨다운 관리 및 변경 이벤트 발행 |
| `CharacterManager` | 캐릭터별 RunTime 보관과 전체 쿨다운 갱신 |
| `CharacterView` | 모델·Animator·전투 구현 참조를 캐싱하고 외형 전환 |
| `CharacterParty` | 파티 구성과 선택 인덱스 관리 |

상태 유지와 갱신은 Manager와 RunTime이, 외형 교체는 View가 담당합니다. 모델과 전투 컴포넌트 참조는 최초 생성 시 보관하고 이후 교체에서 재사용합니다.

관련 코드:
[CharacterRunTime](Assets/Scripts/Character/Core/CharacterRunTime.cs) ·
[CharacterManager](Assets/Scripts/Character/Core/CharacterManager.cs) ·
[CharacterView](Assets/Scripts/Character/Core/CharacterView.cs)

### 3. 공통 전투 흐름과 캐릭터별 구현 분리

`PlayerAttackState`, `PlayerSkillState`, `PlayerBurstState`는 상태의 시작·진행·종료 흐름을 담당합니다. 실제 공격 방식은 `ICharacterAttack`, `ICharacterSkill`, `ICharacterBurst`를 구현하는 캐릭터별 컴포넌트가 담당합니다.

몬스터도 `IMonsterAttack`을 통해 근접·원거리·보스의 공격 구현을 연결합니다.

피해량과 원소는 `AttackData`로 전달하며, `Health`에서 원소 반응과 무적 여부를 확인한 뒤 플레이어 또는 몬스터의 상태 데이터에 반영합니다.

관련 코드:
[Interface](Assets/Scripts/Interface) ·
[Health](Assets/Scripts/Common/Health.cs) ·
[AttackData](Assets/Scripts/Data/AttackData.cs)

### 4. 이벤트 기반 UI 갱신과 반복 할당 개선

- `StaminaUI`, `CombatUI`, `PlayerHealthUI`, `PartyUI`는 상태 변경 이벤트를 구독해 수치와 선택 상태를 갱신합니다.
- HP·에너지 텍스트는 표시 값이 바뀔 때 갱신합니다.
- 쿨다운 텍스트는 표시할 소수 첫째 자리 값이 바뀔 때 갱신합니다.
- 몬스터 탐지와 물고리의 원소 부착은 `OverlapSphereNonAlloc` 결과 배열을 재사용합니다.
- `AttackProjectile`은 Collider 형태에 따라 `OverlapBoxNonAlloc` 또는 `OverlapCapsuleNonAlloc`을 사용합니다.
- 히트박스와 투사체는 `HashSet<Health>`로 이미 타격한 대상을 기록해 중복 피해를 방지합니다.

현재 근접 `AttackHitbox`는 `OnTriggerEnter`와 `OnTriggerExit`에서 타격을 시도합니다. 투사체의 Overlap 검사와 구분하여 구성했습니다.

NonAlloc 결과 배열이 가득 차면 확장하므로, 확장 시에는 새 배열 할당이 발생할 수 있습니다.

관련 코드:
[UI](Assets/Scripts/UI) ·
[MonsterPatrolState](Assets/Scripts/Monster/States/MonsterPatrolState.cs) ·
[AttackHitbox](Assets/Scripts/Attack/AttackHitbox.cs) ·
[AttackProjectile](Assets/Scripts/Attack/AttackProjectile.cs)

## 저장과 불러오기

메인 화면으로 복귀하거나 애플리케이션을 종료할 때 진행 상태를 저장하고, 메인 화면의 이어하기로 복원합니다.

| 저장 대상 | 저장 항목 |
| --- | --- |
| 플레이어 | 위치와 Y축 회전 |
| 파티 | 캐릭터 ID 목록과 선택 인덱스 |
| 캐릭터 | ID, 현재 HP와 원소 에너지 |

쿨다운, 스태미나, 몬스터·웨이브 진행 상태와 지속 효과는 저장하지 않습니다.

### 저장 파일 처리

1. 새 데이터를 JSON으로 변환하여 `save.json.tmp`에 기록합니다.
2. 기존 저장 파일이 있으면 `File.Replace`로 교체하고 이전 내용을 `save.json.bak`에 보관합니다.
3. 최초 저장은 임시 파일을 `save.json`으로 이동합니다.
4. 저장 처리 후 남은 임시 파일의 삭제를 시도합니다.

### 복구 순서

1. `save.json` → `save.json.bak` → `save.json.tmp` 순서로 읽기·JSON 변환·필수 데이터 검증을 시도합니다.
2. 백업 또는 임시 파일에서 읽었다면 해당 내용으로 원본 파일 복구를 시도하고 게임 상태를 불러옵니다.
3. 모두 실패하면 기존 파일을 `FailedSaves` 하위 폴더에 보관하려고 시도하고, 새로운 게임을 시작합니다.
4. 실패 파일 보관이 완료되지 않았다면 이후 저장에서도 보관을 먼저 재시도하여 해당 파일의 덮어쓰기를 방지합니다.

### 저장 위치

저장 위치는 `Application.persistentDataPath`입니다.

현재 프로젝트 설정을 기준으로 Windows의 일반적인 경로는 다음과 같습니다.

```text
%USERPROFILE%\AppData\LocalLow\DefaultCompany\Genshin-Clone-Unity\
```

관련 코드:
[SaveManager](Assets/Scripts/System/SaveManager.cs) ·
[GameSaveData](Assets/Scripts/System/GameSaveData.cs) ·
[GameLoadHandler](Assets/Scripts/System/GameLoadHandler.cs)

## 실행 방법

### Windows 빌드

Unity 설치 없이 플레이하려면 상단의 [Windows 빌드 다운로드](https://drive.google.com/file/d/144Dv7vXejf76KfAgQ0qCvyvv5m9bW7_Y/view?usp=sharing)를 이용해 주세요.

아래의 외부 에셋 준비 과정은 Unity에서 소스 프로젝트를 여는 경우에 해당합니다.

### 소스 프로젝트 준비

- Unity Hub 및 Unity **6000.3.7f1**
- Git 및 Git LFS
- 아래 목록의 필수 외부 에셋

```bash
git lfs install
git clone --branch develop https://github.com/kimtaeyun99/Genshin-Clone-Unity.git
cd Genshin-Clone-Unity
git lfs pull
```

### 소스 프로젝트 실행 순서

1. Unity Hub에서 복제한 프로젝트 폴더를 추가하고 지정 버전으로 엽니다.
2. 패키지 설치와 에셋 임포트가 완료될 때까지 기다립니다.
3. 아래 외부 에셋을 임포트하고 기존 폴더 구조와 참조를 복원합니다.
4. Console의 오류와 누락된 참조를 확인합니다.
5. `Assets/Scenes/Main.unity`를 열고 실행합니다.
6. 새 게임 또는 저장 데이터가 있을 때 표시되는 이어하기를 선택합니다.

빌드 설정의 씬 순서는 `Main` → `Field`입니다.

입력 래퍼 `PlayerInputActions`는 `Assets/InputSystem_Actions.inputactions`의 Generate C# Class 설정으로 생성합니다. 입력 클래스 관련 오류가 발생하면 해당 에셋의 생성 설정과 임포트 상태를 확인합니다.

## 사용 에셋 및 실행 안내

본 프로젝트에서 사용하는 외부 에셋은 `Assets/ThirdParty`에서 관리하며, 해당 폴더는 `.gitignore`로 저장소 업로드에서 제외되어 있습니다.

**소스 프로젝트를 실행하려면 아래 에셋을 별도로 준비하고, 기존 폴더 구조와 참조를 복원해야 합니다.**

| 구분 | 에셋 |
| --- | --- |
| 건물 | [Kingdom - Stylized Modular Castle Environment](https://assetstore.unity.com/packages/3d/environments/kingdom-stylized-modular-castle-environment-303689) |
| 캐릭터 | [Low Poly Medieval Fantasy Heroes Basic Pack](https://assetstore.unity.com/packages/3d/characters/humanoids/fantasy/low-poly-medieval-fantasy-heroes-basic-pack-288957) |
| 몬스터 | [Low Poly Medieval Fantasy Biped Creatures Pack](https://assetstore.unity.com/packages/3d/characters/creatures/low-poly-medieval-fantasy-biped-creatures-pack-340684) |
| 환경 | [Stylized Nature Environment Mega Pack](https://assetstore.unity.com/packages/3d/environments/stylized-nature-environment-mega-pack-326646) |
| VFX | [Elemental Spells Full Pack VFX](https://assetstore.unity.com/packages/vfx/particles/spells/elemental-spells-full-pack-vfx-297318) |
| UI | [GUI Mono Round](https://assetstore.unity.com/packages/2d/gui/gui-mono-round-29326) |
| SFX | [Universal Audio Bundle](https://assetstore.unity.com/packages/audio/sound-fx/universal-audio-bundle-287786) |
| SFX | [Pro Sound Collection](https://assetstore.unity.com/packages/audio/sound-fx/pro-sound-collection-50235) |

VFX 에셋의 `PixPlays.ElementalVFX` 코드를 직접 참조하므로, 해당 에셋이 없으면 컴파일 오류가 발생할 수 있습니다. 필수 에셋이 복원되지 않은 상태에서는 정상적인 실행과 빌드가 어려울 수 있습니다.

## 코드 구성

| 경로 | 내용 |
| --- | --- |
| [`Assets/Scripts/Common`](Assets/Scripts/Common) | 공통 State, StateMachine, Health |
| [`Assets/Scripts/Player`](Assets/Scripts/Player) | 플레이어 컨트롤러, 이동·스태미나, 상태별 행동 |
| [`Assets/Scripts/Character`](Assets/Scripts/Character) | 캐릭터 상태·외형·파티 및 캐릭터별 전투 |
| [`Assets/Scripts/Monster`](Assets/Scripts/Monster) | 몬스터 상태·공격·스폰·웨이브 |
| [`Assets/Scripts/Attack`](Assets/Scripts/Attack) | 히트박스, 투사체, 원소 반응, 물고리 |
| [`Assets/Scripts/Data`](Assets/Scripts/Data) | 캐릭터·몬스터 기본 데이터와 공격 데이터 |
| [`Assets/Scripts/System`](Assets/Scripts/System) | 입력, 카메라, 메뉴, 저장·불러오기, 게임오버 |
| [`Assets/Scripts/UI`](Assets/Scripts/UI) | 전투·파티·체력·스태미나·피해량 UI |

## 제작 목적과 사용 에셋의 권리

학습 및 포트폴리오 목적으로 제작한 비공식 모작 프로젝트입니다.

원작 「원신」과 관련된 명칭 및 자료, 프로젝트에 사용한 외부 모델·애니메이션·VFX·UI·오디오 등 에셋의 권리는 각 권리자에게 있습니다.

외부 에셋은 각 라이선스에 따라 사용하며, 본 저장소는 제외된 외부 에셋 파일을 재배포하지 않습니다.

## 개발자

- 김태윤
- GitHub: [kimtaeyun99](https://github.com/kimtaeyun99)
