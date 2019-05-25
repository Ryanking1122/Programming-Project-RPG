using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TurnHandler
{
    public string attacker; //Name of Attacker
    public string type; //Type of Attacker
    public GameObject attackerGameObject; //GameObject of the Attacker
    public GameObject attackersTarget; //GameObject for the Target of the Attacker.

    public Attack chosenAttack; //The Attack being used
    public Form chosenForm; //The Form Chosen
}
