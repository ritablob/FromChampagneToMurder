using UnityEngine;
using Live2D.Cubism.Framework.Motion;

public class CubismMotionLoopPlayer : MonoBehaviour
{
    // AnimationClip to be played in a loop
    [SerializeField]
    public AnimationClip Animation;

    private CubismMotionController _motionController;


    private void Start()
    {
        _motionController = GetComponent<CubismMotionController>();


        // Loop playback of motions
        _motionController.PlayAnimation(Animation);
    }
}