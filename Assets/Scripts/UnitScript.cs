using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UnitScript : MonoBehaviour {

    [Header("Character Statistics")]
    public CharacterClass character;

    ///Type of unit
    public Unit unitType = Unit.CONTROLLABLE;
    public enum Unit { CONTROLLABLE, FRIENDLY, NEUTRAL, HOSTILE }

    //Spells
    [Tooltip("Place in order: 'Passive, Active1, Active2, Active3... Active5'")]
    public List<Ability> abilities = new List<Ability>();

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
    protected float attackSpeed;
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
    
    protected void Awake()
    {
        //Apply ScriptableObject 'CharacterStats' stats
        ApplyScriptableObjectStats();

        //Places Icons & Text onto UI
        FirstCharacterDisplay();
        //Calculates all stats for this character
        UpdateAllStats();

    }


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
    protected void UnitToUnitManager(bool add)
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

    //---------------------------------------------------------------------------------------------------------------------//

    void Start () {
        //Assigns this unit to the UnitManager class
        UnitToUnitManager(true);

        //Current HP/Mana
        currentHealth = maxHealth;
        currentMana = maxMana;

        //Updates UI current health/mana
        UpdateHealthManaTexT();

        //Setup for AutoAttack functions
        AutoAttackStart();
    }
	
	void Update () {

        //Starts Auto-Attack when hostiles are near player or vise versa
        AutoAttack();

    }

    protected void FixedUpdate()
    {
        //Resource Regen of Health & Mana
        HealthManaRegen();
    }

    //---------------------------------------------------------------------------------------------------------------------//
    ///Regeneration

    protected void UpdateHealthManaTexT()
    {
        //currentHealth/maxHealth
        healthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
        //currentMana/maxMana
        manaText.text = currentMana.ToString() + "/" + maxMana.ToString();
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
    protected Vector3 currentTrans;
    //Target enemy saved as auto-attack target
    protected GameObject attackTarget;
    //Signifies unit is trying to auto attack
    protected bool autoAttacking;
    
    protected void AutoAttackStart()
    {
        currentTrans = transform.position;
    }

    //Starts Auto-Attack when hostiles are near player or vise versa
    protected void AutoAttack()
    {
        //If unit has moved
        if (currentTrans != transform.position)
        {
            CheckForEnemies();
        }
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
            DistanceCheck(UnitManager.hostileUnits);
        }
        //If I'm a hostile
        if (unitType == Unit.HOSTILE)
        {
            //Checks for controllable & friendlies
            DistanceCheck(UnitManager.alliedUnits);
        }

    }

    //Checks the list of units to see if any of them are within detection range
    protected void DistanceCheck(List<GameObject> unitListToCheck)
    {
        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;

        //Check for hostiles
        for (int i = 0; i < unitListToCheck.Count - 1; i++)
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
    protected void DamageToTake(int damageType, float damage)
    {
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


        //Allows HP regen to happen
        fullHealth = false;
    }
}
