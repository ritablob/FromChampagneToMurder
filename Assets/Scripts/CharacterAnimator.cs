using Live2D.Cubism.Framework.Motion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class AnimatorTriggerAttribute : PropertyAttribute
{
    public string TriggerName { get; private set; }

    public AnimatorTriggerAttribute(string triggerName)
    {
        TriggerName = triggerName;
    }
}

[System.Serializable]
public class Emotion_Triggers
{
    public Emotion enumValue;

    [AnimatorTrigger("Trigger1")]
    public string emotion_trigger;

    [AnimatorTrigger("Trigger2")]
    public string start_talking_trigger;

    [AnimatorTrigger("Trigger3")]
    public string finish_talking_trigger;
}

public enum Emotion
{
    Null, Normal, Angry, Bored, Doubtful, Excited, Unhappy, Flirtatious, Happy
}

public class CharacterAnimator : MonoBehaviour
{
    public Emotion_Triggers[] TriggerArray;

    public Animator animator;

    [Header("Debug")]
    [SerializeField] bool disable_debug_messages = false;
    [SerializeField] bool disable_warning_messages = false;
    [SerializeField] bool disable_test_animation_keys = false;

    [Header("Debug Variables (Changing them wont do anything)")]
    public Emotion CurrentEmotion = Emotion.Normal;
    public bool CurrentlyTalking;

    private void Start()
    {

    }

    //private void Update()
    //{
    //    if (!disable_test_animation_keys) UpdateAnim();
    //}

    ////Plays Motions specific to the different animation layers. Each layer can only play one animation. 
    ////If one layer overwrites parameters of another layer, the motion with the higher priority takes over
    //public void PlayMotion(AnimationClip animation, ANIMATIONLAYER animation_layer, bool looping = false, int priority = CubismMotionPriority.PriorityNormal)
    //{
    //    Debug.Log("Play animation on Layer " + animation_layer);
    //    if ((_motionController == null) || (animation == null))
    //    {
    //        Debug.LogWarning("Animation could not be played, no Motion Controller or Animation " + _motionController + " "+ animation);
    //        return;
    //    }
    //    _motionController.PlayAnimation(animation, isLoop: false, layerIndex: (int)animation_layer); //Enum os changed to int so PlayAnimation can be called
    //}

    //Stops Animations, animations take one frame to become stopped
    //public void StopMotion(ANIMATIONLAYER animation_layer) 
    //{
    //    Debug.Log("Stop Motion on Layer" + animation_layer);
    //    _motionController.StopAnimation(1, (int)animation_layer); //I litarally have no fucking clue why there is an animation index (that 3) the function doesnt even use it
    //}

    //private void UpdateAnim()
    //{
    //    //Debug Input for testing
    //    //Keep pressed
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        StartTalking();
    //    }
    //    if (Input.GetKeyUp(KeyCode.Space))
    //    {
    //        FinishTalking();
    //    }
    //    //Press
    //    if (Input.GetKeyDown(KeyCode.Q)) 
    //    {
    //        ChangeEmotion(Emotion.Normal);
    //    }
    //    if (Input.GetKeyDown(KeyCode.W))
    //    {
    //        ChangeEmotion(Emotion.Angry);
    //    }
    //    if (Input.GetKeyDown(KeyCode.E))
    //    {
    //        ChangeEmotion(Emotion.Bored);
    //    }
    //    if (Input.GetKeyDown(KeyCode.R))
    //    {
    //        ChangeEmotion(Emotion.Doubtful);
    //    }
    //    if (Input.GetKeyDown(KeyCode.T))
    //    {
    //        ChangeEmotion(Emotion.Excited);
    //    }
    //}

    public void StartTalking(Emotion emotion)
    {
        ChangeEmotion(emotion);
        StartMouthAnimation(emotion);
        CurrentlyTalking = true;
    }

    public void StartTalking(string emotion)
    {
        //Get Emotion from string
        Emotion new_emotion = Emotion.Null;
        switch (emotion)
        {
            case "Angry":
                new_emotion = Emotion.Angry; break;
            case "Bored":
                new_emotion = Emotion.Bored; break;
            case "Doubtful":
                new_emotion = Emotion.Doubtful; break;
            case "Excited":
                new_emotion = Emotion.Excited; break;
            case "Null":
                new_emotion = Emotion.Null; break;
            case "Normal":
                new_emotion = Emotion.Null; break;
            case "Unhappy":
                new_emotion = Emotion.Unhappy; break;
            case "Flirtatious":
                new_emotion = Emotion.Flirtatious; break;
            default:
                Debug.LogWarning("Emotion " + emotion + " not found, set default animation");
                new_emotion = Emotion.Normal; break;
        }
        StartTalking(new_emotion);
    }

    public void StartTalking()
    {
        StartMouthAnimation(CurrentEmotion);
    }

    public void FinishTalking()
    {
        StopMouthAnimation(CurrentEmotion);
        CurrentlyTalking = false;
    }

