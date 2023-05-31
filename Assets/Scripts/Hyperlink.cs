using UnityEngine;
using TMPro;
using Fungus;

public class Hyperlink : MonoBehaviour
{
    private TMP_Text m_TextComponent;
    private const int INVALID_LINK_INDEX = -1;
    int selectedLinkIndex; // index of the hyperlink you just clicked on (from 0 to infinity, each hyperlink
                           // in the text has the next int number index)
    public VariableReference fungusVarRef;
    private void Awake()
    {
        m_TextComponent = GetComponent<TMP_Text>();
    }

    private void LateUpdate()
    {
        selectedLinkIndex = INVALID_LINK_INDEX;
        if (CheckForInteraction(out selectedLinkIndex)) 
        {
            fungusVarRef.Set(selectedLinkIndex);
            // we can send the selected link index back to fungus through here
            Debug.Log("Link Clicked, link ID = " + selectedLinkIndex);
        }
    }

    private bool CheckForInteraction(out int linkIndex)
    {
        linkIndex = INVALID_LINK_INDEX;
        Vector2 inputPosition = Vector2.zero;

        if (!CheckForInput(out inputPosition))
        {
            return false;
        }

        linkIndex = TMP_TextUtilities.FindIntersectingLink(m_TextComponent, inputPosition, null); // this finds the index of the hyperlink
        return (linkIndex > INVALID_LINK_INDEX);
    }

    private bool CheckForInput(out Vector2 inputPosition)
    {
        inputPosition = Vector2.zero;

        if (!Input.GetMouseButtonUp(0)) return false;
        inputPosition = Input.mousePosition;

        return TMP_TextUtilities.IsIntersectingRectTransform(m_TextComponent.rectTransform, inputPosition, null);
    }
}
