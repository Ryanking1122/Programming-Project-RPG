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
        //Debug.Log(currentState);
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

        void UpgradeProgressBar() //Sets the time for the enemy to perform an attack
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
        TurnHandler myAttack = new TurnHandler(); //Instantiation of TurnHandler Class to collect the Attacker Information
        myAttack.attacker = enemy.name; //Sets the Attacker name
        myAttack.type = "Enemy"; //Setst the Attacker Type
        myAttack.attackerGameObject = this.gameObject; //Sets the Attacker Game Object
        myAttack.attackersTarget = bsm.playersInBattle[Random.Range(0, bsm.playersInBattle.Count)]; //Sets the Target of the Attacker
        int attackIndex = Random.Range(0, enemy.attackList.Count); //Randomly perform an attack from the available attacks the enemy has available
        myAttack.chosenAttack = enemy.attackList[attackIndex]; //Set the random attack
        Debug.Log(this.gameObject + " has chosen " + myAttack.chosenAttack.attackName + " and inflicts " + myAttack.chosenAttack.attackDamage + " damage");

        bsm.CollectActions(myAttack); //Send the Attacker Information to the BattleStateMachine
    }

    private IEnumerator TimeForBattle()
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
        //TODO: Wait A Bit
        yield return new WaitForSeconds(0.5f); //Wait for Animtaion to complete
        PerformDamage(); //Performs Damage to the Player Character
        Vector2 firstPosition = startPosition; //Sets local variable for the original position of the Enemy
        while (MoveTowardStart(firstPosition)) //Animate Enemy back to original battle position
        {
            yield return null;
        }

        //TODO: Remove from Performer List
        bsm.performList.RemoveAt(0); //Removes the Enemy from the Performer List
        //TODO: Reset bsm -> Wait
        bsm.battleState = BattleStateMachine.PerformAction.WAIT; //Sets the next performer b to the Wait State
        actionStarted = false; //Sets the actionStarted Boolean back to false
        curCooldown = 0f; //Resets wait bar value to 0
        currentState = TurnState.PROCESSING; //

    }

    private bool MoveTowardEnemy(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
    private bool MoveTowardStart(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    void PerformDamage() //Performs Damage to the Player Character
    {
        float damageValue = enemy.currentStrength *(100/(100 + targetToAttack.GetComponent<PCStateMachine>().playerCharacter.currentDefense)); //Formula for calculating basic damage
        Debug.Log(damageValue);
        targetToAttack.GetComponent<PCStateMachine>().TakeDamage(damageValue); //Applies the Damage
    }
}
