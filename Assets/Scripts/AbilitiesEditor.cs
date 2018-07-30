//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//[CustomEditor(typeof(Abilities))]
//public class AbilitiesEditor : Editor {

//    public override void OnInspectorGUI()
//    {
//        // If we call base the default inspector will get drawn too.
//        // Remove this line if you don't want that to happen.
//        base.OnInspectorGUI();

//        Abilities ability = target as Abilities;

//        ability.active = EditorGUILayout.Toggle("active", ability.active);

//        if (ability.active)
//        {
//            ability.key = EditorGUILayout.Keycode
//        }
//    }
//}
