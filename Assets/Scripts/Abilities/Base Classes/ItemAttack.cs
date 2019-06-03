using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAttack : MonoBehaviour
{
    public Item itemToUse;
    public void UseItem()
    {
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>().Item(itemToUse);

    }
}
