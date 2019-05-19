using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    private BattleStateMachine bsm;
    public Enemy enemy;

    public enum TurnState
    {
        PROCESSING,
        CHOOSEACTION,
        WAITING,
        ACTION,
        DEAD
    }

    public TurnState currentState;
    private float curCooldown = 0f;
    private float maxCooldown = 5f;
    public GameObject turnPointer;
    private Vector2 startPosition;
    //TimeForBattle stuff
    private bool actionStarted = false;
    public GameObject targetToAttack;
    private float animSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        currentState = TurnState.PROCESSING;
        turnPointer.SetActive(false);
        bsm = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(currentState);
        switch (currentState)
        {
            case (TurnState.PROCESSING):
                UpgradeProgressBar();
                break;

            case (TurnState.CHOOSEACTION):
                ChooseAction();
                currentState = TurnState.WAITING;
                break;

            case (TurnState.ACTION):
                StartCoroutine(TimeForBattle());
                currentState = TurnState.WAITING;
                break;

            case (TurnState.DEAD):
                break;

            case (TurnState.WAITING):
                break;

            default:
                break;
        }

        void UpgradeProgressBar()
        {
            curCooldown = curCooldown + Time.deltaTime;
            if (curCooldown >= maxCooldown)
            {
                currentState = TurnState.CHOOSEACTION;
            }
        }
    }
    void ChooseAction()
    {
        TurnHandler myAttack = new TurnHandler();
        myAttack.attacker = enemy.name;
        myAttack.type = "Enemy";
        myAttack.attackerGameObject = this.gameObject;
        myAttack.attackersTarget = bsm.playersInBattle[Random.Range(0, bsm.playersInBattle.Count)];
        bsm.CollectActions(myAttack);
    }

    private IEnumerator TimeForBattle()
    {
        if (actionStarted)
        {
            yield break;
        }
        actionStarted = true;
        //TODO: Animate Enemy toward Target
        Vector2 targetPosition = new Vector2(targetToAttack.transform.position.x-1.5f, targetToAttack.transform.position.y);
        while (MoveTowardEnemy(targetPosition))
        {
            yield return null;
        }
        //TODO: Wait A Bit
        yield return new WaitForSeconds(0.5f);
        //TODO: Do Damage

        //TODO: Animate back to startPosition
        Vector2 firstPosition = startPosition;
        while (MoveTowardStart(firstPosition))
        {
            yield return null;
        }

        //TODO: Remove from Performer List
        bsm.performList.RemoveAt(0);
        //TODO: Reset bsm -> Wait
        bsm.battleState = BattleStateMachine.PerformAction.WAIT; 
        actionStarted = false;
        curCooldown = 0f;
        currentState = TurnState.PROCESSING;

    }

    private bool MoveTowardEnemy(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
    private bool MoveTowardStart(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
}
