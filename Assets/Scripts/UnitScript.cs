using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.AI;

public class UnitScript : MonoBehaviour {

    [Header("Character Statistics")]
    public CharacterClass character;

    ///Type of unit
    public Unit unitType = Unit.CONTROLLABLE;
    public enum Unit { CONTROLLABLE, FRIENDLY, NEUTRAL, HOSTILE }

    //Spells
    [Tooltip("Place in order: 'Passive, Active1, Active2, Active3... Active5'")]
    public List<Ability> abilities = new List<Ability>();

    //Combat
    public Combat combatState = Combat.IDLE;
    public enum Combat { IDLE, LOOKAT, CHASE, ATTACK, CHANNELCAST }

    //NavMesh Movement
    [HideInInspector]
    public Transform moveLocation;
    protected NavMeshAgent agent;

    //Character
    protected string characterName;
    protected Sprite characterIcon;
    protected int currentLevel = 1;

    ///Resources
    //Health
    protected int baseHealth;
    protected float maxHealth;
    public float currentHealth = 1;
    //Health gained from items & spells
    [HideInInspector]
    public float additionalHealth;

    //Mana
    protected int baseMana;
    protected float maxMana;
    public float currentMana = 1;
    //Mana gained from items & spells
    [HideInInspector]
    public float additionalMana;

    ///Offensive
    //Checks for auto-aggro
    public float detectionRange;
    protected int baseAttackDamage;
    protected int attackDamage;
    protected float attackSpeedMult;
    //Damage gained from items & spells
    [HideInInspector]
    public int additionalAttackDamage;
    [HideInInspector]
    public int totalAttackDamage;

    protected bool melee;
    protected int range;

    ///Defensive
    protected int baseArmor;
    protected float armor;
    //Armor gained from items & spells
    [HideInInspector]
    public int additionalArmor;

    //Percentage(%) based defense
    [HideInInspector]
    protected float physicalDefense;
    //Physical defense gained from items & spells
    [HideInInspector]
    public float additionalPhysicalDefense;
    [HideInInspector]
    protected float magicalDefense;
    //Physical defense gained from items & spells
    [HideInInspector]
    public float additionalMagicallDefense;

    //Animations
    [HideInInspector]
    public Animator anim;
    protected bool takeDamageAnim;
    protected bool attackAnim;


    //---------------------------------------------------------------------------------------------------------------------//

    //Apply ScriptableObject 'CharacterStats' stats
    protected void ApplyScriptableObjectStats()
    {
        //Character
        characterName = character.characterName;
        characterIcon = character.characterIcon;

        //Unit Type
        unitType = (Unit)character.unitType;

        //Resources
        baseHealth = character.baseHealth;
        baseMana = character.baseMana;

        //Offensive
        baseAttackDamage = character.baseAttackDamage;
        melee = character.melee;
        range = character.range;

        //Defensive
        baseArmor = character.baseArmor;
}

    //---------------------------------------------------------------------------------------------------------------------//   

    protected void Awake()
    {
        //Apply ScriptableObject 'CharacterStats' stats
        ApplyScriptableObjectStats();

        //Places Icons & Text onto UI
        FirstCharacterDisplay();
        //Calculates all stats for this character
        UpdateAllStats();

    }

    void Start()
    {
        //Assigns this unit to the UnitManager class
        AssignToUnitManager(true);

        //Current HP/Mana
        currentHealth = maxHealth;
        currentMana = maxMana;
        //Sets combat state
        combatState = Combat.IDLE;

        //Assigns Animatior
        StartVariableAssignments();

        //Updates UI current health/mana
        UpdateHealthManaTexT();
    }

    void Update()
    {
        //Starts Auto-Attack when hostiles are near player or vise versa
        AutoAttack();

        //Checks for Combat States
        CombatStateChecks();

    }

    protected void FixedUpdate()
    {
        //Resource Regen of Health & Mana
        HealthManaRegen();

        //Turns off animations to keep from looping
        TurnOffAnimation();
    }

    //---------------------------------------------------------------------------------------------------------------------//

    //IDLE, LOOKAT, CHASE, ATTACK, CHANNELCAST
    protected void CombatStateChecks()
    {
        //Checks for CHANNELCAST state
        if (combatState == Combat.CHANNELCAST)
        {
            //Start CHANNEL
        }
        //Checks for ATTACK state
        else if (combatState == Combat.ATTACK)
        {
            //Start ATTACK
            AttackSpeedCheck();
        }
        //Checks for CHASE state
        else if (combatState == Combat.CHASE)
        {
            //Start CHASE
            Chase();
        }
        //Checks for LOOKAT state
        else if (combatState == Combat.LOOKAT)
        {
            //Start LOOKAT
            RotateTowards(attackTarget.transform);
        }
        //Checks for IDLE state
        else if (combatState == Combat.IDLE)
        {
            //Start IDLE
        }
    }

