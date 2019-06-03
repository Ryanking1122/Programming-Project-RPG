using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PCStateMachine : MonoBehaviour
{
    public PlayerCharacter playerCharacter; //Object of the Player Character
    private BattleStateMachine bsm; //Object of the BattleStateMachine

    public enum TurnState //States the Player can be in during a Battle
    {
        PROCESSING,
        ADDTOLIST,
        WAITING,
        ACTION,
        DEAD
    }

    public TurnState currentState; //Current State of Character
    private float curTime = 0f; //Current Time for the Wait Bar
    private float maxTime = 5f; //Max Time for the Wait Bar
    private Image progressBar; //Object of the Progress Bar
    public GameObject turnPointer; //Object of the Turn Pointer
    private Vector2 startPosition; //Player Character's Start Position for Animation
    //TimeForBattle stuff
    private bool actionStarted = false; //Boolean used to see if an Action has started
    public GameObject targetToAttack; //Object of the Target to Attack
    private float animSpeed = 10f; //Speed variable for animation of the player character
    private bool isAlive = true; //Boolean to see if character is alive
    private CharPanelInfo charPanelInfo;
    public GameObject charPanel;
    private Transform charPanelSpacer;

    // Start is called before the first frame update
    void Start()
    {
        charPanelSpacer = GameObject.Find("Battle Canvas").transform.Find("CharPanel").Find("CharPanelSpacer");
        PopulateCharacterBar();
        curTime = UnityEngine.Random.Range(0, 2.5f); //Sets the Current Time to anywhere between 0 and half full for the Progress Bar
        turnPointer.SetActive(false); //Makes the Turn Pointer Invisible
        bsm = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        currentState = TurnState.PROCESSING; //Set Current State to Processing
        startPosition = transform.position; //Set the variable for Start Position to the position of the Player Character when the battle begins
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case (TurnState.PROCESSING): //In this state the Progress Bar updates until it is full
                UpdateProgressBar();
                break;

            case (TurnState.ADDTOLIST): //In this state the Player Character is added ro the list of Player Characters able to be managed in the battle
                bsm.heroManageList.Add(this.gameObject);
                currentState = TurnState.WAITING;
                break;

            case (TurnState.DEAD): //In this state the Player Character is Dead and can't be used anymore
                if (!isAlive)
                {
                    return;
                }
                else
                {
                    //Change Tag
                    this.gameObject.tag = "Dead Player";
                    //Make Not Attackable by Enemy
                    bsm.playersInBattle.Remove(this.gameObject);
                    //Not Managable
                    bsm.heroManageList.Remove(this.gameObject);
                    //Deactivate Turn Pointer
                    turnPointer.SetActive(false);
                    //Reset GUI
                    bsm.actionPanel.SetActive(false);
                    bsm.enemySelectPanel.SetActive(false);
                    //If Action was selected, Remove from Perform List
                    if(bsm.playersInBattle.Count > 0)
                    {
                        for (int i = 0; i < bsm.performList.Count; i++)
                        {
                            if (bsm.performList[i].attackerGameObject == this.gameObject)
                            {
                                bsm.performList.Remove(bsm.performList[i]);
                            }
                            if (bsm.performList[i].attackersTarget == this.gameObject)
                            {
                                bsm.performList[i].attackersTarget = bsm.playersInBattle[UnityEngine.Random.Range(0, bsm.playersInBattle.Count)];
                            }
                        }
                    }
                    //Reset PlayerInput
                    bsm.battleState = BattleStateMachine.PerformAction.CHECKIFALIVE;

                    isAlive = false;
                }
                break;

            case (TurnState.WAITING): //This state is used for waiting
                break;

            case (TurnState.ACTION): //In this state the Player Character is performing an action
                StartCoroutine(TimeForBattle());
                currentState = TurnState.WAITING;
                break;

            default:
                break;
        }

        void UpdateProgressBar() //This function updates the Progress Bar until it is full then sets the state to ADDTOLIST
        {
            curTime = curTime + Time.deltaTime; //Increments time bar value
            float calcTime = curTime / maxTime; //Sets the coordinates for the wait bar animation
            progressBar.transform.localScale = new Vector2(Mathf.Clamp(calcTime, 0, 1), progressBar.transform.localScale.y); //Animates the Progress Bar charging up
            if(curTime >= maxTime) //if Progress Bar is full add the player to the performer list
            {
                currentState = TurnState.ADDTOLIST; //Adds the Player Character to the Manage List
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
        while (MoveTowardEnemy(targetPosition)) //Animates the Player Character towards the Enemy
        {
            yield return null;
        }
        //Wait A Bit
        yield return new WaitForSeconds(0.5f); //Wait for the animation to complete
        //Do Damage
        string typeOfAttack = bsm.GetHeroChoice().typeOfAttack;
        PerformDamage(typeOfAttack);
        //Animate back to startPosition
        Vector2 firstPosition = startPosition; //Sets this local variable for the original position of the character to the coordinates of the original position
        while (MoveTowardStart(firstPosition)) //Resets the Player Character to it's original position
        {
            yield return null;
        }
        bsm.performList.RemoveAt(0); //Removes the current performer from the Performers List
        if(bsm.battleState != BattleStateMachine.PerformAction.WIN && bsm.battleState != BattleStateMachine.PerformAction.LOSE)
        {
            bsm.battleState = BattleStateMachine.PerformAction.WAIT;
            curTime = 0f; //resets time gauge
            currentState = TurnState.PROCESSING; //Sets the Current State of the Player Character back to PROCESSING
        }
        else
        {
            currentState = TurnState.WAITING;
        }
        actionStarted = false; //resets the actionStarted boolean back to false because no action is being performed
    }

    private bool MoveTowardEnemy(Vector3 target) //Moves the player character towards the enemy in the battle
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
    private bool MoveTowardStart(Vector3 target) //Moves the player character back to it's original position in the battle
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    public void PerformDamage(string typeOfAttack)
    {
        float damageValue;
        if(typeOfAttack == "Item")
        {
            damageValue = 50;
        }
        else
        {
            damageValue = playerCharacter.currentStrength * (100 / (100 + targetToAttack.GetComponent<EnemyStateMachine>().enemy.currentDefense));
        }
        Debug.Log("Player Damage: " + damageValue);
        targetToAttack.GetComponent<EnemyStateMachine>().TakeDamage(damageValue);
        Debug.Log(targetToAttack.GetComponent<EnemyStateMachine>().enemy.currentHP);
    }

    public void TakeDamage(float damageValue) //This function is used so the Player Character takes Damage
    {
        playerCharacter.currentHP -= damageValue; //Subtract the Damage Value from the Player Character's HP
        if(playerCharacter.currentHP <= 0) //Checks if Player Character is Dead
        {
            playerCharacter.currentHP = 0; //Sets the HP to 0 incase of negative numbers
            currentState = TurnState.DEAD; //If the Player Character is Dead then their Current State is set to Dead
        }
        UpdateCharacterBar();
    }

    void PopulateCharacterBar()
    {
        charPanel = Instantiate(charPanel) as GameObject;
        charPanelInfo = charPanel.GetComponent<CharPanelInfo>();
        charPanelInfo.playerCharName.text = playerCharacter.name;
        charPanelInfo.playerCharHP.text = "HP: " + playerCharacter.currentHP + "/" + playerCharacter.baseHP;
        charPanelInfo.playerCharMP.text = "MP: " + playerCharacter.currentMP + "/" + playerCharacter.baseMP;
        progressBar = charPanelInfo.progressBar;
        charPanel.transform.SetParent(charPanelSpacer, false);
    }

    public void UpdateCharacterBar()
    {
        charPanelInfo.playerCharHP.text = "HP: " + playerCharacter.currentHP + "/" + playerCharacter.baseHP;
        charPanelInfo.playerCharMP.text = "MP: " + playerCharacter.currentMP + "/" + playerCharacter.baseMP;
    }
}
