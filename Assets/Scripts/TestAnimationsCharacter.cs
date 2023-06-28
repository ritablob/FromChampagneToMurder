using Live2D.Cubism.Framework.Motion;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
public class TestAnimationsCharacter : MonoBehaviour
{
    CubismMotionController _motionController;
    public AnimationClip blinking_animation;              //Animation Layer: 1
    public AnimationClip body_bounce_animation;           //Animation Layer: 2
    public AnimationClip excited_animation;               //Animation Layer: 5
    public AnimationClip smile_animation;                 //Animation Layer: 3
    public AnimationClip happy_talking_animation;         //Animation Layer: 4
    public AnimationClip sad_talking_animation;           //Animation Layer: 4

    public bool currently_talking; //No logic until now, change to true when currently text appears, change to false when text has finished appearing
    public bool eager_to_talk = true; //No logic


    private void Start()
    {
        _motionController = GetComponent<CubismMotionController>();
        //Start the Starting Animations
        PlayMotion(blinking_animation, 1, true); //Blinking
        PlayMotion(body_bounce_animation, 2, true); //Body Bounce
        PlayMotion(smile_animation, 3, true, priority: CubismMotionPriority.PriorityIdle); //Idle so it can be overwritten by Layer 4 (Talking)
    }

    private void Update()
    {
        UpdateAnim();
    }

    //Plays Motions specific to the different animation layers. Each layer can only play one animation. 
    //If one layer overwrites parameters of another layer, the motion with the higher priority takes over
    public void PlayMotion(AnimationClip animation, int animation_layer, bool looping = false, int priority = CubismMotionPriority.PriorityNormal)
    {
        if ((_motionController == null) || (animation == null))
        {
            Debug.LogWarning("Animation could not be played, no Motion Controller or Animation");
            return;
        }
        _motionController.PlayAnimation(animation, isLoop: looping, layerIndex: animation_layer);
    }

    //Stops Animations, animations take one frame to become stopped
    public void StopMotion(int animation_layer) 
    {
        _motionController.StopAnimation(1, animation_layer); //I litarally have no fucking clue why there is an animation index (that 3) the function doesnt even use it
    }

    private void UpdateAnim()
    {
        //Debug Input for testing
        //Keep pressed
        if (Input.GetKeyDown(KeyCode.W))
        {
            PlayMotion(happy_talking_animation, 4, true);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            PlayMotion(sad_talking_animation, 4, true);
        }
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.E))
        {
            StopMotion(4);
        }
        //Toggle
        if (Input.GetKeyDown(KeyCode.R)) 
        {
            eager_to_talk = !eager_to_talk;
            if (eager_to_talk)
            {
                PlayMotion(excited_animation, 5, true); 
            }
            else
            {
                StopMotion(5);
            }
            
        }
    }
}