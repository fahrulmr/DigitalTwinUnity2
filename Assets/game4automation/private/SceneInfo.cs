
using game4automationtools;
using UnityEngine;

namespace game4automation
{
    [ExecuteInEditMode]
    public class SceneInfo : MonoBehaviour
    {
        public string HeaderText;
        [TextArea] public string InfoText;
        public Texture2D Image;
        public int Width;
        public int Height;
        public string ButtonText;
        public string ButtonURL;
        public string Button2Text;
        public string Button2URL;
        public string Button3Text;
        public string Button3URL;

        public bool ShowOnSceneOpen = true;

        [Button(("Show Info"))]
        public void OnSceneLoad()
        {
#if UNITY_EDITOR
            if (ShowOnSceneOpen)
            {
                var window = ScriptableObject.CreateInstance<SceneInfoWindow>();
                //var window = (SceneInfoWindow)EditorWindow.GetWindow(typeof(SceneInfoWindow));
                window.Info = InfoText.Replace("\\n", "\n");
                window.Button1URL = ButtonURL;
                window.Button1Text = ButtonText;
                window.Button2Text = Button2Text;
                window.Button2URL = Button2URL;
                window.Button3URL = Button3URL;
                window.Button3Text = Button3Text;
                window.Image = Image;
                window.Header = HeaderText;
                window.Height = Height;
                window.Width = Width;
                window.Open(this);
            }

#endif
        }
    }
}
