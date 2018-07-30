using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName ="Ability")]
public class Ability : ScriptableObject {

    //Abilities name
    public string abilityName;
    //Abilities flavour text
    [Tooltip("Type (backwards slash)n to start on another line.")]
    public string flavour;

    //Abilities description
    [Tooltip("Type (backwards slash)n to start on another line.")]
    public string description;

    //Abilitie's Icon
    public Sprite icon;

    //Level this spell auto unlocks at
    public int levelUnlock;

    //Whether the ability is a passive or active ability
    public bool active;
    //Hotkey used to cast ability
    public KeyCode hotkey;

    //What kinds of targets spell can hit
    public targetType target = targetType.ENEMY;
    public enum targetType { ENEMY, FRIENDLY, BOTH, SELF }

    ////Damage
    [Header("Damage")]
    public float strength;
    public float agility;
    public float intelligence;

}
