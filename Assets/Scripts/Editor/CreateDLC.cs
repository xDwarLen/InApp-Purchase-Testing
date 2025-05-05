using System;
using UnityEngine;
using UnityEditor;

public class CreateDLC
{
    [MenuItem("Assets/Create DLC")]
    private static void BuildAllDLC()
    {
        string DLCDirectoryPath = Application.dataPath + "/../DLC";
        try
        {
            BuildPipeline.BuildAssetBundles(DLCDirectoryPath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        }
        catch(Exception e)
        {
            Debug.LogWarning(e);
        }
    }
}
