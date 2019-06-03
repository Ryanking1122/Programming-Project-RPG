using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    private BattleStateMachine bsm; //Object of Battle State Machine
    public Enemy enemy; //Object of the Enemy

    public enum TurnState //All the different States for the Enemy
    {
        PROCESSING,
        CHOOSEACTION,
        WAITING,
        ACTION,
        DEAD
    }

    public TurnState currentState; //Current State of the Enemy
    private float curWait = 0f; //Current Wait Time
    private float maxWait = 5f; //Max Wait Time
    public GameObject turnPointer; //Object of the Enemy Pointer
    private Vector2 startPosition; //Start Position of the Enemy
    //TimeForBattle stuff
    private bool actionStarted = false; //Boolean for checking whether an action has started
    public GameObject targetToAttack; //Stores the Target of the Enemy
    private float animSpeed = 10f; //Animation Speed
    private bool isAlive = true; //Boolean for checking if the Enemy is alive

    // Start is called before the first frame update
    void Start()
    {
        currentState = TurnState.PROCESSING; //Sets the Enemy State to Processing to let the Wait Bar fill
        turnPointer.SetActive(false); //Disables the Enemy Pointer
        bsm = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>(); //Instantiates the Battle State Machine
        startPosition = transform.position; //Sets the Start Position of the Enemy
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(currentState);
        switch (currentState)
        {
            case (TurnState.PROCESSING): //State for when the wait bar is filling
                UpdateProgressBar(); //Updates the Wait Time
                break;

            case (TurnState.CHOOSEACTION): //State for Choosing an action for the enemy to perform
                ChooseAction(); //Randomly choose an action
                currentState = TurnState.WAITING; //Sets the state to Waiting
                break;

            case (TurnState.ACTION): //State for starting the chosen action
                StartCoroutine(TimeForBattle()); //Starts the selected action
                currentState = TurnState.WAITING;
                break;

            case (TurnState.DEAD): //Dead State
                if (!isAlive)
                {
                    return;
                }
                else
                {
                    //Change Tag to Dead Enemy
                    this.gameObject.tag = "Dead Enemy";
                    //Make Not Attackable by Player Character
                    bsm.enemiesInBattle.Remove(this.gameObject);
                    //If Action was selected, Remove from Perform List
                    if(bsm.enemiesInBattle.Count > 0)
                    {
                        for (int i = 0; i < bsm.performList.Count; i++)
                        {
                            if (bsm.performList[i].attackerGameObject == this.gameObject)
                            {
                                bsm.performList.Remove(bsm.performList[i]);
                            }

                            if (bsm.performList[i].attackersTarget == this.gameObject)
                            {
                                bsm.performList[i].attackersTarget = bsm.enemiesInBattle[Random.Range(0, bsm.enemiesInBattle.Count)];
                            }
                        }
                    }
                    isAlive = false;
                    //Reset Enemy Buttons
                    bsm.EnemyButtons();
                    //Check if the battle has been won
                    bsm.battleState = BattleStateMachine.PerformAction.CHECKIFALIVE;
                }
                break;

            case (TurnState.WAITING): //Waiting State
                break;

            default:
                break;
        }

        void UpdateProgressBar() //Sets the time for the enemy to perform an attack
        {
            curWait = curWait + Time.deltaTime; //Increments the current wait over time
            if (curWait >= maxWait)
            {
                currentState = TurnState.CHOOSEACTION; //Sets the Enemy State to Choose Action once the Wait bar is full
            }
        }
    }

    void ChooseAction() //Chooses the attack for the Enemy and sends it to the Turn Handler
    {
        TurnHandler myAttack = new TurnHandler(); //Instantiation of TurnHandler Class to collect the Attacker Information
        myAttack.attacker = enemy.name; //Sets the Attacker name
        myAttack.type = "Enemy"; //Sets the Attacker Type
        myAttack.attackerGameObject = this.gameObject; //Sets the Attacker Game Object
        myAttack.attackersTarget = bsm.playersInBattle[Random.Range(0, bsm.playersInBattle.Count)]; //Sets the Target of the Attacker
        int attackIndex = Random.Range(0, enemy.attackList.Count); //Randomly perform an attack from the available attacks the enemy has available
        myAttack.chosenAttack = enemy.attackList[attackIndex]; //Set the random attack
        Debug.Log(this.gameObject + " has chosen " + myAttack.chosenAttack.attackName + " and inflicts " + myAttack.chosenAttack.attackDamage + " damage");

        bsm.CollectActions(myAttack); //Send the Attacker Information to the BattleStateMachine
    }

    private IEnumerator TimeForBattle() //Battle Logic
    {
        if (actionStarted)
        {
            yield break;
        }
        actionStarted = true; //Set ActionStarted Boolean to true to signify an action starting
        Vector2 targetPosition = new Vector2(targetToAttack.transform.position.x-1.5f, targetToAttack.transform.position.y); //Animate Enemy moving towards the player character
        while (MoveTowardEnemy(targetPosition)) //Animates the Enemy toward the Player Character
        {
            yield return null;
        }
        //Wait A Bit
        yield return new WaitForSeconds(0.5f); //Wait for Animation to complete
        PerformDamage(); //Performs Damage to the Player Character
        Vector2 firstPosition = startPosition; //Sets local variable for the original position of the Enemy
        while (MoveTowardStart(firstPosition)) //Animate Enemy back to original battle position
        {
            yield return null;
        }

        //Remove from Performer List
        bsm.performList.RemoveAt(0); //Removes the Enemy from the Performer List
        //Reset Battle State Machine to Wait
        bsm.battleState = BattleStateMachine.PerformAction.WAIT; //Sets the next performer b to the Wait State
        actionStarted = false; //Sets the actionStarted Boolean back to false
        curWait = 0f; //Resets wait bar value to 0
        currentState = TurnState.PROCESSING; //sets the Enemy state to Processing for the wait bar to fill
    }

    private bool MoveTowardEnemy(Vector3 target) //Animates the Enemy toward the Player Character
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
    private bool MoveTowardStart(Vector3 target)//Animates the Enemy back to it's original position
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    void PerformDamage() //Performs Damage to the Player Character
    {
        float damageValue = enemy.currentStrength *(100/(100 + targetToAttack.GetComponent<PCStateMachine>().playerCharacter.currentDefense)); //Formula for calculating basic damage
        Debug.Log(damageValue);
        targetToAttack.GetComponent<PCStateMachine>().TakeDamage(damageValue); //Applies the Damage
    }

    public void TakeDamage(float damageValue) //This function is used so the Enemy takes Damage
    {
        enemy.currentHP -= damageValue; //Subtract the Damage Value from the Enemy's HP
        if (enemy.currentHP <= 0) //Checks if Enemy is Dead
        {
            enemy.currentHP = 0; //Sets the HP to 0 incase of negative numbers
            currentState = TurnState.DEAD; //If the Enemy is Dead then their Current State is set to Dead
        }
    }
}
