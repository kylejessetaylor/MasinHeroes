using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour {

    //All CONTROLLABLE & FRIENDLY units to the player
    static public List<GameObject> alliedUnits = new List<GameObject>();

    //All CONTROLLABLE units to the player
    static public List<GameObject> controllableUnits = new List<GameObject>();

    //All FRIENDLY units to the player
    static public List<GameObject> friendlyUnits = new List<GameObject>();

    //All NEUTRAL units to the player
    static public List<GameObject> neutralUnits = new List<GameObject>();

    //All HOSTILE units to the player
    static public List<GameObject> hostileUnits = new List<GameObject>();

}
