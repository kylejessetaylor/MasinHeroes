using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BackupCharacter : MonoBehaviour {

    [Header("Character Statistics")]
    public CharacterStats character;

    public List<Ability> abilities = new List<Ability>();

    //Character
    private string characterName;
    private Sprite characterIcon;

    private int currentLevel = 1;
    private float currentXP;
    private float maxXP;

    ///Resources
    //Health
    private int baseHealth;
    private float maxHealth;
    public float currentHealth = 1;
    //Health gained from items & spells
    [HideInInspector]
    public float additionalHealth;

    //Mana
    private int baseMana;
    private float maxMana;
    public float currentMana = 1;
    //Mana gained from items & spells
    [HideInInspector]
    public float additionalMana;

    ///Stats
    [Tooltip("'Strength', 'Agility', 'Intelligence'. Type only one of these three. This determines what style of gear/stats the player should aim for.")]
    private string heroType = "Strength";

    ///Offensive
    private int baseAttackDamage;
    private float attackPerLevel;
    private int attackDamage;
    //Damage gained from items & spells
    [HideInInspector]
    public int additionalAttackDamage;
    [HideInInspector]
    public int totalAttackDamage;

    private bool melee;
    private int range;

    ///Defensive
    private int baseArmor;
    private float armor;
    //Armor gained from items & spells
    [HideInInspector]
    public int additionalArmor;

    //Percentage(%) based defense
    [HideInInspector]
    private float physicalDefense;
    //Physical defense gained from items & spells
    [HideInInspector]
    public float additionalPhysicalDefense;
    [HideInInspector]
    private float magicalDefense;
    //Physical defense gained from items & spells
    [HideInInspector]
    public float additionalMagicallDefense;

    ///Raw Stats
    private int baseStrength;
    private float strPerLevel;
    private float strength;
    //Strength gained from items & spells
    [HideInInspector]
    public int additionalStrength;
    [HideInInspector]
    public int totalStrength;

    private int baseAgility;
    private float agiPerLevel;
    private float agility;
    //Agility gained from items & spells
    [HideInInspector]
    public int additionalAgility;
    [HideInInspector]
    public int totalAgility;

    private int baseIntelligence;
    private float intPerLevel;
    private float intelligence;
    //Intelligence gained from items & spells
    [HideInInspector]
    public int additionalIntelligence;
    [HideInInspector]
    public int totalIntelligence;

    //---------------------------------------------------------------------------------------------------------------------//

    //Apply ScriptableObject 'CharacterStats' stats
    void ApplyScriptableObjectStats()
    {
        //Character
        characterName = character.characterName;
        characterIcon = character.characterIcon;

        //Resources
        baseHealth = character.baseHealth;
        baseMana = character.baseMana;

        ///Stats
        heroType = character.heroType;

        //Offensive
        baseAttackDamage = character.baseAttackDamage;
        melee = character.melee;
        range = character.range;

        //Defensive
        baseArmor = character.baseArmor;

        //Raw Stats
        baseStrength = character.baseStrength;
        baseAgility = character.baseAgility;
        baseIntelligence = character.baseIntelligence;
        //Stats per level
        strPerLevel = character.strPerLevel;
        agiPerLevel = character.agiPerLevel;
        intPerLevel = character.intPerLevel;
}

    private void Awake()
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
    private void UpdateAllStats()
    {
        //Calculates Primary Stats, Health & Mana
        CalculateAll();

        //Updates UI with new calculations
        UpdateCharacterDisplay();
    }


    #region StatCalculations

    //Calculates Primary Stats, Health & Mana, Attack & Armor
    private void CalculateAll()
    {
        //Calculates primary stats
        CalcStats();
        //Calculates health & mana
        CalcResources();
        //Calculates attack & armor
        CalcAttackNArmor();

        //Calculates physical & magical defense
        CalcDefense();
    }

    //Calculates primary stats at start of the game
    private void CalcStats()
    {
        ///Stats based on level
        //Strength
        strength = baseStrength + ((currentLevel-1) * strPerLevel);
        //Agility
        agility = baseAgility + ((currentLevel - 1) * agiPerLevel);
        //Intelligence
        intelligence = baseIntelligence + ((currentLevel - 1) * intPerLevel);


        ///Total Stats used for spells & abilities
        //Strength
        totalStrength = (int)strength + additionalStrength;
        //Agility
        totalAgility = (int)agility + additionalAgility;
        //Intelligence
        totalIntelligence = (int)intelligence + additionalIntelligence;
    }

    //Calculates health & mana at start of the game
    private void CalcResources()
    {
        //Health
        maxHealth = baseHealth + (strength * SystemsManager.strToHealth) + additionalHealth;

        //Mana
        maxMana = baseMana + (intelligence * SystemsManager.intToMana) + additionalMana;
    }

    //Calculates attack & armor at start of the game
    private int statDamageBonus;
    private void CalcAttackNArmor()
    {
        //AutoAttack
        #region Attack
        //Strength
        if (heroType == "Strength")
        {
            statDamageBonus = (int)(strength * SystemsManager.statToAttack);
        }
        //Agility
        else if (heroType == "Agility")
        {
            statDamageBonus = (int)(agility * SystemsManager.statToAttack);
        }
        //Intelligence
        else if (heroType == "Intelligence")
        {
            statDamageBonus = (int)(intelligence * SystemsManager.statToAttack);
        }
        //Applies damage bonuses
        attackDamage = baseAttackDamage + (int)((currentLevel - 1) * attackPerLevel) + statDamageBonus;
        totalAttackDamage = attackDamage + additionalAttackDamage;
        #endregion


        //PhysicalArmor
        #region Armor
        //Calculates Armor
        armor = baseArmor + (int)(agility * SystemsManager.agiToArmor) + additionalArmor;
        #endregion
    }

    //Calculates physical & magical defense at start of the game
    private void CalcDefense()
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

    public Image spellOne;
    public Image spellTwo;
    public Image spellThree;
    public Image spellFour;
    public Image spellFive;

    public Image strengthIcon;
    public Image agilityIcon;
    public Image intelligenceIcon;


    //Places all numbers & icons onto UI
    private void FirstCharacterDisplay()
    {
        //characterName
        characterNameText.text = characterName.ToString();
        //characterIcon
        characterIconImage.sprite = characterIcon;

        //Spell 1
        //Spell 2
        //Spell 3
        //Spell 4
        //Spell 5

        #region PrimaryStatIcon

        Image primaryIcon = null;

        //Strength
        if (heroType == "Strength")
        {
            primaryIcon = strengthIcon;
        }
        //Agility
        else if (heroType == "Agility")
        {
            primaryIcon = agilityIcon;
        }
        //Intelligence
        else if (heroType == "Intelligence")
        {
            primaryIcon = intelligenceIcon;
        }

        //Rect transform primaryIcon's width & height > 20x20
        //Place particle glow around it

        #endregion
    }

    public TextMeshProUGUI healthText;
    public TextMeshProUGUI manaText;

    public TextMeshProUGUI attackText;
    public TextMeshProUGUI additionalAttackText;
    public TextMeshProUGUI armorText;
    public TextMeshProUGUI additionalArmorText;

    public TextMeshProUGUI strengthText;
    public TextMeshProUGUI additionalSTRText;
    public TextMeshProUGUI agilityText;
    public TextMeshProUGUI additionalAGIText;
    public TextMeshProUGUI intelligenceText;
    public TextMeshProUGUI additionalINTText;

    //Updates all numbers onto UI
    private void UpdateCharacterDisplay()
    {
        //currentHealth/maxHealth
        healthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
        //currentMana/maxMana
        manaText.text = currentMana.ToString() + "/" + maxMana.ToString();

        //attackDamage + (green)additionalAttackDamage+statDamageBonus
        attackText.text = attackDamage.ToString();
        additionalAttackText.text = " + " + (additionalAttackDamage + statDamageBonus).ToString();
        //armor + (green)additionalArmor
        armorText.text = armor.ToString();
        additionalArmorText.text = " + " + additionalArmor.ToString();

        //strength + (green)additionalStrength
        strengthText.text = strength.ToString();
        additionalSTRText.text = " + " + additionalStrength.ToString();
        //agility + (green)additionalAgility
        agilityText.text = agility.ToString();
        additionalAGIText.text = " + " + additionalAgility.ToString();
        //intelligence + (green)additionalIntelligence
        intelligenceText.text = intelligence.ToString();
        additionalINTText.text = " + " + additionalIntelligence.ToString();

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




    //Resource Regen
    private bool fullHealth = true;
    private bool fullMana = true;
    //Regenerates health & mana constantly when possible
    private void HealthManaRegen()
    {
        //Health
        if (!fullHealth)
        {
            currentHealth += (strength * SystemsManager.strToHpRegen) * Time.deltaTime;
            //Cancels on full HP
            if (currentHealth >= maxHealth)
            {
                currentHealth = maxHealth;
                fullHealth = true;
            }
            //Displays UI
            ///TODO
            ///Update character's health info
        }
        //Mana
        if (!fullMana)
        {
            currentMana += (intelligence * SystemsManager.intToManaRegen) * Time.deltaTime;
            //Cancels on full HP
            if (currentMana >= maxMana)
            {
                currentMana = maxMana;
                fullMana = true;
            }
            //Displays UI
            ///TODO
            ///Update character's mana info
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
    public void DamageToTake(int damageType, float damage)
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
