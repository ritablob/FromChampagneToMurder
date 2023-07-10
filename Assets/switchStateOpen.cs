using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class switchStateOpen : MonoBehaviour
{
    public Animator anim;

    public void switchState()
    {
        if(anim.GetBool("Open")==true)
        {
            anim.SetBool("Open",false);
            
        }
        else
        {
            anim.SetBool("Open", true);
        }
    }
    public void switchStateHover()
    {
        if (anim.GetBool("Hover") == true)
        {
            anim.SetBool("Hover", false);

        }
        else
        {
            anim.SetBool("Hover", true);
        }
    }
}
