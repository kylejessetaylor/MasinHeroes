using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName ="Ability")]
public class Ability : ScriptableObject {

    //Abilities name
    public string abilityName;
    //Abilities flavour text
    public string flavour;

    //Abilities description
    public string desc;

    //Whether the ability is a passive or active ability
    public bool active;
    //Hotkey used to cast ability
    public KeyCode key;


}
