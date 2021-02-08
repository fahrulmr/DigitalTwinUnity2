
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#pragma warning disable 0168
#pragma warning disable 0649

namespace game4automation
{
    //! Controls floating windows during simulation / gamemode for hierarchy / inspector and automation UI display
    public class WindowController : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        public GameObject Window;
        public GameObject UpdateLayoutGroup;
        public GameObject OpenIcon;
        public GameObject CloseIcon;


        public bool TranslateInY = false;
        public bool HideIconWhenClosed = false;
        public bool SetPassiveWhenClosed = true;
        public bool WindowClosed = true;
        public float OpenCloseSpeed = 1f;

        private float _standardsize;
        private float _lastsize;
        private float _startwindowopen;
        private RectTransform _windowtransform;
        private bool dragging = false;

        private float _startdragpos;
        private Vector2 _startsize;
        private RectTransform _transform;
        private RectTransform _updatelayoutgroup;
        private RectTransform _thisrecttransform;
        private CanvasScaler _canvas;

        public delegate void WindowControllerOnWindowOpen(WindowController windowcontroller);

        public event WindowControllerOnWindowOpen OnWindowOpen;

        public delegate void WindowControllerOnWindowClose(WindowController windowcontroller);

        public event WindowControllerOnWindowClose OnWindowClose;

        public void Start()
        {
            _windowtransform = Window.GetComponent<RectTransform>();
            if (TranslateInY)
                _standardsize = _windowtransform.sizeDelta.y;
            else
                _standardsize = _windowtransform.sizeDelta.x;
            _lastsize = _standardsize;
            OpenWindow(!WindowClosed);

            _canvas = GetComponentInParent<CanvasScaler>();
            _thisrecttransform = GetComponent<RectTransform>();
            _transform = Window.GetComponent<RectTransform>();
            if (GetComponent<WindowController>().UpdateLayoutGroup != null)
                _updatelayoutgroup = GetComponent<WindowController>().UpdateLayoutGroup.GetComponent<RectTransform>();
        }

        public void PointerDown(float size)
        {
            if (WindowClosed)
            {
                OpenWindow(true);
                _startwindowopen = Time.unscaledTime;
            }
        }

        public void PointerUp(float size)
        {

            // If not dragged and not opened just before
            if (!(System.Math.Abs(size - _startdragpos) > 10))
            {
                if (!WindowClosed)
                {
                    var delta = Time.unscaledTime - _startwindowopen;
                    // Window was lon ooened, no short click - close it
                    if (delta > 0.3f)
                    {
                        OpenWindow(false);
                    }

                    if (delta < 0.3f)
                    {
                        // Short click - no dragging - set to size

                        if (!TranslateInY)
                            _windowtransform.sizeDelta = new Vector2(_lastsize, _windowtransform.sizeDelta.y);
                        else
                            _windowtransform.sizeDelta = new Vector2(_windowtransform.sizeDelta.x, _lastsize);
                    }
                }
            }
        }

        public void OpenWindow(bool open)
        {
            if (open)
            {
                var max = 0;
                if (TranslateInY)
                    max = Screen.height - 30;
                else
                    max = Screen.width - 30;

                if (_lastsize < 30)
                {
                    _lastsize = _standardsize;
                }

                if (_lastsize > Screen.width - 30)
                {
                    _lastsize = _standardsize;
                }

                Window.SetActive(true);
                if (!TranslateInY)
                    _windowtransform.sizeDelta = new Vector2(_lastsize, _windowtransform.sizeDelta.y);
                else
                    _windowtransform.sizeDelta = new Vector2(_windowtransform.sizeDelta.x, _lastsize);
                WindowClosed = false;
                OpenIcon.SetActive(false);
                CloseIcon.SetActive(true);
            }
            else
            {
                if (!TranslateInY)
                    _lastsize = _windowtransform.sizeDelta.x;
                else
                    _lastsize = _windowtransform.sizeDelta.y;
                if (SetPassiveWhenClosed)
                {
                    Window.SetActive(false);
                }

                if (!TranslateInY)
                    _windowtransform.sizeDelta = new Vector2(0, _windowtransform.sizeDelta.y);
                else
                    _windowtransform.sizeDelta = new Vector2(_windowtransform.sizeDelta.x, 0);


                WindowClosed = true;
                OpenIcon.SetActive(true);
                CloseIcon.SetActive(false);
                if (HideIconWhenClosed)
                    gameObject.SetActive(false);

                if (OnWindowClose != null)
                    OnWindowClose(this);
            }
        }

        public void ToggleWindow()
        {
            if (WindowClosed)
            {
                if (OnWindowOpen != null)
                    OnWindowOpen(this);
                OpenWindow(true);
            }
            else
            {
                if (OnWindowClose != null)
                    OnWindowClose(this);
                OpenWindow(false);
            }
        }

        private float GetMousePos()
        {
            Vector3 screen;

            var posi = Input.mousePosition;


            if (TranslateInY)
                return posi.y * _canvas.referenceResolution.y / Screen.height;
            else
                return posi.x * _canvas.referenceResolution.x / Screen.width;
        }


        public void Update()
        {
            if (dragging)
            {
                if (TranslateInY)
                {
                    var delta = GetMousePos() - _startdragpos;
                    var newpos = _startsize.y + delta;
                    if (newpos < 0)
                        OpenWindow(false);
                    else
                        _transform.sizeDelta = new Vector2(_transform.sizeDelta.x, newpos);
                }
                else
                {
                    if (Input.mousePosition.x < Screen.width - 30)
                    {
                        var delta = GetMousePos() - _startdragpos;
                        var newpos = _startsize.x + delta;
                        if (newpos < 0)
                            OpenWindow(false);
                        else
                            _transform.sizeDelta = new Vector2(newpos, _transform.sizeDelta.y);
                    }
                }

                if (_updatelayoutgroup != null)
                    LayoutRebuilder.ForceRebuildLayoutImmediate(_updatelayoutgroup);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            dragging = true;
            _startsize = _transform.sizeDelta;
            PointerDown(GetMousePos());
            _startdragpos = GetMousePos();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            dragging = false;
            PointerUp(GetMousePos());
        }
    }
}