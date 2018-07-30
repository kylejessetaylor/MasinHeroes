using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Character : Unit {

    [Header("Character Statistics")]

    //Character
    private float currentXP;
    private float maxXP;

    ///Resources

    ///Stats
    [Tooltip("'Strength', 'Agility', 'Intelligence'. Type only one of these three. This determines what style of gear/stats the player should aim for.")]
    private string heroType = "Strength";

    ///Offensive
    private float attackPerLevel;

    ///Defensive


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
    protected new void ApplyScriptableObjectStats()
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
        attackPerLevel = character.attackPerLevel;
        strPerLevel = character.strPerLevel;
        agiPerLevel = character.agiPerLevel;
        intPerLevel = character.intPerLevel;
}

    protected new void Awake()
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
    protected new void UpdateAllStats()
    {
        //Calculates Primary Stats, Health & Mana
        CalculateAll();

        //Updates UI with new calculations
        UpdateCharacterDisplay();
    }

    #region StatCalculations

    //Calculates Primary Stats, Health & Mana, Attack & Armor
    protected new void CalculateAll()
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
    protected void CalcStats()
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
    protected new void CalcResources()
    {
        //Health
        maxHealth = baseHealth + (strength * SystemsManager.strToHealth) + additionalHealth;

        //Mana
        maxMana = baseMana + (intelligence * SystemsManager.intToMana) + additionalMana;
    }

    //Calculates attack & armor at start of the game
    private int statDamageBonus;
    private new void CalcAttackNArmor()
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

    #endregion

    #region UIDisplayStats

    [Header("UI Naviation")]

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

    public TextMeshProUGUI strengthText;
    public TextMeshProUGUI additionalSTRText;
    public TextMeshProUGUI agilityText;
    public TextMeshProUGUI additionalAGIText;
    public TextMeshProUGUI intelligenceText;
    public TextMeshProUGUI additionalINTText;

    //Updates all numbers onto UI
    protected new void UpdateCharacterDisplay()
    {
        //currentHealth/maxHealth
        //currentMana/maxMana
        UpdateHealthManaTexT();

        ///attackDamage + (green)additionalAttackDamage+statDamageBonus
        attackText.text = (attackDamage + statDamageBonus).ToString();
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

        //------------------------------------------------------------//

        ///strength + (green)additionalStrength
        strengthText.text = strength.ToString();
        //Hides green if + 0
        if (additionalStrength > 0) {
            additionalSTRText.text = " + " + additionalStrength.ToString();
        }
        else
        {
            //Hides + 0
            additionalSTRText.text = "";
        }

        ///agility + (green)additionalAgility
        agilityText.text = agility.ToString();
        //Hides green if + 0
        if (additionalAgility > 0)
        {
            additionalAGIText.text = " + " + additionalAgility.ToString();
        }
        else
        {
            //Hides + 0
            additionalAGIText.text = "";
        }

        ///intelligence + (green)additionalIntelligence
        intelligenceText.text = intelligence.ToString();
        //Hides green if + 0
        if (additionalIntelligence > 0)
        {
            additionalINTText.text = " + " + additionalIntelligence.ToString();
        }
        else
        {
            //Hides + 0
            additionalINTText.text = "";
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

    //Regenerates health & mana constantly when possible
    private new void HealthManaRegen()
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
            UpdateHealthManaTexT();
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
            UpdateHealthManaTexT();
        }
    }
}