    //---------------------------------------------------------------------------------------------------------------------//


    //Call when swapping in/out items & on Awake
    ///TODO
    ///AND when items in inventory change
    protected void UpdateAllStats()
    {
        //Calculates Primary Stats, Health & Mana
        CalculateAll();

        //Updates UI with new calculations
        UpdateCharacterDisplay();

    }


    #region StatCalculations

    //Calculates Primary Stats, Health & Mana, Attack & Armor
    protected void CalculateAll()
    {
        //Calculates health & mana
        CalcResources();
        //Calculates attack & armor
        CalcAttackNArmor();

        //Calculates physical & magical defense
        CalcDefense();
    }

    //Calculates health & mana at start of the game
    protected void CalcResources()
    {
        //Health
        maxHealth = baseHealth + additionalHealth;

        //Mana
        maxMana = baseMana + additionalMana;
    }

    //Calculates attack & armor at start of the game
    protected void CalcAttackNArmor()
    {
        //AutoAttack
        #region Attack
        //Applies damage bonuses
        totalAttackDamage = baseAttackDamage + additionalAttackDamage;
        #endregion

        //PhysicalArmor
        #region Armor
        //Calculates Armor
        armor = baseArmor + additionalArmor;
        #endregion
    }

    //Calculates physical & magical defense at start of the game
    protected void CalcDefense()
    {
        #region Physical Def %

        //Equation that returns % dmg reduction for physical attacks in decimal form
        float armorToPercent = (-SystemsManager.rateOfArmor / (armor + SystemsManager.rateOfArmor)) + 1;
        //Calculates additional defense from items/spells after armor calculation
        physicalDefense = armorToPercent + (1-armorToPercent)*additionalPhysicalDefense;

        //Note: ^^ prevents ever reaching 100% defense. 

        #endregion

        #region Magical Def %

        magicalDefense = additionalMagicallDefense;

        #endregion
    }

    #endregion

    #region UIDisplayStats

    [Header("UI Naviation")]
    public TextMeshProUGUI characterNameText;
    public Image characterIconImage;


    //Places all numbers & icons onto UI
    private void FirstCharacterDisplay()
    {
        //characterName
        characterNameText.text = characterName.ToString();
        //characterIcon
        characterIconImage.sprite = characterIcon;
    }

    public TextMeshProUGUI healthText;
    public TextMeshProUGUI manaText;

    public TextMeshProUGUI attackText;
    public TextMeshProUGUI additionalAttackText;
    public TextMeshProUGUI armorText;
    public TextMeshProUGUI additionalArmorText;

    //Updates all numbers onto UI
    protected void UpdateCharacterDisplay()
    {
        //Checks if this unit is current selected target
        if (GameManager.selectedTarget != gameObject)
        {
            return;
        }
        else
        {
            //Turn on all unit-based UI
        }

        //currentHealth/maxHealth
        //currentMana/maxMana
        UpdateHealthManaTexT();

        ///attackDamage + (green)additionalAttackDamage+statDamageBonus
        attackText.text = attackDamage.ToString();
        if (additionalAttackDamage > 0)
        {
            additionalAttackText.text = " + " + additionalAttackDamage.ToString();
        }
        else
        {
            //Hides + 0
            additionalAttackText.text = "";
        }

        ///armor + (green)additionalArmor
        armorText.text = armor.ToString();
        //Hides green if + 0
        if (additionalAttackDamage > 0)
        {
            additionalArmorText.text = " + " + additionalArmor.ToString();
        }
        else
        {
            //Hides + 0
            additionalArmorText.text = "";
        }
    }

    #endregion

    //---------------------------------------------------------------------------------------------------------------------//

