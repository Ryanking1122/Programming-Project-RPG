using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PCStateMachine : MonoBehaviour
{
    public PlayerCharacter playerCharacter;
    private BattleStateMachine BSM;

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
    private float curCooldown = 0f;
    private float maxCooldown = 5f;
    public Image progressBar;
    public GameObject turnPointer;
    private Vector2 startPosition;
    //TimeForBattle stuff
    private bool actionStarted = false;
    public GameObject TargetToAttack;
    private float animSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        curCooldown = UnityEngine.Random.Range(0, 2.5f);
        turnPointer.SetActive(false);
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
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
                BSM.HeroManageList.Add(this.gameObject);
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
            curCooldown = curCooldown + Time.deltaTime;
            float calcCooldown = curCooldown / maxCooldown;
            progressBar.transform.localScale = new Vector3(Mathf.Clamp(calcCooldown, 0, 1), progressBar.transform.localScale.y, progressBar.transform.localScale.z);
            if(curCooldown >= maxCooldown)
            {
                currentState = TurnState.ADDTOLIST;
            }
        }
    }

    private IEnumerator TimeForBattle()
    {
        if (actionStarted)
        {
            yield break;
        }
        actionStarted = true;
        //TODO: Animate Enemy toward Target
        Vector2 targetPosition = new Vector2(TargetToAttack.transform.position.x + 1.5f, TargetToAttack.transform.position.y);
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
        BSM.PerformList.RemoveAt(0);
        //TODO: Reset BSM -> Wait
        BSM.battleState = BattleStateMachine.PerformAction.WAIT;
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
