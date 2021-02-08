
using RuntimeInspectorNamespace;
using UnityEngine;
using UnityEngine.UI;

namespace game4automation
{
  
public class HierarchyDisplay : MonoBehaviour
{
    public Text ValueText;

    public Color ColorInput;
    public Color ColorOutput;
    public Color ColorNotConnected;
    
    private HierarchyItemTransform _hierarchyitem;
    private Signal _signal; 
    
    
    // Start is called before the first frame update
    void Start()
    {
        _hierarchyitem = GetComponent<HierarchyItemTransform>();
        ValueText.color = ColorNotConnected;
        ValueText.fontSize = _hierarchyitem.nameText.fontSize;
        _signal = _hierarchyitem.BoundTransform.GetComponent<Signal>();
        ValueText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (_signal != null)
        {
            if (_signal.IsInput())
            {
                ValueText.color = ColorInput;
            }
            else
            {
                ValueText.color = ColorOutput;
            }

            if (_signal.GetStatusConnected() == false)
            {
                ValueText.color = ColorNotConnected;
            }

            ValueText.text = _signal.GetVisuText();
        }
    }
}
}
