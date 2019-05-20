using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyAttack : Attack
{
    public HeavyAttack()
    {
        attackName = "Heavy Attack";
        attackDescription = "An Attack with a lot more force behind it";
        attackDamage = 15f;
        attackManaCost = 4f;
    }
}
