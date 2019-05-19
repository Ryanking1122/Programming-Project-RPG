﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attack
{
    public string attackName; //Name of the Attack.
    public string attackDescription; //Description of the Attack.
    public float attackDamage; //Damage of the Attack.
    public float attackManaCost; //If it's a Magic Attack, how much MP it uses.
}
