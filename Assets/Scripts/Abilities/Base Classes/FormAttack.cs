using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormAttack : MonoBehaviour
{
    public Form formToEnter;
    public void EnterForm()
    {
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>().Form(formToEnter);
    }
}
