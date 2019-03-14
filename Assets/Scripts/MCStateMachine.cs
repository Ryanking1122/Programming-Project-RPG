using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MCStateMachine : MonoBehaviour
{
    public MainCharacter mainCharacter;

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

    // Start is called before the first frame update
    void Start()
    {
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
                break;

            case (TurnState.DEAD):
                break;

            case (TurnState.SELECTING):
                break;

            case (TurnState.WAITING):
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