    /// <summary>
    /// Assigns this unit to the UnitManager class
    /// </summary>
    /// <param name="add">Set to 'True' if you want to add the unit to the UnitManager class.'False' if you want to remove it.</param>
    protected void AssignToUnitManager(bool add)
    {
        //Adds this unit to UnitManager class
        if (add)
        {
            //Assigns hostile unit
            if (unitType == Unit.HOSTILE)
            {
                UnitManager.hostileUnits.Add(gameObject);
            }
            //Assigns neutral unit
            else if (unitType == Unit.NEUTRAL)
            {
                UnitManager.neutralUnits.Add(gameObject);
            }
            //Assigns friendly unit
            else if (unitType == Unit.FRIENDLY)
            {
                UnitManager.friendlyUnits.Add(gameObject);
                UnitManager.alliedUnits.Add(gameObject);
            }
            //Assigns controllable unit
            else if (unitType == Unit.CONTROLLABLE)
            {
                UnitManager.controllableUnits.Add(gameObject);
                UnitManager.alliedUnits.Add(gameObject);
            }
            //Debug.Log("Added " + gameObject + " to " + unitType + " list.");
        }
        //Removes this unit from UnitManager class
        else
        {
            //Removes hostile unit
            if (unitType == Unit.HOSTILE)
            {
                UnitManager.hostileUnits.Remove(gameObject);
            }
            //Removes neutral unit
            else if (unitType == Unit.NEUTRAL)
            {
                UnitManager.neutralUnits.Remove(gameObject);
            }
            //Removes friendly unit
            else if (unitType == Unit.FRIENDLY)
            {
                UnitManager.friendlyUnits.Remove(gameObject);
                UnitManager.alliedUnits.Add(gameObject);
            }
            //Removes controllable unit
            else if (unitType == Unit.CONTROLLABLE)
            {
                UnitManager.controllableUnits.Remove(gameObject);
                UnitManager.alliedUnits.Add(gameObject);
            }
            //Debug.Log("Removed " + gameObject + " from " + unitType + " list.");
        }
    }

    //Assigns Animations to script variables
    protected void StartVariableAssignments()
    {
        //Animation
        anim = gameObject.GetComponent<Animator>();
        //NavMesh Agent
        agent = GetComponent<NavMeshAgent>();
        //Rigid Body
        rb = gameObject.GetComponent<Rigidbody>();

    }

    //Turns off Animation bools every FixedUpdate
    protected void TurnOffAnimation()
    {
        //Take Damage Animation
        anim.SetBool("TakeDamage_Anim", false);
        //Attack Animation
        anim.SetBool("Attack_Anim", false);

    }

    //---------------------------------------------------------------------------------------------------------------------//
    ///Regeneration

    protected void UpdateHealthManaTexT()
    {
        //Doesn't let hp/mana view below 0
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        if (currentMana < 0)
        {
            currentMana = 0;
        }
        //Stops Decimals showing on HP & MP
        int hp = (int)currentHealth;
        int mp = (int)currentMana;      

        //currentHealth/maxHealth
        healthText.text = hp.ToString() + "/" + maxHealth.ToString();
        //currentMana/maxMana
        manaText.text = mp.ToString() + "/" + maxMana.ToString();
    }

    //Resource Regen
    protected bool fullHealth = true;
    protected bool fullMana = true;
    private float healthRegenPercent = 0.00001f;
    //Regenerates health & mana constantly when possible
    protected void HealthManaRegen()
    {
        //Health
        if (!fullHealth)
        {
            currentHealth += healthRegenPercent * maxHealth * Time.deltaTime;
            //Cancels on full HP
            if (currentHealth >= maxHealth)
            {
                currentHealth = maxHealth;
                fullHealth = true;
            }
            //Displays UI
            UpdateHealthManaTexT();
        }
        //Mana
        if (!fullMana)
        {
            currentMana += (0.0001f * maxMana) * Time.deltaTime;
            //Cancels on full HP
            if (currentMana >= maxMana)
            {
                currentMana = maxMana;
                fullMana = true;
            }
            //Displays UI
            UpdateHealthManaTexT();
        }
    }

    //---------------------------------------------------------------------------------------------------------------------//
    ///Combat

    //Stores whether unit has moved & needs to check radius
    //protected Vector3 currentTrans;
    //Target enemy saved as auto-attack target
    [HideInInspector]
    public GameObject attackTarget;
    //Signifies unit is trying to auto attack
    protected bool autoAttacking;

    [HideInInspector]
    public Rigidbody rb;
    //Starts Auto-Attack when hostiles are near player or vise versa
    protected void AutoAttack()
    {
        //If unit has moved
        if (rb.velocity.magnitude > 0)
        {
            CheckForEnemies();
            Debug.Log(gameObject + " moved!");
        }
        CheckForEnemies();
    }

    //Checks if an enemy is within range
    ///TODO
    ///MAKE SURE TO DO THIS CHECK AFTER TARGET ENEMY DIES
    protected void CheckForEnemies()
    {
        //If I'm a friendly
        if (unitType == Unit.CONTROLLABLE || unitType == Unit.FRIENDLY)
        {
            //Checks for hostiles
            DetectionCheck(UnitManager.hostileUnits);
        }
        //If I'm a hostile
        if (unitType == Unit.HOSTILE)
        {
            //Checks for controllable & friendlies
            DetectionCheck(UnitManager.alliedUnits);
        }

    }

