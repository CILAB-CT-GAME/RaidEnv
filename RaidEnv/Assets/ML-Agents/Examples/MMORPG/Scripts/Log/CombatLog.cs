using System;

[Serializable]
public struct CombatLog
{
    public int trigTime;
    public int hitTime;
    public AbstractAgent source;
    public AbstractAgent target;
    public AbstractSkill skill;
    public float value;
    public bool isCritical;
    public bool isBackAttack;

    public override string ToString()
    {
        // Debug.Log(source._skillList.Count);
        // Debug.Log(skill);
        AbstractSkill _skill = this.skill;
        int skillIdx = source._skillList.FindIndex(s => s.info.uuid == _skill.info.uuid);

        return String.Format("CombatLog(From: {1}, To: {2}, Skill Id {3}, Damage: {4}) At Frame: {0}",
                            hitTime, source, target, skillIdx, value);
    }
}