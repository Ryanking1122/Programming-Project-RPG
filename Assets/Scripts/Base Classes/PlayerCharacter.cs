using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class PlayerCharacter : Character
{
    public int abilityPoints; //Points used to buy Abilities in the Ability Shop 
    public List<Attack> skillList = new List<Attack>(); //List of Skills
    public List<Form> formList = new List<Form>(); //List of Forms that can be used by the Player Character
    public List<Item> itemList = new List<Item>(); //List of Items that can be used in battle.
}