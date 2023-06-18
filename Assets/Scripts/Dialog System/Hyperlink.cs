using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;


public class Hyperlink : MonoBehaviour
{
    public bool doesColorChangeOnHover = true;
    public Color hoverColor = new Color(60f / 255f, 120f / 255f, 1f);


    private int pCurrentLink = -1;
    public bool IsLinkHighlighted { get { return pCurrentLink != -1; } }
    public Canvas pCanvas;

    private TextMeshProUGUI pTextMeshPro;
    private Camera pCamera;

    private List<Color32[]> pOriginalVertexColors = new List<Color32[]>();
    private TMP_Text m_TextComponent;
    private const int INVALID_LINK_INDEX = -1;
    int selectedLinkIndex; // index of the hyperlink you just clicked on (from 0 to infinity, each hyperlink
                           // in the text has the next int number index)
    protected virtual void Awake()
    {
        m_TextComponent = GetComponent<TMP_Text>();
        pTextMeshPro = GetComponent<TextMeshProUGUI>();
        pCanvas = GetComponentInParent<Canvas>();

        // Get a reference to the camera if Canvas Render Mode is not ScreenSpace Overlay.
        if (pCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            pCamera = null;
        else
            pCamera = pCanvas.worldCamera;
    }


    void LateUpdate()
    {

        // is the cursor in the correct region (above the text area) and furthermore, in the link region?
        var isHoveringOver = TMP_TextUtilities.IsIntersectingRectTransform(pTextMeshPro.rectTransform, Input.mousePosition, pCamera);
        int linkIndex = isHoveringOver ? TMP_TextUtilities.FindIntersectingLink(pTextMeshPro, Input.mousePosition, pCamera)
            : -1;

        // Clear previous link selection if one existed.
        if (pCurrentLink != -1 && linkIndex != pCurrentLink)
        {
            // Debug.Log("Clear old selection");
            SetLinkToColor(pCurrentLink, (linkIdx, vertIdx) => pOriginalVertexColors[linkIdx][vertIdx]);
            pOriginalVertexColors.Clear();
            pCurrentLink = -1;
        }

        // Handle new link selection.
        if (linkIndex != -1 && linkIndex != pCurrentLink)
        {
            // Debug.Log("New selection");
            pCurrentLink = linkIndex;
            if (doesColorChangeOnHover)
                pOriginalVertexColors = SetLinkToColor(linkIndex, (_linkIdx, _vertIdx) => hoverColor);
        }

        selectedLinkIndex = INVALID_LINK_INDEX;
        if (CheckForInteraction(out selectedLinkIndex))
        {
            // we send the selected link index back to fungus
           // fungusVarRef.Set(selectedLinkIndex);
            Debug.Log("Link Clicked, link ID = " + selectedLinkIndex);
        }
    }


    List<Color32[]> SetLinkToColor(int linkIndex, Func<int, int, Color32> colorForLinkAndVert)
    {
        TMP_LinkInfo linkInfo = pTextMeshPro.textInfo.linkInfo[linkIndex];

        var oldVertColors = new List<Color32[]>(); // store the old character colors

        for (int i = 0; i < linkInfo.linkTextLength; i++)
        { // for each character in the link string
            int characterIndex = linkInfo.linkTextfirstCharacterIndex + i; // the character index into the entire text
            var charInfo = pTextMeshPro.textInfo.characterInfo[characterIndex];
            int meshIndex = charInfo.materialReferenceIndex; // Get the index of the material / sub text object used by this character.
            int vertexIndex = charInfo.vertexIndex; // Get the index of the first vertex of this character.

            Color32[] vertexColors = pTextMeshPro.textInfo.meshInfo[meshIndex].colors32; // the colors for this character
            oldVertColors.Add(vertexColors.ToArray());

            if (charInfo.isVisible)
            {
                vertexColors[vertexIndex + 0] = colorForLinkAndVert(i, vertexIndex + 0);
                vertexColors[vertexIndex + 1] = colorForLinkAndVert(i, vertexIndex + 1);
                vertexColors[vertexIndex + 2] = colorForLinkAndVert(i, vertexIndex + 2);
                vertexColors[vertexIndex + 3] = colorForLinkAndVert(i, vertexIndex + 3);
            }
        }

        // Update Geometry
        pTextMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

        return oldVertColors;
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
        return linkIndex > INVALID_LINK_INDEX;
    }

    private bool CheckForInput(out Vector2 inputPosition)
    {
        inputPosition = Vector2.zero;

        if (!Input.GetMouseButtonUp(0)) return false;
        inputPosition = Input.mousePosition;

        return TMP_TextUtilities.IsIntersectingRectTransform(m_TextComponent.rectTransform, inputPosition, null);
    }
}
