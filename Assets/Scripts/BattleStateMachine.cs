using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public enum HeroGUI
    {
        ACTIVATE,
        WAITING,
        INPUT1,
        INPUT2,
        DONE
    }
    public HeroGUI HeroInput;

    public List<GameObject> HeroManageList = new List<GameObject>();
    private TurnHandler HeroChoice;
    public GameObject enemyButton;
    public Transform spacer;

    // Start is called before the first frame update
    void Start()
    {
        battleState = PerformAction.WAIT;
        EnemiesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        PlayersInBattle.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        EnemyButtons();
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

    void EnemyButtons()
    {
        foreach(GameObject enemy in EnemiesInBattle)
        {
            GameObject newButton = Instantiate(enemyButton) as GameObject;
            EnemySelectButton button = newButton.GetComponent<EnemySelectButton>();
            EnemyStateMachine cur_enemy = enemy.GetComponent<EnemyStateMachine>();
            Text buttonText = newButton.transform.Find("Text").gameObject.GetComponent<Text>();
            buttonText.text = " " + cur_enemy.enemy.name;
            button.EnemyPrefab = enemy;
            newButton.transform.SetParent(spacer);
        }
    }
}
