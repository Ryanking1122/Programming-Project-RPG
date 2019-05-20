using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    public string name; //Character Name

    public int charLevel; //Character Level

    public float baseHP; //Character's Base Health Point Stat
    public float currentHP; //Character's Current Health Point Stat

    public float baseMP; //Character's Base Magic Point Stat
    public float currentMP; //Character's Current Magic Point Stat

    public float baseStrength; //Character's Base Strength Stat
    public float currentStrength; //Character's Current Strength Stat
    public float baseDefense; //Character's Base Defense Stat
    public float currentDefense; //Character's Current Defense Stat
    public float baseSpeed; //Character's Base Speed Stat
    public float currentSpeed; //Character's Current Speed Stat
    public float baseMagic; //Character's Base Magic Stat
    public float currentMagic; //Character's Current Magic Stat
    public float baseEvasion; //Character's Base Evasion Stat
    public float currentEvasion; //Character's Current Evasion Stat

    public enum Type //Character Element Type
    {
        FIRE,
        WATER,
        LIGHT,
        DARK,
        GRASS,
        STEEL
    }

    public List<Attack> attackList = new List<Attack>(); //List of Attacks that can be used by the character
}
