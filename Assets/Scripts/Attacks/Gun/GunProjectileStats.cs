using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunProjectileStats : MonoBehaviour
{
    public Projectile prefab;
    public Transform muzzle;
    public int projectileCount = 1;
}


// Custom editor from GaticusHax for directly editing prefab properties. IdentScope is custom though and must be replaced

/*

using UnityEngine;
using UnityEditor;

using EditorTools;

//[CustomPropertyDrawer( typeof( Object ), true )]
//[CustomPropertyDrawer( typeof( ScriptableObject ), true )]
public class ObjectDrawer : PropertyDrawer {
    // Cached scriptable object editor
    private Editor editor = null;

    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label ) {
        // Draw label
        EditorGUI.PropertyField( position, property, label, true );

        // Draw foldout arrow
        if ( property.objectReferenceValue != null ) {
            property.isExpanded = EditorGUI.Foldout( position, property.isExpanded, GUIContent.none );
        }

        if ( !property.isExpanded ) return;

        // Draw foldout properties
        using ( var indentScope = new IndentScope() ) {
            // Draw object properties
            if ( !editor ) Editor.CreateCachedEditor( property.objectReferenceValue, null, ref editor );
            // Draw object properties
            EditorGUI.BeginChangeCheck();
            if ( editor ) editor.OnInspectorGUI(); // catch empty property
            if ( EditorGUI.EndChangeCheck() ) property.serializedObject.ApplyModifiedProperties();
        }
    }
}

*/