    //Checks the list of units to see if any of them are within detection range
    protected void DetectionCheck(List<GameObject> unitListToCheck)
    {
        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;

        //Check for hostiles
        for (int i = 0; i < unitListToCheck.Count; i++)
        {
            GameObject target = unitListToCheck[i];
            float targetDistance = Vector3.Distance(transform.position,
                target.transform.position);
            //If a unit is within detection range
            if (targetDistance <= detectionRange)
            {
                //If it's closer than current closest
                if (targetDistance < closestDistance)
                {
                    //Assigns unit as new closest target
                    closestDistance = targetDistance;
                    closestTarget = target;
                }
                //I am attacking
                autoAttacking = true;
            }
        }
        //Stops auto attacking if no enemy unit is near
        if (closestTarget == null)
        {
            autoAttacking = false;
        }
        //Targets closest enemy as the new attack
        else
        {
            attackTarget = closestTarget;
            //Puts unit into attacking state
            combatState = Combat.ATTACK;
        }

    }

    //Used for AttackSpeed
    protected float timeSinceLastAttack;
    //Performs attack based on Attack Speed
    protected void AttackSpeedCheck()
    {
        //Faces target
        Vector3 relativePos = attackTarget.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        transform.rotation = rotation * Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z);
        
        //Checks for attack speed CD
        float lastAttackTime = Time.time - timeSinceLastAttack;
        if (SystemsManager.attackSpeed / (1+attackSpeedMult) <= lastAttackTime
            && SystemsManager.attackSpeed <= lastAttackTime)
        {
            Attack();
        }
    }

    //Checks Range for & Attacks
    protected void Attack()
    {
        //Distance from attack target to player
        float distanceToTarget = Vector3.Distance(transform.position, attackTarget.transform.position);
        //Checks if close enough to attack
        if (distanceToTarget <= range)
        {
            //Makes sure combat state is ATTACK
            combatState = Combat.ATTACK;

            //Plays attack animation
            anim.SetBool("Attack_Anim", true);

            //Checks for UnitScript.cs & deals dmg
            if (attackTarget.GetComponent<UnitScript>() != null)
            {
                //Deals auto attack dmg as physical
                attackTarget.GetComponent<UnitScript>().DamageToTake(gameObject, 1, totalAttackDamage);
            }
            //Checks for Character.cs & deals dmg
            else
            {
                //Deals auto attack dmg as physical
                attackTarget.GetComponent<Character>().DamageToTake(gameObject, 1, totalAttackDamage);
            }

            timeSinceLastAttack = Time.time;

            RotateTowards(attackTarget.transform);
        }
        //Goes to chase instead
        else
        {
            combatState = Combat.CHASE;
            Chase();
        }
    }
    
    /// <summary>
    /// Causes the targeted unit to take damage.
    /// </summary>
    /// <param name="damageType">
    /// 1 = Physical | 
    /// 2 = Magical | 
    /// 3 = Pure/Raw damage</param>
    /// <param name="damage">The amount of damage the unit will take (unmitigated).</param>
    protected void DamageToTake(GameObject attacker, int damageType, float damage)
    {
        //Plays Animation
        anim.SetBool("TakeDamage_Anim", true);

        //Phyiscal Damage
        if (damageType == 1)
        {
            //Applies armor defense
            damage = damage * (1 - physicalDefense);
        }

        //Magical Damage
        else if (damageType == 2)
        {
            //Applies armor defense
            damage = damage * (1 - magicalDefense);
        }

        //Applies damage to take
        currentHealth -= damage;
        //IF THIS UNIT IS CURRENTLY SELECTED TARGET
        UpdateHealthManaTexT();

        //Checks for death
        if (currentHealth < 0)
        {
            //Kills unit
            Death();
            //Stops Regen & Cancels regen from occuring
            fullHealth = true;
            return;
        }

        //Turns unit hostile on first attack
        if (attackTarget == null)
        {
            //Assigns attacker to attack target
            attackTarget = attacker;
            //Enters Combat State
            combatState = Combat.ATTACK;
        }

        //Allows HP regen to happen
        fullHealth = false;
    }

    //Kills this unit
    protected void Death()
    {
        //Removes this unit from UnitManager class
        AssignToUnitManager(false);

        //Plays Death Animation
        //Plays Death Sound
    }

    //---------------------------------------------------------------------------------------------------------------------//
    ///Chase
    ///


    private void RotateTowards(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * 100f);
    }
    protected void Chase()
    {
        //Faces target if cant move
        if (agent.speed == 0)
        {
            //RotateTowards(attackTarget.transform);
            ///TODO
            ///FIX ANIMATION SO IT AFFECTS A CHILD SO I CAN ROTATE THIS OBJECT 
            Debug.Log("Looking at you");
        }
        //Chases target
        else
        {
            agent.destination = attackTarget.transform.position;
            Debug.Log("Chasing you");
        }

        //Checks for range & attacks if within range
        float distanceToTarget = Vector3.Distance(transform.position, attackTarget.transform.position);
        if (distanceToTarget <= range)
        {
            Attack();
        }
    }
}
