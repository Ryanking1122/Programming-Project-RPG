using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : Attack
{
    public Fire()
    {
        attackName = "Fire";
        attackDescription = "A Fire based Magic Ability";
        attackDamage = 10f;
        attackManaCost = 10;
    }
}
