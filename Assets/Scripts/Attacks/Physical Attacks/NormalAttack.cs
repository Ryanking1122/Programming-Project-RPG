using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttack : Attack
{
    public NormalAttack()
    {
        attackName = "Attack";
        attackDescription = "The normal base attack";
        attackManaCost = 0;
        attackDamage = 10f; //playerCurStrength * (100/(100+enemyCurDef))
    }
}
