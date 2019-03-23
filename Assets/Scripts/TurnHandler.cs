using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TurnHandler
{
    public string Attacker; //Name of Attacker
    public string Type;
    public GameObject AttackerGameObject; //GameObject of the Attacker
    public GameObject AttackersTarget; //GameObject for the Target of the Attacker.
}
