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
    public GameManager gameManager;
    public GameObject letter;

    private Animator animator;
    private bool showingLetter;


    // Start is called before the first frame update
    void Start()
    {
        if (letter.activeSelf)
        {
            showingLetter = true;
        }
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (showingLetter && Input.GetKeyDown(KeyCode.Mouse0))
        {
            HideIntroPanel();
        }
    }
    public void ShowIntroPanel()
    {
        animator.SetBool("hasClickedButton", true);
        animator.SetBool("hasClickedScreen", false);
        Invoke(nameof(SetTextBoxInactive), 1f);
        showingLetter = true;
    }
    public void HideIntroPanel()
    {
        animator.SetBool("hasClickedButton", false);
        animator.SetBool("hasClickedScreen", true);
        Invoke(nameof(SetTextBoxActive), 0.5f);
        showingLetter = false;
        if (gameManager.GetComponent<TextWriter>())
        {
            Invoke(nameof(InvokeWriteText), 0.5f);
        }
    }
    void SetTextBoxInactive()
    {
        gameManager.writer.tmp.gameObject.SetActive(false);
    }
    void SetTextBoxActive()
    {
        gameManager.writer.tmp.gameObject.SetActive(true);
    }
    void InvokeWriteText()
    {
        gameManager.GetComponent<TextWriter>().WriteText();
    }
}