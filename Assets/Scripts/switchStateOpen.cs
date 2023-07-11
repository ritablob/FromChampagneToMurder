using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class switchStateOpen : MonoBehaviour
{
    public Animator anim;
    public bool Hover;
    public bool Open;
    public bool active;

    public void switchState()
    {
        if(anim.GetBool("Open")==true && active == true)
        {
            anim.SetBool("Open",false);
            Open = false;
            
        }
        else if(active == true)
        {
            anim.SetBool("Open", true);
            Open = true;
        }
    }
    public void switchStateHover()
    {
        if (anim.GetBool("Hover") == true && active == true)
        {
            anim.SetBool("Hover", false);
            Hover = false;

        }
        else if(active == true)
        {
            anim.SetBool("Hover", true);
            Hover = true;
        }
    }
}
