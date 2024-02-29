using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(ParticleEffect))]
public class EditorParticleEffects : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ParticleEffect inspected = (ParticleEffect)target;

        GUILayout.Space(100f);

        if(inspected.particleEffect == null)
        {
            if (GUILayout.Button("Create Empty Prefab"))
            {
               inspected.particleEffect = CreateParticleEffectObject(inspected);
            }
        }

    }

    private GameObject CreateParticleEffectObject(ParticleEffect inspected)
    {
        // Get the path of the scriptable object asset
        string assetPath = AssetDatabase.GetAssetPath(inspected);

        // Get the folder path
        string folderPath = System.IO.Path.GetDirectoryName(assetPath);

        GameObject tempGo = new GameObject();
        tempGo.AddComponent<ParticleSystem>();
        // Create a new prefab instance
        GameObject newParticleEffectObject = PrefabUtility.SaveAsPrefabAsset(tempGo, folderPath + "/" + inspected.name + ".prefab");

        // Refresh the asset database
        AssetDatabase.Refresh();

        DestroyImmediate(tempGo);
        Debug.Log("did u win?");

        return newParticleEffectObject;
    }
}

# endif