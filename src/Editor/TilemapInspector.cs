using UnityEngine;
using UnityEditor;
using System.Collections;


namespace Symphony {
    [CustomEditor (typeof(TileMap))]
    public class TilemapInspector : Editor {

        TileMap tilemap;
        GUIStyle style;
        
        public void OnEnable() {
            tilemap = (TileMap) target;
        }

        public override void OnInspectorGUI() {
            EditorGUILayout.HelpBox("Only use this script for TMX Map Format", MessageType.Info);
            EditorGUILayout.HelpBox("Only use TMX file with CSV data, also change the extension .tmx to .xml. " +
                                    "You can drop XML file from Asset Explorer.", MessageType.Warning);
            EditorGUILayout.HelpBox("Create a folder inside folder Resources (create folder Resources inside " +
                                    "Asset directory if you do not have one) and put all using tilesets there. " +
                                    "Write created folder name inside this field.", MessageType.Warning);
            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Pixels to Units");
            tilemap.unit = EditorGUILayout.FloatField(tilemap.unit, GUILayout.Width(160));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Tilesets Folder");
            tilemap.tileFolder = EditorGUILayout.TextField(tilemap.tileFolder, GUILayout.Width(160));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Tilemap XML");
            tilemap.tileXml = (TextAsset) EditorGUILayout.ObjectField(tilemap.tileXml, typeof(TextAsset), false, GUILayout.Width(160));
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.stretchWidth = true;
            if (GUILayout.Button("Render/Reload Tilemap", buttonStyle)) {   
                tilemap.RenderMap();
            }
            if (GUILayout.Button("Clear Tilemap", buttonStyle)) {   
                tilemap.ClearMap();
            }
        }

    }
}