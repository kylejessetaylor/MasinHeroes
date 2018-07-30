using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Unit : MonoBehaviour {

    [Header("Character Statistics")]
    public CharacterStats character;

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
    protected int baseAttackDamage;
    protected int attackDamage;
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

    void Start () {
        //Current HP/Mana
        currentHealth = maxHealth;
        currentMana = maxMana;

        UpdateCharacterDisplay();
    }
	
	void Update () {

        //Resource Regen of Health & Mana
        HealthManaRegen();

	}

    //---------------------------------------------------------------------------------------------------------------------//


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
    //Regenerates health & mana constantly when possible
    protected void HealthManaRegen()
    {
        //Health
        if (!fullHealth)
        {
            currentHealth += 0.0001f * maxHealth * Time.deltaTime;
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
