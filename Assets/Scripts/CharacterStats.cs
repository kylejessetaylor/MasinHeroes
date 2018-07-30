using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName ="Character")]
public class CharacterStats : ScriptableObject {

    //Character
    public string characterName;
    public Sprite characterIcon;

    //Resources
    [Header("Resources")]
    public int baseHealth;
    public int baseMana;

    ///Stats
    [Tooltip("'Strength', 'Agility', 'Intelligence'. Type only one of these three. This determines what style of gear/stats the player should aim for.")]
    public string heroType = "Strength";

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
