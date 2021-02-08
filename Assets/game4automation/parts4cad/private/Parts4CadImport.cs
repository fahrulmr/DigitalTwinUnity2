﻿﻿// Game4Automation (R) Parts4Cad
// (c) 2019 in2Sight GmbH - Usage of this source code only allowed based on License conditions see https://game4automation.com/lizenz  

using UnityEditor;
using System.Diagnostics;
using System;
using UnityEngine.Networking;
using System.IO;
using System.Collections;
using UnityEngine;
using Debug = UnityEngine.Debug;
using System.Threading.Tasks;


 namespace game4automation
 {
   #if UNITY_EDITOR
     public static class Parts4CadImport
     {


         public static void UpdateParts4Cad(Parts4Cad parts4cad)
         {
           
             OpenParts4Cad(parts4cad);
    
         }


         public static async void OpenParts4Cad(Parts4Cad parts4cad)
         {
             

             Parts4CadSettings settings =
                 (Parts4CadSettings) AssetDatabase.LoadAssetAtPath<ScriptableObject>(
                     "Assets/game4automation/parts4cad/Parts4CadSettings.asset");


             var apppath = Application.streamingAssetsPath + "/parts4cad/";
             var startpath = apppath + "bin/x86/64/cscripthost.exe";
             var sciptpaht = apppath + "setup/scripts/common/startseamless_parts4cad.vbb";
             var outputpath = Path.GetTempPath() + "cadenas_part4cad_output.dae";
             var format = "COLLADA";
             var appname = "game4automation";
             var key = "280c2a13ea0444c999ce9eb38e72e7be";

             if (!CheckInstall())
                 return;
             
             Process myProcess = null;
             Debug.Log("Starting parts4cad");
             EditorUtility.DisplayProgressBar("parts4cad", "parts4cad is starting", 0.5f);
             FileUtil.DeleteFileOrDirectory(outputpath);
             try
             {
                 using (myProcess = new Process())
                 {
                     myProcess.StartInfo.UseShellExecute = false;

                     myProcess.StartInfo.FileName = startpath;
                     if (parts4cad == null)
                     {
                         myProcess.StartInfo.Arguments =
                             string.Format(
                                 "\"{0}\" -cadname \"{1}\" -path \"{2}\" -format \"{3}\" -key {4}",
                                 sciptpaht, appname, outputpath, format, key);
                     }
                     else
                     {

                         var mident = GetMident(parts4cad.Mident);

                         myProcess.StartInfo.Arguments =
                             string.Format(
                                 "\"{0}\" -cadname \"{1}\" -path \"{2}\" -format \"{3}\"  -mident \"{4}\" -key {5}",
                                 sciptpaht, appname, outputpath, format, mident, key);
                     }

                     myProcess.StartInfo.CreateNoWindow = true;
                     myProcess.Start();
                     await WaitOneSecondAsync();
                     myProcess.WaitForExit();

                     var path = Path.GetTempPath() + "cadenas_part4cad_output.dae";

                     if (System.IO.File.Exists(path))
                     {
                         EditorUtility.DisplayProgressBar("parts4cad", "Importing part into Unity", 0.9f);
                         ImportFile(path,parts4cad);
                         EditorUtility.ClearProgressBar();
                     }
                     else
                     {
                         EditorUtility.ClearProgressBar();
                     }
                 }


             }
             catch (Exception e)
             {
                 UnityEngine.Debug.Log(e.Message);
             }
           

         }

         private static bool CheckInstall()
         {
             
             var sourcepath = Application.streamingAssetsPath + "/parts4cad.zip";
             var destpath = Application.streamingAssetsPath + "/parts4cad/";

             // If parts4cad not allready in sreaming assets then extract it to Streaming assets
             if (!Directory.Exists(destpath))
             {
                 if (!File.Exists(sourcepath))
                 {
                     if (EditorUtility.DisplayDialog("parts4cad Installation",
                         "You are using parts4cad the first time. parts4cad needs a client application to be downloaded and unzipped into your StreamingAssets folder in the project. Are you OK with this?","OK","CANCEL"))
                     {
                         DownloadParts4Cad();
                     } else
                     {
                         return false;
                     }
                 }

                 EditorUtility.DisplayProgressBar("Please wait some minutes",
                     "parts4cad zip is now unzipped into StreamingAssets", 0.2f);
                 // Unpack and then delete file to reduce project size
                 ZipUtil.Unzip(sourcepath, Application.streamingAssetsPath);
                 EditorUtility.DisplayProgressBar("Please Wait", "parts4cad is starting soon", 0.3f);

                 FileUtil.DeleteFileOrDirectory(sourcepath);

                 AssetDatabase.Refresh();

                
             }
             return true;
            
         }
         
         static void DownloadParts4Cad()
         {

             IEnumerator enumerator = Download();
             enumerator.MoveNext();
             while (!((UnityWebRequestAsyncOperation) enumerator.Current).isDone)
             {
                 EditorUtility.DisplayProgressBar("Please wait some minutes", "parts4cad download as zip to Streaming Assets",
                     ((UnityWebRequestAsyncOperation) enumerator.Current).progress);
             }

             enumerator.MoveNext();
             EditorUtility.ClearProgressBar();
          
         }

         static IEnumerator Download()
         {
             string url = "http://game4automation.com/download/parts4cad.zip";

             string savepath = Path.Combine(Application.streamingAssetsPath, "parts4cad.zip");


             var uwr = new UnityWebRequest(url);
             uwr.method = UnityWebRequest.kHttpVerbGET;
             var dh = new DownloadHandlerFile(savepath);
             dh.removeFileOnAbort = true;
             uwr.downloadHandler = dh;
             yield return uwr.SendWebRequest();

             if (uwr.isNetworkError || uwr.isHttpError)
                 Debug.Log(uwr.error);
             else
                 Debug.Log("Parts4Cad saved ");
         }

         static async Task WaitOneSecondAsync()
         {
             await Task.Delay(TimeSpan.FromSeconds(1));
             EditorUtility.DisplayProgressBar("parts4cad", "Plese select parts and close Parts4Cad to continue", 0.7f);
         }

         static void ImportFile(string file,Parts4Cad parts4cad)
         {
             Parts4CadSettings settings =
                 (Parts4CadSettings) AssetDatabase.LoadAssetAtPath<ScriptableObject>(
                     "Assets/game4automation/parts4cad/Parts4CadSettings.asset");

             var path = Path.GetTempPath() + "cadenas_part4cad_output.dae.txt";
             var mident = ReadTextFile(path);
             var name = GetMidentAttribute(mident, "NN");
             var oderid = GetMidentAttribute(mident, "LINA");
             var vendor = GetMidentAttribute(mident, "VENDOR");

             var destpath = Application.dataPath + "/game4automation/parts4cad/Imported/" + vendor + "-" + oderid + ".dae";
             var assetpath = "Assets/game4automation/parts4cad/Imported/" + vendor + "-" + oderid + ".dae";
             FileUtil.DeleteFileOrDirectory(destpath);
             FileUtil.CopyFileOrDirectory(file, destpath);
             AssetDatabase.Refresh();
             
             var where = Selection.activeGameObject;
            
           
             if (parts4cad != null)
             {
                 var delobj = parts4cad.gameObject;
                 if (parts4cad.gameObject.transform.parent!= null)
                   where = parts4cad.gameObject.transform.parent.gameObject;
                else
                    where = null;
         
                UnityEngine.Object.DestroyImmediate(delobj, true);
             }

             var obj = AddComponent(assetpath, where);
             if (settings.ShortNamingInScene)
             {
                 obj.name = vendor + "-" + name;
             }

             if (settings.RotateToUnityStandard)
             {
                 obj.transform.eulerAngles = new Vector3(90, 0, 0);
             }

             parts4cad = obj.AddComponent<Parts4Cad>();
             
             parts4cad.Mident = mident;
             parts4cad.Name = GetMidentAttribute(mident, "NN");
             parts4cad.Description = GetMidentAttribute(parts4cad.Mident, "NT");
             parts4cad.OrderID = oderid;
             parts4cad.Catalog = GetMidentAttribute(parts4cad.Mident, "CATALOG");
             parts4cad.Vendor = vendor;
             parts4cad.Supplier = GetMidentAttribute(parts4cad.Mident, "SUPPLIER");
             parts4cad.Attributes = GetMidentAttributes(parts4cad.Mident);
             FileUtil.DeleteFileOrDirectory(file);
             FileUtil.DeleteFileOrDirectory(path);
         }


         static string ReadTextFile(string path)
         {
             //Read the text from directly from the test.txt file
             StreamReader reader = new StreamReader(path);
             var file = reader.ReadToEnd();
             reader.Close();
             return file;
         }


         static void WriteTextFile(string path, string text)
         {

             StreamWriter writer = new StreamWriter(path, true);
             writer.Write(text);
             writer.Close();
         }

         static string GetMident(string mident)
         {
             var ident1 = "MIDENT=";
             var pos1 = mident.IndexOf(ident1) + ident1.Length;

             var ident2 = "[Attributes]";
             var pos2 = mident.IndexOf(ident2) - 2;

             var midentcontent = mident.Substring(pos1, pos2 - pos1);

             return midentcontent;
         }

         static string GetMidentAttribute(string mident, string theattribute)
         {
             var ident = "[Attributes]";
             var pos = mident.IndexOf(ident) + ident.Length;
             string attributes = mident.Remove(0, pos);

             string myattribute = theattribute + "=";
             var pos2 = attributes.IndexOf(myattribute) + myattribute.Length;
             var attribute = attributes.Substring(pos2);
             var poslf = attribute.IndexOf("\r\n");
             attribute = attribute.Substring(0, poslf);
             return attribute;
         }

         static string GetMidentAttributes(string mident)
         {
             var ident = "[Attributes]";
             var pos = mident.IndexOf(ident) + ident.Length;
             string attributes = mident.Remove(0, pos);


             return attributes;
         }

         static GameObject AddComponent(string assetpath, GameObject where)
         {
             Parts4CadSettings settings =
                 (Parts4CadSettings) AssetDatabase.LoadAssetAtPath<ScriptableObject>(
                     "Assets/game4automation/parts4cad/Parts4CadSettings.asset");

             GameObject component = null;
          
             component = where;

             UnityEngine.Object prefab = AssetDatabase.LoadAssetAtPath(assetpath, typeof(GameObject));
             GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
             if (settings.UnpackWhenImported)
                 PrefabUtility.UnpackPrefabInstance(go, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
             go.transform.position = new Vector3(0, 0, 0);
             if (component != null)
             {
                 go.transform.parent = component.transform;

             }

             Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
             return go;

         }
     }
#endif
 }
