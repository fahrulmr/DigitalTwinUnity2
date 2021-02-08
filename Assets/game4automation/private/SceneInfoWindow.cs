#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace game4automation
{



    public class SceneInfoWindow : EditorWindow
    {
        public string Header = "";
        public string Info = "The Info text";
        public string Button1Text = "Button Text";
        public string Button1URL = "";
        public string Button2Text;
        public string Button2URL;
        public string Button3Text;
        public string Button3URL;
        public Texture2D Image;
        public int Width = 500;
        public int Height = 770;

        private SceneInfo _sceneInfo;

        public void Open(SceneInfo sceneinfo)
        {
            // Get existing open window or if none, make a new one:
            _sceneInfo = sceneinfo;
            SceneInfoWindow window = this;
            window.position = new Rect(Screen.width / 2, Screen.height / 2, Width, Height);

            Global.CenterOnMainWin(window);
            window.ShowPopup();
        }


        void OnGUI()
        {
            if (Header != "")
            {
                EditorGUILayout.LabelField("\n");
                EditorGUILayout.LabelField(Header, EditorStyles.wordWrappedLabel);
            }

            if (Image != null)
                GUILayout.Label(Image);

            EditorGUILayout.LabelField("\n");
            EditorGUILayout.LabelField(Info, EditorStyles.wordWrappedLabel);

            if (Button1URL != "")
                if (GUILayout.Button(Button1Text))
                {
                    Application.OpenURL(Button1URL);
                }

            if (Button2URL != "")
                if (GUILayout.Button(Button2Text))
                {
                    Application.OpenURL(Button2URL);
                }

            if (Button3URL != "")
                if (GUILayout.Button(Button3Text))
                {
                    Application.OpenURL(Button3URL);
                }

            EditorGUILayout.LabelField("\n");

            if (GUILayout.Button("Close and hide this message next time"))
            {
                _sceneInfo.ShowOnSceneOpen = false;
                this.Close();
            }
            
            if (GUILayout.Button("Close"))
            {
                this.Close();
            }

        }
    }
}
#endif