    public void ChangeEmotion(Emotion next_emotion)
    {
        if (CurrentEmotion == next_emotion)
        {
            if (!disable_debug_messages) Debug.Log("Skipped ChangeEmotion(), Emotion was the same ");
            return;
        }

        CurrentEmotion = next_emotion;
        PlayEmotion(next_emotion);
    }

    void PlayEmotion(Emotion emotion)
    {
        //switch (emotion)
        //{
        //    case Emotion.Null:
        //        animator.SetTrigger("Default");
        //        break;
        //    case Emotion.Normal:
        //        animator.SetTrigger("Default");
        //        break;
        //    case Emotion.Doubtful:
        //        animator.SetTrigger("Doubtful");
        //        break;
        //    case Emotion.Excited:
        //        animator.SetTrigger("Excited");
        //        break;
        //    case Emotion.Bored:
        //        animator.SetTrigger("Default");
        //        break;
        //    case Emotion.Angry:
        //        animator.SetTrigger("Angry");
        //        break;
        //    case Emotion.Unhappy:
        //        animator.SetTrigger("Default");
        //        break;
        //    case Emotion.Flirtatious:
        //        animator.SetTrigger("Doubtful");
        //        break;  
            
        //    default:
        //        if (!disable_warning_messages) Debug.LogWarning("Emotion not found: " + emotion);
        //        break;
        //}
        //animator.SetTrigger(trigger);

        foreach (Emotion_Triggers emotion_Triggers in TriggerArray)
        {
            if (emotion_Triggers.enumValue == emotion)
            {
                animator.SetTrigger(emotion_Triggers.emotion_trigger);
            }
        }
    }

    void StartMouthAnimation(Emotion emotion)
    {
        //AnimationClip new_talking_animation = null;

        //switch (emotion)
        //{
        //    case Emotion.Null:
        //        if (!disable_warning_messages) Debug.LogWarning("Emotion was Null");
        //        break;

        //    case Emotion.Normal:
        //        new_talking_animation = happy_talking_animation;
        //        break;
        //    case Emotion.Doubtful:
        //        new_talking_animation = sad_talking_animation;
        //        break;
        //    case Emotion.Excited:
        //        new_talking_animation = happy_talking_animation;
        //        break;
        //    case Emotion.Bored:
        //        new_talking_animation = sad_talking_animation;
        //        break;
        //    case Emotion.Angry:
        //        new_talking_animation = sad_talking_animation;
        //        break;
        //    case Emotion.Unhappy:
        //        new_talking_animation = sad_talking_animation;
        //        break;
        //    case Emotion.Flirtatious:
        //        new_talking_animation = happy_talking_animation;
        //        break;

        //    default:
        //        if (!disable_warning_messages) Debug.LogWarning("Emotion not found: " + emotion);
        //        break;
        //}

        //PlayMotion(sad_talking_animation, ANIMATIONLAYER.MOUTH_TALKING, true);
        //StopMotion(ANIMATIONLAYER.MOUTH_IDLE);
        
        foreach (Emotion_Triggers emotion_Triggers in TriggerArray)
        {
            if (emotion_Triggers.enumValue == emotion)
            {
                animator.SetTrigger(emotion_Triggers.start_talking_trigger);
            }
        }
    }

    void StopMouthAnimation(Emotion emotion)
    {
        //AnimationClip new_mouth_animation = null;

        //switch (emotion)
        //{
        //    case Emotion.Null:
        //        if (!disable_warning_messages) Debug.LogWarning("Emotion was Null");
        //        break;
        //    case Emotion.Normal:
        //        new_mouth_animation = smile_mouth_animation;
        //        break;
        //    case Emotion.Doubtful:
        //        new_mouth_animation = unhappy_mouth_animation;
        //        break;
        //    case Emotion.Excited:
        //        new_mouth_animation = smile_mouth_animation;
        //        break;
        //    case Emotion.Bored:
        //        new_mouth_animation = unhappy_mouth_animation;
        //        break;
        //    case Emotion.Angry:
        //        new_mouth_animation = unhappy_mouth_animation;
        //        break;
        //    case Emotion.Unhappy:
        //        new_mouth_animation = unhappy_mouth_animation;
        //        break;
        //    case Emotion.Flirtatious:
        //        new_mouth_animation = happy_talking_animation;
        //        break;

        //    default:
        //        if (!disable_warning_messages) Debug.LogWarning("Emotion not found: " + emotion);
        //        break;
        //}

        //PlayMotion(new_mouth_animation, ANIMATIONLAYER.MOUTH_IDLE, true);
        //StopMotion(ANIMATIONLAYER.MOUTH_TALKING);

        foreach (Emotion_Triggers emotion_Triggers in TriggerArray)
        {
            if (emotion_Triggers.enumValue == emotion)
            {
                animator.SetTrigger(emotion_Triggers.finish_talking_trigger);
            }
        }
    }

}
