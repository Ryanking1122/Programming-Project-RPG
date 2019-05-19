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

    public List<TurnHandler> performList = new List<TurnHandler>();
    public List<GameObject> playersInBattle = new List<GameObject>();
    public List<GameObject> enemiesInBattle = new List<GameObject>();

    public enum PlayerGUI
    {
        ACTIVATE,
        WAITING,
        INPUT1,
        INPUT2,
        DONE
    }
    public PlayerGUI playerInput;

    public List<GameObject> heroManageList = new List<GameObject>();
    private TurnHandler heroChoice;
    public GameObject enemyButton;
    public Transform spacer;

    public GameObject attackPanel;
    public GameObject enemySelectPanel;

    // Start is called before the first frame update
    void Start()
    {
        battleState = PerformAction.WAIT;
        enemiesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        playersInBattle.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        playerInput = PlayerGUI.ACTIVATE;
        attackPanel.SetActive(false);
        enemySelectPanel.SetActive(false);
        EnemyButtons();
    }

    // Update is called once per frame
    void Update()
    {
        switch (battleState)
        {
            case (PerformAction.WAIT):
                if(performList.Count > 0)
                {
                    battleState = PerformAction.TAKEACTION;
                }
                break;
            case (PerformAction.TAKEACTION):
                GameObject performer = GameObject.Find(performList[0].attacker);
                if(performList[0].type == "Enemy")
                {
                    EnemyStateMachine ESM = performer.GetComponent<EnemyStateMachine>();
                    ESM.targetToAttack = performList[0].attackersTarget;
                    ESM.currentState = EnemyStateMachine.TurnState.ACTION;
                }
                if(performList[0].type == "Player")
                {
                    Debug.Log("Hero ready to perform");
                    PCStateMachine PSM = performer.GetComponent<PCStateMachine>();
                    PSM.targetToAttack = performList[0].attackersTarget;
                    PSM.currentState = PCStateMachine.TurnState.ACTION;
                }
                battleState = PerformAction.PERFORMACTION;
                break;
            case (PerformAction.PERFORMACTION):
                break;
        }

        switch (playerInput)
        {
            case (PlayerGUI.ACTIVATE):
                if(heroManageList.Count > 0)
                {
                    heroManageList[0].transform.Find("TurnPointer").gameObject.SetActive(true);
                    heroChoice = new TurnHandler();
                    attackPanel.SetActive(true);
                    playerInput = PlayerGUI.WAITING;
                }
                break;

            case (PlayerGUI.WAITING):
                break;

            case (PlayerGUI.DONE):
                PlayerInputComplete();
                break;
        }
    }

    public void CollectActions(TurnHandler turnInfo)
    {
        performList.Add(turnInfo);
    }

    void EnemyButtons()
    {
        foreach(GameObject enemy in enemiesInBattle)
        {
            GameObject newButton = Instantiate(enemyButton) as GameObject;
            EnemySelectButton button = newButton.GetComponent<EnemySelectButton>();
            EnemyStateMachine cur_enemy = enemy.GetComponent<EnemyStateMachine>();
            Text buttonText = newButton.transform.Find("Text").gameObject.GetComponent<Text>();
            buttonText.text = " " + cur_enemy.enemy.name;
            button.enemyPrefab = enemy;
            newButton.transform.SetParent(spacer);
        }
    }

    public void Input1()
    {
        heroChoice.attacker = heroManageList[0].name;
        heroChoice.attackerGameObject = heroManageList[0];
        heroChoice.type = "Player";

        attackPanel.SetActive(false);
        enemySelectPanel.SetActive(true);
    }

    public void EnemySelection(GameObject chosenEnemy)
    {
        heroChoice.attackersTarget = chosenEnemy;
        playerInput = PlayerGUI.DONE;
    }

    public void PlayerInputComplete()
    {
        performList.Add(heroChoice);
        enemySelectPanel.SetActive(false);
        heroManageList[0].transform.Find("TurnPointer").gameObject.SetActive(false);
        heroManageList.RemoveAt(0);
        playerInput = PlayerGUI.ACTIVATE;
    }
}
