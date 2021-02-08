﻿using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;
 
public class InstalledPackages : AssetPostprocessor
{
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        var inPackages = importedAssets.Any(path => path.StartsWith("game4automation/")) ||
                         deletedAssets.Any(path => path.StartsWith("game4automation/")) ||
                         movedAssets.Any(path => path.StartsWith("game4automation/")) ||
                         movedFromAssetPaths.Any(path => path.StartsWith("game4automation/"));
 
        if (inPackages)
        {
            InitializeOnLoad();
        }
    }
   
    [InitializeOnLoadMethod]
    private static void InitializeOnLoad()
    {
        var listRequest = Client.List(true);
        while (!listRequest.IsCompleted)
            Thread.Sleep(100);
 
        if (listRequest.Error != null)
        {
            Debug.Log("Error: " + listRequest.Error.message);
            return;
        }
 
        var packages = listRequest.Result;
        var text = new StringBuilder("Packages:\n");
        foreach (var package in packages)
        {
            if (package.source == PackageSource.Registry)
                text.AppendLine($"{package.name}: {package.version} [{package.resolvedPath}]");
        }
       
        Debug.Log(text.ToString());
    }
}