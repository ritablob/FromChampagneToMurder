using UnityEngine;
using Live2D.Cubism.Framework.Motion;

public class CubismMotionLoopPlayer : MonoBehaviour
{
    // ループ再生させるAnimationClip
    [SerializeField]
    public AnimationClip Animation;

    private CubismMotionController _motionController;


    private void Start()
    {
        _motionController = GetComponent<CubismMotionController>();


        // Register a callback function in the handler of CubismMotionController
        _motionController.AnimationEndHandler += OnAnimationEnded;


        // Let the motion play directly only the first time
        OnAnimationEnded(0);
    }


    // Callback when the motion has finished playing
    private void OnAnimationEnded(float instanceId)
    {
        // Motion playback (loop playback disable
        _motionController.PlayAnimation(Animation, isLoop: false);
    }
}