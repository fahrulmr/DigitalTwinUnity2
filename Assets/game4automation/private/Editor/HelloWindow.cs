
using UnityEngine;
using UnityEditor;

namespace game4automation
{
    public class HelloWindow : EditorWindow
    {
        public string URLDoc = "https://game4automation.com/documentation/current/index.html";
        public string URLYoutube = "https://www.youtube.com/channel/UCiL22-4L3bkX-rz6bUN8ZYQ/videos";
        public string URLSupport = "https://forum.game4automation.com";
        public string URLRate = "https://assetstore.unity.com/packages/slug/139866";
        public string URLUpgrade = "https://assetstore.unity.com/packages/slug/143543";

        public string URLReleaseNotes =
            "https://forum.game4automation.com/knowledge-bases/2/articles/111-release-notes";

        public string URLTraining = "https://game4automation.com/en/learn/digitaltwin-workshop-2";
        public string TextHeader = "\nWelcome to game4automation! \n";

        public string TextInto = "\n" +
                                 "Game4Automation is an open framework for developing industrial digital twins. Game4automation can be used for simulation, virtual commissioning, and 3D Human Machine Interfaces. Let's change the game for Digital Twins - affordable, shared source, extendable, and with gaming power based on Unity.";

        public string TextStarted = "\nPlease check our online documentation to get started.";

        public string TextYoutube = "\nOn our Youtube channel, you can find several tutorials.";

        public string TextTraining = "\nYou would like to participate in an online training?";

        public string TextSupport = "\nOn our Forum, you can ask questions and get support if needed.";

        public string TextUpgrade =
            "\nYou need more functions like more automation interfaces or the ability to work with large CAD assemblies, then you could upgrade to game4automation Professional.";

        public string TextRate =
            "\nYou are happy with our solution? Please rate our solution on the Unity Asset store.";

        public string TextReleaseNotes =
            "\nIf you are upgrading from a previous version please first check our release notes.";



        public static Texture2D image = null;

        // Add menu named "My Window" to the Window menu
        [MenuItem("game4automation/Info")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            HelloWindow window = ScriptableObject.CreateInstance<HelloWindow>();
            window.Open();

        }

        public void Open()
        {
            var window = this;
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 500, 770);
            image = (Texture2D) UnityEngine.Resources.Load("Icons/hellowindow", typeof(Texture2D));
            Global.CenterOnMainWin(window);
            window.ShowPopup();
        }



        void OnGUI()
        {


            EditorGUILayout.LabelField("\n");
            EditorGUILayout.LabelField(TextHeader);

            EditorGUILayout.LabelField(Global.Version);

            GUILayout.Label(image);

            EditorGUILayout.LabelField(TextInto, EditorStyles.wordWrappedLabel);

            EditorGUILayout.LabelField(TextReleaseNotes, EditorStyles.wordWrappedLabel);
            if (GUILayout.Button("Check the release notes"))
            {
                Application.OpenURL(URLReleaseNotes);
            }

            EditorGUILayout.LabelField(TextStarted, EditorStyles.wordWrappedLabel);
            if (GUILayout.Button("Read documentation"))
            {
                Application.OpenURL(URLDoc);
            }

            EditorGUILayout.LabelField(TextYoutube, EditorStyles.wordWrappedLabel);
            if (GUILayout.Button("Watch Youtube"))
            {
                Application.OpenURL(URLYoutube);
            }

            EditorGUILayout.LabelField(TextTraining, EditorStyles.wordWrappedLabel);
            if (GUILayout.Button("Register for a training"))
            {
                Application.OpenURL(URLTraining);
            }


            EditorGUILayout.LabelField(TextSupport, EditorStyles.wordWrappedLabel);
            if (GUILayout.Button("Open the Forum"))
            {
                Application.OpenURL(URLSupport);
            }


            EditorGUILayout.LabelField(TextRate, EditorStyles.wordWrappedLabel);
            if (GUILayout.Button("Give us a rating"))
            {
                Application.OpenURL(URLRate);
            }


            EditorGUILayout.LabelField(TextUpgrade, EditorStyles.wordWrappedLabel);
            if (GUILayout.Button("Upgrade to Professional"))
            {
                Application.OpenURL(URLUpgrade);
            }

            EditorGUILayout.LabelField("\n\n");

            if (GUILayout.Button("Close"))
            {
                this.Close();
            }

        }
    }
}