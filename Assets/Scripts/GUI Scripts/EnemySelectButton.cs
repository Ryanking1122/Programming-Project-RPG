using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySelectButton : MonoBehaviour
{
    public GameObject enemyPrefab; //Object of the Enemy Prefab used in Unity

    public void SelectEnemy() //Selects the enemy that is in the corresponding button for an action to be performed on.
    {
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>().EnemySelection(enemyPrefab);
        HideEnemySelector(); //Fixes the issue of pointer not disappearing when button is pushed
    }

    public void ShowEnemySelector() //Activates the enemy selectors whenever the mouse hovers over the button representing the enemy
    {
        enemyPrefab.transform.Find("TurnPointer").gameObject.SetActive(true); //Activates the Pointer
    }

    public void HideEnemySelector() //Deactivates the enemy selectors whenever the mouse leaves the button representing the enemy
    {
        enemyPrefab.transform.Find("TurnPointer").gameObject.SetActive(false); //Deactivates the Pointer
    }
}
