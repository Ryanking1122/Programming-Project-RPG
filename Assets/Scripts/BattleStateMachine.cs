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

    public enum PlayerGUI
    {
        ACTIVATE,
        WAITING,
        INPUT1,
        INPUT2,
        DONE
    }
    public PlayerGUI PlayerInput;

    public List<GameObject> HeroManageList = new List<GameObject>();
    private TurnHandler HeroChoice;
    public GameObject enemyButton;
    public Transform spacer;

    public GameObject attackPanel;
    public GameObject enemySelectPanel;

    // Start is called before the first frame update
    void Start()
    {
        battleState = PerformAction.WAIT;
        EnemiesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        PlayersInBattle.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        PlayerInput = PlayerGUI.ACTIVATE;
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
                if(PerformList[0].Type == "Player")
                {
                    Debug.Log("Hero ready to perform");
                    PCStateMachine PSM = performer.GetComponent<PCStateMachine>();
                    PSM.TargetToAttack = PerformList[0].AttackersTarget;
                    PSM.currentState = PCStateMachine.TurnState.ACTION;
                }
                battleState = PerformAction.PERFORMACTION;
                break;
            case (PerformAction.PERFORMACTION):
                break;
        }

        switch (PlayerInput)
        {
            case (PlayerGUI.ACTIVATE):
                if(HeroManageList.Count > 0)
                {
                    HeroManageList[0].transform.Find("TurnPointer").gameObject.SetActive(true);
                    HeroChoice = new TurnHandler();
                    attackPanel.SetActive(true);
                    PlayerInput = PlayerGUI.WAITING;
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

    public void Input1()
    {
        HeroChoice.Attacker = HeroManageList[0].name;
        HeroChoice.AttackerGameObject = HeroManageList[0];
        HeroChoice.Type = "Player";

        attackPanel.SetActive(false);
        enemySelectPanel.SetActive(true);
    }

    public void EnemySelection(GameObject chosenEnemy)
    {
        HeroChoice.AttackersTarget = chosenEnemy;
        PlayerInput = PlayerGUI.DONE;
    }

    public void PlayerInputComplete()
    {
        PerformList.Add(HeroChoice);
        enemySelectPanel.SetActive(false);
        HeroManageList[0].transform.Find("TurnPointer").gameObject.SetActive(false);
        HeroManageList.RemoveAt(0);
        PlayerInput = PlayerGUI.ACTIVATE;
    }
}
