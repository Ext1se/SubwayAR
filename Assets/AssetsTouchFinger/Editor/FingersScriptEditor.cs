using System;

using UnityEngine;
using UnityEditor;

namespace DigitalRubyShared
{
    [CustomEditor(typeof(FingersScript))]
    public class FingersScriptEditor : Editor
    {
        private Texture2D logo;

        public override void OnInspectorGUI()
        {

            string[] guids = AssetDatabase.FindAssets("FingersScriptLogo");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                logo = AssetDatabase.LoadMainAssetAtPath(path) as Texture2D;
                if (logo != null)
                {
                    break;
                }
            }

            DrawDefaultInspector();
        }
    }
}