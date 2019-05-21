using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PCStateMachine : MonoBehaviour
{
    public PlayerCharacter playerCharacter;
    private BattleStateMachine bsm;

    public enum TurnState
    {
        PROCESSING,
        ADDTOLIST,
        WAITING,
        SELECTING,
        ACTION,
        DEAD
    }

    public TurnState currentState;
    private float curTime = 0f;
    private float maxTime = 5f;
    public Image progressBar;
    public GameObject turnPointer;
    private Vector2 startPosition;
    //TimeForBattle stuff
    private bool actionStarted = false;
    public GameObject targetToAttack;
    private float animSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        curTime = UnityEngine.Random.Range(0, 2.5f);
        turnPointer.SetActive(false);
        bsm = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        currentState = TurnState.PROCESSING;
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case (TurnState.PROCESSING):
                UpgradeProgressBar();
                break;

            case (TurnState.ADDTOLIST):
                bsm.heroManageList.Add(this.gameObject);
                currentState = TurnState.WAITING;
                break;

            case (TurnState.DEAD):
                break;

            case (TurnState.WAITING):
                break;

            case (TurnState.ACTION):
                StartCoroutine(TimeForBattle());
                currentState = TurnState.WAITING;
                break;

            default:
                break;
        }

        void UpgradeProgressBar()
        {
            curTime = curTime + Time.deltaTime;
            float calcTime = curTime / maxTime;
            progressBar.transform.localScale = new Vector2(Mathf.Clamp(calcTime, 0, 1), progressBar.transform.localScale.y); //Animates the Progress Bar charging up
            if(curTime >= maxTime) //if Progress Bar is full add the player to the performer list
            {
                currentState = TurnState.ADDTOLIST;
            }
        }
    }

    private IEnumerator TimeForBattle() //Battle Logic
    {
        if (actionStarted)
        {
            yield break;
        }
        actionStarted = true; //Sets actionStarted boolean to true to signify an action has begun
        Vector2 targetPosition = new Vector2(targetToAttack.transform.position.x + 1.5f, targetToAttack.transform.position.y); //Animates player character towards the enemy
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
        bsm.performList.RemoveAt(0); //Removes the current performer from the Performers List
        bsm.battleState = BattleStateMachine.PerformAction.WAIT; //Reset the BattleStateMachine back to Wait
        actionStarted = false; //resets the actionStarted boolean back to false because no action is being performed
        curTime = 0f; //resets time gauge
        currentState = TurnState.PROCESSING;

    }

    private bool MoveTowardEnemy(Vector3 target) //Moves the player character towards the enemy in the battle
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
    private bool MoveTowardStart(Vector3 target) //Moves the player character back to it's original position in the battle
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    public void TakeDamage(float damageValue)
    {
        playerCharacter.currentHP -= damageValue;
        if(playerCharacter.currentHP <= 0)
        {
            currentState = TurnState.DEAD;
        }
    }
}
