# 프로그램 소스 해석 및 재활용 관련 포트폴리오

## 기존 프로젝트

해당 프로젝트는 템플릿 완성형, 게임형 에셋입니다.

## 현재 프로젝트 방향

1. 현재 존재하는 프로젝트를 코드를 분석하고 이해한 후
2. 새롭게 가공 및 변형을 하여

- 소스코드 재활용
- 소스코드 변형(리팩토링)

### 코드 분석 및 리팩토링

1. 기존 완성된 코드의 파악을 위해서 실행을 했지만 버튼부터 작동하지 않는상태
   1. 코드가 어디서부터 먹통인지 파악하기위해 중간중간에 로그를 넣으며 코드 흐름을 따라가봄
   2. Game탭을 Simulator탭으로 변경 후 문제점 파악
      1. 'Game'이라고 불리는 Scene이 build profile에 추가되지 않아서 로드하지 못하는 문제
      2. ![alt text](<markdown/Pasted image 20250408105634.png>)
   3. 하지만 다시 game탭으로 변경시 버튼이 눌리지 않는 문제 발생
   4. input 설정을 Old에서 Both로 변경
2. SceneManger에서 불러오는 씬명을 Raw Value에서 Enum을 통한 스트링으로 변경
3. FbMusicPlayer.cs에서
   1. `GetComponent<AudioSource>()`를 계속 사용하는데 해당 가져온 컴포넌트를 저장하여 사용하도록 변경
4. 기존의 오래된 방식의 Transform의 parent 교체 코드를 SetParent 방식으로 변경
5. 이미 비활성화된 객체를 다시 비활성화 하는 메서드를 중간에 예외처리 포함
6. AI Play를 위한 연산은 SingleMode일때만 연산하도록 예외처리
7. 체이닝이 너무 긴 연산을 중간에 끈어서 변수에 담아서 활용하여 불필요한 연산 감소
   1. 이전
      1. ![alt text](<markdown/Pasted image 20250408145428.png>)
   2. 이후
      1. ![alt text](<markdown/Pasted image 20250408145548.png>)
8. 객체가 가지고 있는 변수들을 활용하여 전체중 몇번째 인덱스인지 반환하는 메서드 생성
   ```cs
   public int GetRealIndex()
   {
   	int realIndex = (int)(buttonColorID * BoardManager.instance.boardSize.y) + buttonPositionID;
   	return realIndex;
   }
   ```
9. 하드코딩된 부분 메서드화
   1. 이전
      1. ![alt text](<markdown/Pasted image 20250408142232.png>)
   2. 이후
      1. ![alt text](<markdown/Pasted image 20250408142406.png>)

### 느낀점

1. 나름 어느정도 구성있는 간단한 게임 에셋이라고 생각했는데 코드를 읽는데 생각보다 많은 시간이 들어갔다.
2. 주석이 생각보다 잘 달려있어서 좋았지만 아다르고 어다름으로 인한 혼돈이 혹시 있을까봐 코드 내용을 하나하나 들여다보면서 코드내용이 주석과 맞는 지 교차검증을 하였다.
3. 매개변수 및 반환형에 대한 주석은 없었는데 나중에 메서드를 만들 때 빠른 코드 읽기를 위해서 반황형과 매개변수에 대한 설명 주석도 잘 달아야겠다는 생각이 들었다.
