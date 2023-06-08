using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class FungusExtension : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// Hyperlink traversing has never been easier! Supports traversing to up to 5 blocks for now.
    /// <br>Make sure that the <b><u>amount and order</u> of hyperlinks in the text</b>  matches up with the order and amount of blocks attached.</br>
    /// </summary>
    /// <param name="selectionIndex"> Assign Flowchart.selectionIndex here</param>
    /// <param name="block1"></param>
    /// <param name="block2"></param>
    /// <param name="block3"></param>
    /// <param name="block4"></param>
    /// <param name="block5"></param>
    public void LinkInput(int selectionIndex, List<Block> blockList)
    {
        /* 
        Block[] blockList = new Block[5];
        if (block1 != null)
        {
            blockList[0] = block1;
            if (block2 != null)
            {
                blockList[1] = block2;  
                if (block3 != null)
                {
                    blockList[2] = block3;
                    if (block4 != null)
                    {
                        blockList[3] = block4;
                        if (block5 != null)
                        {
                            blockList[4] = block5;
                        }
                    }
                }
            }
        }*/

        StartCoroutine(WaitForLinkInput(selectionIndex, blockList));
    }
    IEnumerator WaitForLinkInput(int indexVar, List<Block> blockList)
    {
        for (int i = 0; i < blockList.Count; i++)
        {
            if (indexVar == i)
            {
                yield return blockList[i];
            }
        }
        yield return new WaitForEndOfFrame();
        StartCoroutine(WaitForLinkInput(indexVar, blockList));
    }
}
