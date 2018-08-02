using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class SystemsManager {

    //Armor > Physical Defense
    [Tooltip("Increase this number to make each armor point weaker")]
    static public int rateOfArmor = 600;

    //Base Attack Speed
    static public float attackSpeed = 2;
    static public float attackSpeedCap = 0.2f;

    //Primary Stat > Damage Conversion
    [Tooltip("Attack damage gained form 1 PrimaryStat")]
    static public int statToAttack = 1;

    //STR > Health Conversion
    [Tooltip("Health is gained from 1 STR")]
    static public int strToHealth = 10;
    //STR > Hp/S Conversion
    [Tooltip("Hp/S gained from 1 STR")]
    static public float strToHpRegen = 0.1f;


    //AGI > Armor Conversion
    [Tooltip("Armor gained from 1 AGI")]
    static public float agiToArmor = 0.05f;
    //AGI > AS Conversion
    [Tooltip("Attacks per second gained from 1 AGI")]
    static public float agiTOAttackSpeed = 0.0001f;


    //INT > Mana Conversion
    [Tooltip("Mana gained from 1 INT")]
    static public int intToMana = 10;
    //INT > Mp/S Conversion
    [Tooltip("Mana/S gained from 1 INT")]
    static public float intToManaRegen = 1f;

}
