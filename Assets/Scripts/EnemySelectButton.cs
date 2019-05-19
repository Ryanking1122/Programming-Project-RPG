using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySelectButton : MonoBehaviour
{
    public GameObject enemyPrefab;

    public void SelectEnemy()
    {
        GameObject.Find("BattleManager").GetComponent<BattleStateMachine>().EnemySelection(enemyPrefab);
        HideEnemySelector();
    }

    public void ShowEnemySelector()
    {
        enemyPrefab.transform.FindChild("TurnPointer").gameObject.SetActive(true);
    }

    public void HideEnemySelector()
    {
        enemyPrefab.transform.FindChild("TurnPointer").gameObject.SetActive(false);
    }
}
