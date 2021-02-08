using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace game4automation
{
	public class SceneStats : MonoBehaviour
	{
		public Text TextField;

		private int _objectCount;
		private int _dynamicCount;
		private int _polygonCount;
		private string _sceneName;
		private string _version;
		private int _screen;
		private int _physics;
		private RectTransform _rect;
		private float _lastphysicscycle;
		private const string MessageFormat = "{0}\nVertices: {1}\nGraphic : {2}ms, Physics : {3}ms, Time : {4}s";

		private void Start()
		{
			_screen = 60;
			_rect = GetComponent<RectTransform>();
			_version = Global.Version;
			var meshes = FindObjectsOfType<MeshFilter>();

			foreach (var mesh in meshes)
				if (mesh.sharedMesh != null)
				{
					_polygonCount += mesh.sharedMesh.vertexCount;
				}
		}
		

		// Update is called once per frame
		private void Update()
		{
			_screen = Mathf.RoundToInt(Time.smoothDeltaTime * 1000);
			_sceneName = SceneManager.GetActiveScene().name;
			string time = Time.time.ToString("0.00");
			// Debug.logge
			// _rect.localPosition = new Vector3(Screen.width / 2, -30, 0);
			TextField.text = string.Format(MessageFormat, _sceneName+" " + _version, _polygonCount, _screen, _physics,time);
		}

		// Update is called once per frame
		private void FixedUpdate()
		{
			_physics = Mathf.RoundToInt((Time.unscaledTime - _lastphysicscycle) * 1000);
			_lastphysicscycle = Time.unscaledTime;
		}


	
	}
}