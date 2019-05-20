﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class PlayerCharacter : Character
{
    public int abilityPoints; //Points used to buy Abilities in the Ability Shop 
    public List<Form> formList = new List<Form>(); //List of Forms that can be used by the Player Character
}