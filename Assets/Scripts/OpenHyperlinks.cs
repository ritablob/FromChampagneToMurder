using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

// somewhat based upon the TextMesh Pro example script: TMP_TextSelector_B
[RequireComponent(typeof(TextMeshProUGUI))]
public class OpenHyperlinks : MonoBehaviour
{
    public Color normalHyperlinkColor; // hex code for hyperlinks :)
    public bool doesColorChangeOnHover = true;
    public Color hoverColor = new Color(60f / 255f, 120f / 255f, 1f);
    private string _linkStartReplace = "<u color=#FF44FFFF><color=#FF44FFFF>";
    private string _linkEndReplace = "</u></color>";
    private TMP_Text m_TextComponent;

    private TextMeshProUGUI pTextMeshPro;
    public Canvas pCanvas;
    private Camera pCamera;

    private List<int> _linkHashes;
    private bool hasFormatted;
    public bool isLinkHighlighted { get { return pCurrentLink != -1; } }

    private int pCurrentLink = -1;
    private List<Color32[]> pOriginalVertexColors = new List<Color32[]>();

    protected virtual void Awake()
    {
        m_TextComponent = GetComponent<TMP_Text>();
        pTextMeshPro = GetComponent<TextMeshProUGUI>();
        pCanvas = GetComponentInParent<Canvas>();
        _linkHashes = new List<int>();
        // Get a reference to the camera if Canvas Render Mode is not ScreenSpace Overlay.
        if (pCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            pCamera = null;
        else
            pCamera = pCanvas.worldCamera;
    }
    private void OnEnable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ON_TEXT_CHANGED);
    }
    private void OnDisable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(ON_TEXT_CHANGED);
    }
    private void Start()
    {
        if (normalHyperlinkColor != null)
        {
            string colourHex = ColorUtility.ToHtmlStringRGB(normalHyperlinkColor);
            _linkStartReplace = "<u color=#" + colourHex+ "><color=#" + colourHex + ">";
        }
        Debug.Log(_linkStartReplace);
    }

    void ON_TEXT_CHANGED(UnityEngine.Object obj) // do all the things once the text in the textbox is changed
    {
        if (obj == m_TextComponent)
        {
            FormatLinks();
        }
    }

    private void FormatLinks()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(ON_TEXT_CHANGED);
        hasFormatted = false;
        if (!hasFormatted)
        {
            //We go backwards thrrough the list so we only have to get the indexes once
            TMP_LinkInfo[] links = pTextMeshPro.textInfo.linkInfo.Reverse().ToArray();
            Debug.Log(links.Count() + " links, ");
            pTextMeshPro.text = pTextMeshPro.text.Insert(links[0].linkTextfirstCharacterIndex + links[0].linkTextLength + links[0].linkIdLength + 7, _linkEndReplace);
            //Place before <link
            pTextMeshPro.text = pTextMeshPro.text.Insert(links[0].linkTextfirstCharacterIndex, _linkStartReplace);
            /*for (int i = 0; i < links.Count(); i++)
            {
                //Append behind </link>
                pTextMeshPro.text = pTextMeshPro.text.Insert(links[i].linkTextfirstCharacterIndex + links[i].linkTextLength + links[i].linkIdLength + 7, _linkEndReplace);
                //Place before <link
                pTextMeshPro.text = pTextMeshPro.text.Insert(links[i].linkTextfirstCharacterIndex, _linkStartReplace);
            }*/
            hasFormatted = true;
        }
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ON_TEXT_CHANGED);
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

        // Debug.Log(string.Format("isHovering: {0}, link: {1}", isHoveringOver, linkIndex));
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
}