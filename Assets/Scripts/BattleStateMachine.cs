using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStateMachine : MonoBehaviour
{
    public enum PerformAction
    {
        WAIT,
        TAKEACTION,
        PERFORMACTION
    }
    public PerformAction battleState;

    public List<TurnHandler> PerformList = new List<TurnHandler>();
    public List<GameObject> PlayersInBattle = new List<GameObject>();
    public List<GameObject> EnemiesInBattle = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        battleState = PerformAction.WAIT;
        EnemiesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        PlayersInBattle.AddRange(GameObject.FindGameObjectsWithTag("Player"));
    }

    // Update is called once per frame
    void Update()
    {
        switch (battleState)
        {
            case (PerformAction.WAIT):
                if(PerformList.Count > 0)
                {
                    battleState = PerformAction.TAKEACTION;
                }
                break;
            case (PerformAction.TAKEACTION):
                GameObject performer = GameObject.Find(PerformList[0].Attacker);
                if(PerformList[0].Type == "Enemy")
                {
                    EnemyStateMachine ESM = performer.GetComponent<EnemyStateMachine>();
                    ESM.TargetToAttack = PerformList[0].AttackersTarget;
                    ESM.currentState = EnemyStateMachine.TurnState.ACTION;
                }
                battleState = PerformAction.PERFORMACTION;
                break;
            case (PerformAction.PERFORMACTION):
                break;
        }
    }

    public void CollectActions(TurnHandler turnInfo)
    {
        PerformList.Add(turnInfo);
    }
}
