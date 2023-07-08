using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TerrainUtils;
using UnityEngine.UI;

public class IntroOverlayBehaviour : MonoBehaviour
{
    /* appear on start
     * click anywhere to minimize it -> moves down
     * click button to reappear it -> moves up again
     * fade in / out panel
     */

    public GameObject letter;
    public Image panel;
    public float movingSpeed = 1f;
    public float panelFadingSpeed = 0.1f;
    public float finalPositionY = -1080f;

    private bool showingLetter;


    // Start is called before the first frame update
    void Start()
    {
        if (letter.activeSelf)
        {
            showingLetter = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (showingLetter && Input.GetKey(KeyCode.Mouse0))
        {
            HideLetter();
            HidePanel();
        }
    }
    void HideLetter()
    {

        StartCoroutine(LetterMove(false));

    }
    public void ShowLetter()
    {
        Debug.Log("show letter");
        letter.SetActive(true);
        StartCoroutine(LetterMove(true));
    }

    void HidePanel()
    {
        StartCoroutine(PanelFade(false));
    }
    public void ShowPanel()
    {
        panel.gameObject.SetActive(true);
        Debug.Log("show panel");
    }
    IEnumerator LetterMove(bool moveUp)
    {
        if (moveUp)
        {

            if (Math.Round(letter.transform.position.y, 2) < 0)
            {
                float posY = letter.transform.position.y + (movingSpeed * Time.deltaTime);
                letter.transform.position = new(letter.transform.position.x, posY);
                yield return new WaitForSeconds(1f);
                StartCoroutine(LetterMove(moveUp));
            }
            showingLetter = true;
            yield return null;
        }
        else
        {
            if (Math.Round(letter.transform.position.y, 2) > finalPositionY)
            {
                float posY = letter.transform.position.y - (movingSpeed * Time.deltaTime);
                letter.transform.position = new(letter.transform.position.x, posY);
                yield return new WaitForSeconds(1f);
                StartCoroutine(LetterMove(moveUp));
            }
            showingLetter = false;
            letter.SetActive(false);
            yield return null;
        }
    }
    IEnumerator PanelFade(bool fadeIn)
    {
        if (fadeIn)
        {
            if (panel.color.a < 1)
            {
                Color newColor = panel.color;
                newColor.a += panelFadingSpeed;
                panel.color = newColor;
                yield return new WaitForSeconds(1f);
                StartCoroutine(PanelFade(true));
            }
            yield return null;
        }
        else
        {
            if (panel.color.a > 0)
            {
                Color newColor = panel.color;
                newColor.a -= panelFadingSpeed;
                panel.color = newColor;
                yield return new WaitForSeconds(1f);
                StartCoroutine(PanelFade(true));
            }
            panel.gameObject.SetActive(false);
            yield return null;
        }
    }
}