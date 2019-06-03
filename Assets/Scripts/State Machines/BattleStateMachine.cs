using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleStateMachine : MonoBehaviour
{
    public enum PerformAction
    {
        WAIT,
        TAKEACTION,
        PERFORMACTION,
        CHECKIFALIVE,
        WIN,
        LOSE,
        FLED
    }
    public PerformAction battleState;

    public List<TurnHandler> performList = new List<TurnHandler>();
    public List<GameObject> playersInBattle = new List<GameObject>();
    public List<GameObject> enemiesInBattle = new List<GameObject>();

    public enum PlayerGUI
    {
        ACTIVATE,
        WAITING,
        DONE
    }
    public PlayerGUI playerInput;

    public List<GameObject> heroManageList = new List<GameObject>();
    private TurnHandler heroChoice;
    public GameObject enemyButton;
    public Transform spacer;

    public GameObject actionPanel;
    public Transform actionSpacer;
    public GameObject enemySelectPanel;
    public GameObject skillsPanel;
    public Transform skillsSpacer;
    public GameObject formsPanel;
    public Transform formsSpacer;
    public GameObject actionButton;
    public GameObject skillButton;
    public GameObject formButton;
    public GameObject fledText;
    public GameObject itemButton;
    public GameObject itemsPanel;
    public Transform itemsSpacer;
    public GameObject winText;
    public GameObject loseText;
    private List<GameObject> attackButtons = new List<GameObject>();
    private List<GameObject> enemyButtons = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        battleState = PerformAction.WAIT;
        enemiesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        playersInBattle.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        playerInput = PlayerGUI.ACTIVATE;
        actionPanel.SetActive(false);
        skillsPanel.SetActive(false);
        formsPanel.SetActive(false);
        enemySelectPanel.SetActive(false);
        itemsPanel.SetActive(false);
        fledText.SetActive(false);
        winText.SetActive(false);
        loseText.SetActive(false);
        EnemyButtons();
    }

    // Update is called once per frame
    void Update()
    {
        switch (battleState)
        {
            case (PerformAction.WAIT): //A waiting state that ends if the perform list is greater than 0
                if(performList.Count > 0)
                {
                    battleState = PerformAction.TAKEACTION;
                }
                break;
            case (PerformAction.TAKEACTION):
                GameObject performer = GameObject.Find(performList[0].attacker);
                if(performList[0].type == "Enemy")
                {
                    EnemyStateMachine esm = performer.GetComponent<EnemyStateMachine>();
                    for(int i = 0; i<playersInBattle.Count; i++)
                    {
                        if(performList[0].attackersTarget == playersInBattle[i])
                        {
                            esm.targetToAttack = performList[0].attackersTarget;
                            esm.currentState = EnemyStateMachine.TurnState.ACTION;
                            break;
                        }
                        else
                        {
                            performList[0].attackersTarget = playersInBattle[Random.Range(0, playersInBattle.Count)];
                            esm.targetToAttack = performList[0].attackersTarget;
                            esm.currentState = EnemyStateMachine.TurnState.ACTION;
                        }
                    }
                    esm.targetToAttack = performList[0].attackersTarget;
                    esm.currentState = EnemyStateMachine.TurnState.ACTION;
                }
                if(performList[0].type == "Player")
                {
                    Debug.Log("Hero ready to perform");
                    PCStateMachine psm = performer.GetComponent<PCStateMachine>();
                    psm.targetToAttack = performList[0].attackersTarget;
                    psm.currentState = PCStateMachine.TurnState.ACTION;
                }
                battleState = PerformAction.PERFORMACTION;
                break;
            case (PerformAction.PERFORMACTION):
                break;
            case (PerformAction.CHECKIFALIVE): //Check if a character is still alive
                if(playersInBattle.Count < 1)
                {
                    battleState = PerformAction.LOSE;
                    //Lose Battle
                }
                else if(enemiesInBattle.Count < 1)
                {
                    battleState = PerformAction.WIN;
                    //Win Battle
                }
                else
                {
                    ResetActionPanel();
                    playerInput = PlayerGUI.ACTIVATE;
                }
                break;

            case (PerformAction.WIN): //When the battle concludes and the player wins
                Debug.Log("You won");
                for(int i = 0; i<playersInBattle.Count; i++)
                {
                    playersInBattle[i].GetComponent<PCStateMachine>().currentState = PCStateMachine.TurnState.WAITING;
                }
                winText.SetActive(true);
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    SceneManager.LoadScene("Town");
                }
                break;

            case (PerformAction.LOSE): //When the battle concludes and the player loses
                Debug.Log("You Lost");
                for (int i = 0; i < enemiesInBattle.Count; i++)
                {
                    enemiesInBattle[i].GetComponent<EnemyStateMachine>().currentState = EnemyStateMachine.TurnState.WAITING;
                }
                loseText.SetActive(true);
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    SceneManager.LoadScene("Town");
                }
                break;

            case (PerformAction.FLED):
                for (int i = 0; i < playersInBattle.Count; i++)
                {
                    playersInBattle[i].GetComponent<PCStateMachine>().currentState = PCStateMachine.TurnState.WAITING;
                }
                for (int i = 0; i < enemiesInBattle.Count; i++)
                {
                    enemiesInBattle[i].GetComponent<EnemyStateMachine>().currentState = EnemyStateMachine.TurnState.WAITING;
                }
                fledText.SetActive(true);
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    SceneManager.LoadScene("Town");
                }
                break;
        }

        switch (playerInput)
        {
            case (PlayerGUI.ACTIVATE): //The state when a character is allowed to take their turn
                if(heroManageList.Count > 0)
                {
                    heroManageList[0].transform.Find("TurnPointer").gameObject.SetActive(true);
                    heroChoice = new TurnHandler();
                    actionPanel.SetActive(true);
                    CreateActionButtons();
                    playerInput = PlayerGUI.WAITING;
                }
                break;

            case (PlayerGUI.WAITING): //A Waiting state
                break;

            case (PlayerGUI.DONE): //State to show the character is done
                PlayerInputComplete();
                break;
        }
    }

    public void CollectActions(TurnHandler turnInfo) //Sends the information of that turn to the Perform List
    {
        performList.Add(turnInfo);
    }

    public void EnemyButtons() //Cleans and populates the Buttons for the Enemies to be selected
    {
        /*
         Cleans up the list for the enemy buttons
         Allows for when an enemy is killed in a multi enemy fight, the button
         corresponding to that enemy is removed and the list refreshed
         */
        foreach(GameObject enemyBtn in enemyButtons)
        {
            Destroy(enemyBtn);
        }
        enemyButtons.Clear();
        /*
         Populates the List of enemy buttons so the player can select an enemy to attack.
         */
        foreach(GameObject enemy in enemiesInBattle)
        {
            GameObject newButton = Instantiate(enemyButton) as GameObject;
            EnemySelectButton button = newButton.GetComponent<EnemySelectButton>();
            EnemyStateMachine cur_enemy = enemy.GetComponent<EnemyStateMachine>();
            Text buttonText = newButton.transform.Find("Text").gameObject.GetComponent<Text>();
            buttonText.text = " " + cur_enemy.enemy.name;
            button.enemyPrefab = enemy;
            newButton.transform.SetParent(spacer);
            enemyButtons.Add(newButton);
        }
    }

    public void Attack() //Attack
    {
        heroChoice.attacker = heroManageList[0].name;
        heroChoice.attackerGameObject = heroManageList[0];
        heroChoice.type = "Player";
        heroChoice.chosenAttack = heroManageList[0].GetComponent<PCStateMachine>().playerCharacter.attackList[0];
        actionPanel.SetActive(false);
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
        ResetActionPanel();
        
        heroManageList[0].transform.Find("TurnPointer").gameObject.SetActive(false);
        heroManageList.RemoveAt(0);
        playerInput = PlayerGUI.ACTIVATE;
    }

    void ResetActionPanel()
    {
        enemySelectPanel.SetActive(false);
        actionPanel.SetActive(false);
        skillsPanel.SetActive(false);
        formsPanel.SetActive(false);
        foreach (GameObject atkBtn in attackButtons)
        {
            Destroy(atkBtn);
        }
        attackButtons.Clear();

    }

    void CreateActionButtons()
    {
        GameObject attackButton = Instantiate(actionButton) as GameObject;
        Text attackButtonText = attackButton.transform.Find("Text").gameObject.GetComponent<Text>();
        attackButtonText.text = "Attack";
        attackButton.GetComponent<Button>().onClick.AddListener(()=>Attack());
        attackButton.transform.SetParent(actionSpacer, false);
        attackButtons.Add(attackButton);

        GameObject skillsButton = Instantiate(actionButton) as GameObject;
        Text skillsButtonText = skillsButton.transform.Find("Text").gameObject.GetComponent<Text>();
        skillsButtonText.text = "Skills";
        skillsButton.GetComponent<Button>().onClick.AddListener(() => selectSkill());
        skillsButton.transform.SetParent(actionSpacer, false);
        attackButtons.Add(skillsButton);

        GameObject formsButton = Instantiate(actionButton) as GameObject;
        Text formsButtonText = formsButton.transform.Find("Text").gameObject.GetComponent<Text>();
        formsButtonText.text = "Forms";
        formsButton.GetComponent<Button>().onClick.AddListener(() => SelectForm());
        formsButton.transform.SetParent(actionSpacer, false);
        attackButtons.Add(formsButton);

        GameObject itemsButton = Instantiate(actionButton) as GameObject;
        Text itemsButtonText = itemsButton.transform.Find("Text").gameObject.GetComponent<Text>();
        itemsButtonText.text = "Items";
        itemsButton.GetComponent<Button>().onClick.AddListener(() => SelectItem());
        itemsButton.transform.SetParent(actionSpacer, false);
        attackButtons.Add(itemsButton);

        GameObject fleeButton = Instantiate(actionButton) as GameObject;
        Text fleeButtonText = fleeButton.transform.Find("Text").gameObject.GetComponent<Text>();
        fleeButtonText.text = "Flee";
        fleeButton.GetComponent<Button>().onClick.AddListener(() => Flee());
        fleeButton.transform.SetParent(actionSpacer, false);
        attackButtons.Add(fleeButton);

        GameObject loseButton = Instantiate(actionButton) as GameObject;
        Text loseButtonText = loseButton.transform.Find("Text").gameObject.GetComponent<Text>();
        loseButtonText.text = "Lose";
        loseButton.GetComponent<Button>().onClick.AddListener(() => Lose());
        loseButton.transform.SetParent(actionSpacer, false);
        attackButtons.Add(loseButton);

        if (heroManageList[0].GetComponent<PCStateMachine>().playerCharacter.skillList.Count > 0)
        {
            foreach (Attack skill in heroManageList[0].GetComponent<PCStateMachine>().playerCharacter.skillList)
            {
                if (skill.learned)
                {
                    GameObject addSkillButton = Instantiate(skillButton) as GameObject;
                    Text skillButtonText = addSkillButton.transform.Find("Text").gameObject.GetComponent<Text>();
                    skillButtonText.text = skill.attackName;
                    SkillAttack sB = skillButton.GetComponent<SkillAttack>();
                    sB.skillToPerform = skill;
                    addSkillButton.transform.SetParent(skillsSpacer, false);
                    attackButtons.Add(addSkillButton);
                }
            }
        }
        else
        {
            skillsButton.GetComponent<Button>().interactable = false;
        }

        if (heroManageList[0].GetComponent<PCStateMachine>().playerCharacter.formList.Count > 0)
        {
            foreach (Form form in heroManageList[0].GetComponent<PCStateMachine>().playerCharacter.formList)
            {
                GameObject addFormButton = Instantiate(formButton) as GameObject;
                Text formButtonText = addFormButton.transform.Find("Text").gameObject.GetComponent<Text>();
                formButtonText.text = form.formName;
                FormAttack fB = formButton.GetComponent<FormAttack>();
                fB.formToEnter = form;
                addFormButton.transform.SetParent(formsSpacer, false);
                attackButtons.Add(addFormButton);
            }
        }
        else
        {
            formsButton.GetComponent<Button>().interactable = false;
        }

        if(heroManageList[0].GetComponent<PCStateMachine>().playerCharacter.itemList.Count > 0)
        {
            foreach(Item item in heroManageList[0].GetComponent<PCStateMachine>().playerCharacter.itemList)
            {
                if(item.itemCount > 0)
                {
                    GameObject addItemButton = Instantiate(itemButton) as GameObject;
                    Text itemButtonText = addItemButton.transform.Find("Text").gameObject.GetComponent<Text>();
                    itemButtonText.text = item.itemName;
                    ItemAttack iB = itemButton.GetComponent<ItemAttack>();
                    iB.itemToUse = item;
                    addItemButton.transform.SetParent(itemsSpacer, false);
                    attackButtons.Add(addItemButton);
                }
            }
            
        }
        else
        {
            itemsButton.GetComponent<Button>().interactable = false;
        }

    }

    public void selectSkill()
    {
        actionPanel.SetActive(false);
        skillsPanel.SetActive(true);
    }

    public void Skill(Attack chosenSkill)
    {
        heroChoice.attacker = heroManageList[0].name;
        heroChoice.attackerGameObject = heroManageList[0];
        heroChoice.type = "Player";
        heroChoice.chosenAttack = chosenSkill;
        skillsPanel.SetActive(false);
        enemySelectPanel.SetActive(true);
    }

    public void SelectForm()
    {
        actionPanel.SetActive(false);
        formsPanel.SetActive(true);
    }

    public void Form(Form chosenForm)
    {
        heroChoice.attacker = heroManageList[0].name;
        heroChoice.attackerGameObject = heroManageList[0];
        heroChoice.type = "Player";
        heroChoice.chosenForm = chosenForm;
        ChangePlayerStatsForm(chosenForm);
        formsPanel.SetActive(false);
        enemySelectPanel.SetActive(true);
    }

    public void ChangePlayerStatsForm(Form chosenForm)
    {
        float strength = heroManageList[0].GetComponent<PCStateMachine>().playerCharacter.currentStrength;
        float defense = heroManageList[0].GetComponent<PCStateMachine>().playerCharacter.currentDefense;
        float speed = heroManageList[0].GetComponent<PCStateMachine>().playerCharacter.currentSpeed;
        float magic = heroManageList[0].GetComponent<PCStateMachine>().playerCharacter.currentMagic;
        float evasion = heroManageList[0].GetComponent<PCStateMachine>().playerCharacter.currentEvasion;

        if (chosenForm.formName == "Origin Form")
        {
            heroManageList[0].GetComponent<PCStateMachine>().playerCharacter.currentStrength = strength * 3;
            heroManageList[0].GetComponent<PCStateMachine>().playerCharacter.currentDefense = defense * 3;
            heroManageList[0].GetComponent<PCStateMachine>().playerCharacter.currentSpeed = speed * 3;
            heroManageList[0].GetComponent<PCStateMachine>().playerCharacter.currentMagic = magic * 3;
            heroManageList[0].GetComponent<PCStateMachine>().playerCharacter.currentEvasion = evasion * 3;
        }
        else if(chosenForm.formName == "Released Form")
        {
            heroManageList[0].GetComponent<PCStateMachine>().playerCharacter.currentStrength = strength * 2;    
            heroManageList[0].GetComponent<PCStateMachine>().playerCharacter.currentDefense = defense * 2;
            heroManageList[0].GetComponent<PCStateMachine>().playerCharacter.currentSpeed = speed * 2;
            heroManageList[0].GetComponent<PCStateMachine>().playerCharacter.currentMagic = magic * 2;
            heroManageList[0].GetComponent<PCStateMachine>().playerCharacter.currentEvasion = evasion * 2;
        }
    }

    public void SelectItem()
    {
        actionPanel.SetActive(false);
        itemsPanel.SetActive(true);
    }

    public void Item(Item chosenItem)
    {
        heroChoice.attacker = heroManageList[0].name;
        heroChoice.attackerGameObject = heroManageList[0];
        heroChoice.type = "Player";
        heroChoice.chosenItem = chosenItem;
        heroChoice.typeOfAttack = "Item";
        itemsPanel.SetActive(false);
        enemySelectPanel.SetActive(true);
    }

    public TurnHandler GetHeroChoice()
    {
        return heroChoice;
    }

    public void Flee()
    {
        battleState = PerformAction.FLED;
    }

    public void Lose()
    {
        battleState = PerformAction.LOSE;
    }
}
