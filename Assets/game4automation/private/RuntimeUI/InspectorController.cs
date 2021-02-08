using System.Collections;
using System.Collections.Generic;
using RuntimeInspectorNamespace;
using UnityEngine;

namespace game4automation
{

    public class InspectorController : MonoBehaviour
    {
        [Header("Connected Compponents")] public RuntimeHierarchy RuntimeHierarchy;
        public RuntimeInspector RuntimeInspector;

        public WindowController InspectorSlideButton;

        [Header("Display Options")] public bool ExpandInspectorItems;

        [Header("Global Hide")] public bool HideTransform;

        public bool HideMeshFilters;

        public bool HideMeshRenderers;

        public bool HideRigidBody;

        public bool HideCollider;

        public List<string> DontShowObjectWithNames = new List<string>();

        private List<Inspector> inspectors = new List<Inspector>();



        // Start is called before the first frame update
        void Start()
        {

            RuntimeHierarchy.OnSelectionChanged += RuntimeHierarchySelection;
            InspectorSlideButton.OnWindowClose += InspectorSlideButtonOnOnWindowClose;
        }

        private void InspectorSlideButtonOnOnWindowClose(WindowController windowcontroller)
        {
            RuntimeHierarchy.ConnectedInspector = null;
        }

        private void RuntimeHierarchySelection(Transform selection)
        {
            // Open Inspector if not yet opened
            if (!InspectorSlideButton.gameObject.activeSelf)
            {

                InspectorSlideButton.gameObject.SetActive(true);
                InspectorSlideButton.OpenWindow(true);
                RuntimeHierarchy.ConnectedInspector = RuntimeInspector;
            }

        }

        public void Add(Inspector inspector)
        {
            inspectors.Add(inspector);
            RuntimeHierarchy.AddToPseudoScene(inspector.HierarchyName, inspector.transform);
        }

        public bool DisplayThisField(string field)
        {
            return false;
        }

        public bool DisplayThisComponent(Component component)
        {
            bool show = true;

            if (HideTransform)
            {
                if (component is Transform)
                    show = false;
            }

            if (HideMeshFilters)
            {
                if (component is MeshFilter)
                    show = false;
            }

            if (HideMeshRenderers)
            {
                if (component is MeshRenderer)
                    show = false;
            }

            if (HideCollider)
            {
                if (component is Collider)
                    show = false;
            }

            if (HideRigidBody)
            {
                if (component is Rigidbody)
                    show = false;
            }

            return show;
        }

        public bool DisplayThisObject(GameObject obj)
        {
            var name = obj.name;

            bool show = true;

            if (DontShowObjectWithNames.Contains(name))
            {
                show = false;
            }

            return show;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
