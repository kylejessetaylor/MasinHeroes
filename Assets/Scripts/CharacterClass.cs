using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "New Character", menuName ="Character")]
public class CharacterClass : ScriptableObject {

    //Character
    public string characterName;
    public Sprite characterIcon;

    ///Type of unit
    public Unit unitType = Unit.CONTROLLABLE;
    public enum Unit { CONTROLLABLE, FRIENDLY, NEUTRAL, HOSTILE }

    public float moveSpeed;

    ///Stats
    public Stat primaryStat = Stat.STRENGTH;
    public enum Stat { STRENGTH, AGILITY, INTELLIGENCE }

    //Spells
    [Tooltip("Place in order: 'Passive, Active1, Active2, Active3... Active5'")]
    public List<Ability> abilities = new List<Ability>();

    //Resources
    [Header("Resources")]
    public int baseHealth;
    public int baseMana;

    //Offensive
    public int baseAttackDamage;
    public float attackPerLevel;
    public bool melee;
    public int range;

    //Defensive
    public int baseArmor;

    //Raw Stats
    public int baseStrength;
    public int baseAgility;
    public int baseIntelligence;
    //Stats per level
    public float strPerLevel;
    public float agiPerLevel;
    public float intPerLevel;

}
