using System.Reflection;
using game4automation;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class HideNonG4AObjects
{
    static HideNonG4AObjects()
    {
        Selection.selectionChanged += OnSelectionChange;
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
    }
    static void OnSelectionChange()
    {
        if (Selection.activeTransform == null)
            return;
        var obj = Selection.activeTransform.gameObject;
        var com = obj.GetComponent<Game4AutomationBehavior>();
        if (com!=null)
        {
            Hide(obj,com.HideNonG44Components);
        }
    }
    
    static void OnHierarchyChanged()
    {
       
    }

 
    private  static void Hide(GameObject gameobject,bool hide)
    {
    
        var components = gameobject.GetComponents<Component>();
        foreach (var component in components)
        {
            var ctype = component.GetType();
            bool subclass = ctype.IsSubclassOf(typeof(Game4AutomationBehavior));
            if (!subclass)
            {
                if (ctype != typeof(Transform))
                {
                    if (hide)
                        component.hideFlags = HideFlags.HideInInspector;
                    else
                        component.hideFlags = HideFlags.None;
                }
            }
            else
            {
                ((Game4AutomationBehavior) component).HideNonG44Components = hide;
            }
        }

        if (hide)
        {
            Texture2D texture = (Texture2D) Resources.Load("Icons/Icon48");
            SetIcon(gameobject, texture);
        }
        else
        {
            ClearIcon(gameobject);
        }
    }
    
    public static void ClearIcon( GameObject gObj ) {
        SetIcon( gObj, (Texture2D)null );
    }
        
    private static void SetIcon( GameObject gObj, Texture2D texture ) {
        var ty = typeof( EditorGUIUtility );
        var mi = ty.GetMethod( "SetIconForObject", BindingFlags.NonPublic | BindingFlags.Static );
        mi.Invoke( null, new object[] { gObj, texture } );
    }
    
    [MenuItem("GameObject/Game4Automation/Only G4A Components",false,0)]
    public static void GOShowOnlyG4AComponents()
    {
        foreach (var obj in Selection.objects)
        {
            var gameobject = (GameObject) obj;
            Hide(gameobject,true);
        }
     

    }
    [MenuItem("GameObject/Game4Automation/Show all Components",false,0)]
    public static void GOHideAllComponents()
    {
        foreach (var obj in Selection.objects)
        {
            var gameobject = (GameObject) obj;
            Hide(gameobject,false);
        }
    }
    [MenuItem("CONTEXT/Component/Game4Automation/Only G4A")]
    public static void ShowOnlyG4AComponents(MenuCommand command)
    {
        var gameobject =  command.context;
        var obj = (Component)gameobject;
        Hide(obj.gameObject,true);

    }
    [MenuItem("CONTEXT/Component/Game4Automation/Show all")]
    public static void ShowAllComponents(MenuCommand command)
    {
        var gameobject =  command.context;
        var obj = (Component)gameobject;
        Hide(obj.gameObject,false);

    }
}