# Multi-Agent

## State
* BufferSensor : float[6, 2] 유동적인 개수의 변수를 담는 센서. FixedUpdate마다 초기화되며 입력되지않은 변수는 0으로 설정된다. ActiveInHierarchy 상태인 PlayerAgent들의 다음 정보를 담는다.
  * Normalized Relative Position.x : x 좌표 / normalize factor
  * Normalized Relative Position.z : z 좌표 / normalize factor
  * Normalized Relative Velocity.x : x 방향 속도 / normalize factor
  * Normalized Relative Velocity.z : z 방향 속도 / normalize factor
  * Current Health : 현재 Hp
  * Skill1 Cooltime Left : 1번스킬의 남은 쿨타임
<!-- 그라운드 -10, 10 인데 normalize factor 20일 필요가 있나, 10이어야 하는거아닌가 -->
<!-- inverse transform 잘 동작하는지 확인해봐야할 필요있음 -->
<!-- MLagent 기본적으로 Normalize 있는지 확인해야함. 없으면 Hp나 쿨타임도 해줘야함-->
* RayPerceptionSensor: 오브젝트 전면에서 부채꼴 모양의 여러 개의 직선을 그려 선상에 있는 오브젝트의 태그를 식별
  * 최대 각 140도, ray 갯수 10개, Ray 최대길이 20
  * Detectable Tags : Wall, Agent, Dragon
<!-- Ray 3개 해야하는지? -->

* VectorSensor : 직접적인 값을 넣어서 observation을 전달
  * Velocity $\cdot$ Forward : 전면으로 가해지는 속도
  * Velocity $\cdot$ Right : 측면으로 가해지는 속도
  * Enemy Relative Position : 적과의 상대적인 위치
  * Enemy Position Vector : 적의 절대적인 위치
  * Signal Relative Position : Signal을 보낸 PlayerAgent의 위치
<!-- 1,2 는 scalar 인데 3,4,5는 vector 어떻게 들어가는지? -->

## Action
* Action 1 : move forward 
* Action 2 : move backward 
* Action 3 : turn right
* Action 4 : turn left
* Action 5 : move left 
* Action 6 : move right
* Action 7 : execute 1st skill 
<!-- 휴리스틱엔 없는 좌우이동이 있음 -->

## Reward
* Shooting Reward : +0.01f
* Hit Reward : +0.05f
* Win Group Reward : 1.0f


<!-- 플랫폼 여러개로 늘렸을 때 정상적인 obs가 들어오는지 확인할 필요있음. 이전에 실험시 학습이 안됐었음-->
<!-- num_env 를 늘렸을 때 학습이 빠르게되지 않았었음. 디버깅 필요 -->
<!-- time scale 늘릴 수 있는 방법을 찾을 필요가 있음. MlAgent는 최대 100배 빠르기가 가능-->
<!-- 최종적으로 몇 개, 어떤 형태로 Obs가 들어가는지 기술할 필요성 -->