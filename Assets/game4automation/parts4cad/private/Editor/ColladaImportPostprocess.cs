using UnityEditor;
using UnityEngine;

namespace game4automation
{
    public class ColladaImportPostprocess : AssetPostprocessor
    {

        public void OnPreprocessModel()
        {

            Parts4CadSettings settings =
                (Parts4CadSettings) AssetDatabase.LoadAssetAtPath<ScriptableObject>(
                    "Assets/game4automation/Parts4Cad/Parts4CadSettings.asset");

            if (settings != null)
            {
                ModelImporter modelImporter = assetImporter as ModelImporter;
                if (assetPath.Contains("parts4cad"))
                {
                    modelImporter.globalScale = settings.ImportScale;
                    modelImporter.addCollider = false;
                    modelImporter.preserveHierarchy = true;
                    Debug.Log("parts4cad Import Posprocessor");
                }
            }
        }
    }
}