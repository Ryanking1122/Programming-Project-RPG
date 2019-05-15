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

    // Start is called before the first frame update
    void Start()
    {
        curCooldown = UnityEngine.Random.Range(0, 2.5f);
        turnPointer.SetActive(false);
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        currentState = TurnState.PROCESSING;
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
}
