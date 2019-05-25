using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAttack : MonoBehaviour
{
    public Attack skillToPerform;
    public void PerformSkill()
    {
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>().Skill(skillToPerform);
    }
}
