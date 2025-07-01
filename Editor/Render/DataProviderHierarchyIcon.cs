#nullable enable
using System.Collections.Generic;
using Platonic.Render;
using UnityEditor;
using UnityEngine;

namespace Platonic.Editor.Render
{
    [InitializeOnLoad]
    public static class DataProviderHierarchyIcon
    {
        private static Dictionary<int, GUIContent> _cachedIcons = new Dictionary<int, GUIContent>();
        private static GUIContent _dataProviderIcon;

        static DataProviderHierarchyIcon()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
            _dataProviderIcon = EditorGUIUtility.IconContent("d_ScriptableObject Icon");
        }

        private static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            // Get the GameObject instance from the ID
            var instance = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            
            if (instance == null) return;
            
            // Check if this GameObject has a DataProvider component
            if (instance.GetComponent<DataProvider>() != null)
            {
                // Calculate icon position (right-aligned in the hierarchy view)
                Rect iconRect = new Rect(selectionRect);
                iconRect.x = iconRect.width + iconRect.x - 20f; // 20 pixels from the right edge
                iconRect.width = 16f;
                iconRect.height = 16f;
                
                // Draw the icon
                GUI.Label(iconRect, _dataProviderIcon);
            }
        }
    }
}