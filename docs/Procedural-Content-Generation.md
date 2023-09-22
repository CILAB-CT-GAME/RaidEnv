# PCG (Procedural Content Generation)
시뮬레이터의 파라미터를 조정하여 즉시 시뮬레이터에 적용이 가능한 PCG 학습 인터페이스를 생성하였다. 본 인터페이스에서는 진화 알고리즘과 같은 전통적인 PCG 알고리즘 뿐만 아니라 RL기반 PCG, Adverserial PCG 등 다양한 알고리즘을 학습, 및 실행이 가능한데, 이는 시뮬레이터가 그림N과 같은 구조로 되어 있어 동기, 비동기 학습이 모두 가능하며 시뮬레이터의 구성요소를 파라미터화하여 PCG 알고리즘이 접근 가능하게 만들었기 때문이다. 파라미터는 총 N개로 이루어져있으며, 이를 통해 조정가능한 것은 캐릭터의 HP, MP, 스킬의 공격력, 범위, 캐스팅 시간 등이 있으며 자세한 사항은 표N과 같다. 표N의 파라미터는 다양한 게임에 적용하는 것을 목표로 하기 위해 MMORPG 게임 2종(WoW, LostArk)과 FPS 게임 3종(OverWatch, Sudden Attack, PUBG)을 조사하여 공통되는 특징을 추출하였고 조사하지않은 MMORPG를 개발하는 현직 개발자에게 검수를 받아 파라미터의 유효성을 확인하였다. 

Todo : 그림 그리기

-> ML-Agent를 통해 동기/비동기로 돌아가는 것을 표현하는 그림 2장

-> PCG Agent가 콘텐츠를 생성해서 어떻게 환경에 영향을 주는지 표현하는 그림 1장

-> 1차연도 최종발표 PCG PPT 그림 참고

Todo : 표 만들기

중요 : 설명은 HP -> 캐릭터의 HP <-- 이따위로 적지 말 것
* 예제

 분류  | 세부 파라미터 | 설명
 
캐릭터 |      HP      | 캐릭터가 살아있는 동안 받을 수 있는 데미지를 총량

Todo : 제네럴한 작동 매커니즘 설명

그래서 이게 어떻게 작동함?


Todo : PCG 문제 제기하기

그래서 이 환경에서 뭐함?

Todo : PCG 알고리즘 돌려보기

진짜 학습도 되고 동기/비동기로 다 돌아가나?

사용방법 : 논문에 들어가야하나? -> X?

각 생성기 prefab의 인터페이스 값 조절로 생성, 혹은 OverallGenerator에서 source값 대입하여 생성

* OverallGenerator 
    * PCGRandomType: Stat, SKill, Item, Agent
    * PCGTargetNumber: Agent, Enemy, All, AllAgent, Allenemy
    * agentNumber (1, 2, 3..)
    * source * 각 생성기 요소에 대입하는 값, aciton으로 활용될 예정
    * isItRandom * 무작위 스킬 생성
    * public Generate Item/Stat/SKill/ WithParameter (source)
      *  source 대입할 각 요소 ( ex) cooldown, power, range, ..etc) minmax값 설정
      *  source = Mathf.Min (source, max) , source = Mathf.Max(source, min) *minmax 경계 내부 source값 대입
    * SkillGenerator
      * triggerType: Active, Passive
      * magicalSchool: Arcane, Earth, Fire, Frost, Life, Light, Lightning, Shadow, Storm, Water 
      * hitType: Attack, Spell
      * targetTaype: NonTarget, Target, Region
      * projectileType: Missile, Beam, Slash, PillarBlast
      * projectileSize: Tiny, Small, Normal, Mega
      * projectilespeed
      * affectOnAlly
      * affectOnEnemy
      * castTime
      * cost
      * range
      * nowCharge
      * maximumCharge
      * canCastWhileCasting
      * canCastWhileChanneling
      * value
      * hitCount
      * skillnumber (add to SkillList or replace)
    * ItemGenerator
      * type
      * effectedStatus
      * grants
      * coolDown
      * amount
    * StatGenerator
      * health_current
      * health_max
      * health_regen
      * mana_current
      * mana_max
      * mana_regen
      * strength
      * dexterity
      * intelligence
      * critical
      * haste
      * versatility
      * mastery
      * power
      * range
      * speed
      * spell_power
      * armor
      * evasion
      * movespeed

      
