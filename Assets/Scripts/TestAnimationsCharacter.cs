using Live2D.Cubism.Framework.Motion;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using static TestAnimationsCharacter;

public class TestAnimationsCharacter : MonoBehaviour
{
    public enum ANIMATIONLAYER
    {
        BLINKING = 1,
        BODY_BOUNCE = 2,
        MOUTH_IDLE = 3,
        MOUTH_TALKING = 4,
        EMOTION = 5,
        EMOTION_SECONDARY = 6
    }

    public enum Emotion { Normal, Angry, Bored, Doubtful, Excited, Null }

    CubismMotionController _motionController;

    [TextArea]
    [Tooltip("USe the Keys Q (Reset) W E R T to switch between the Animations")]
    public string Summary = "This component shouldn't be removed, it does important stuff.";

    [Header("Debug")]
    [SerializeField] bool disable_debug_messages = false;
    [SerializeField] bool disable_warning_messages = false;
    [SerializeField] bool disable_test_animation_keys = false;

    [Header("Debug Variables (Changing them wont do anything)")]
    public Emotion CurrentEmotion = Emotion.Normal;
    public bool CurrentlyTalking; //No logic until now, change to true when currently text appears, change to false when text has finished appearing

    [Header("Animations - General - Layer 1 & 2")]
    [SerializeField] AnimationClip blinking_animation;              //Animation Layer: 1
    [SerializeField] AnimationClip body_bounce_animation;           //Animation Layer: 2
    [Header("Animations - Mouths- Layer 3")]
    [SerializeField] AnimationClip smile_mouth_animation;           //Animation Layer: 3
    [SerializeField] AnimationClip unhappy_mouth_animation;
    [Header("Animations - Talking- Layer 4")]
    [SerializeField] AnimationClip happy_talking_animation;         //Animation Layer: 4
    [SerializeField] AnimationClip sad_talking_animation;           //Animation Layer: 4
    [Header("Animations - Emotions- Layer 5")]
    [SerializeField][Tooltip("Plays on W")] AnimationClip angry_animation;
    [SerializeField][Tooltip("Plays on E")] AnimationClip bored_animation;
    [SerializeField][Tooltip("Plays on R")] AnimationClip doubtful_animation;
    [SerializeField][Tooltip("Plays on T")] AnimationClip excited_animation;               //Animation Layer: 5

    bool emotion_was_changed = false;

    private bool use_secondary_emotion_layer = false;

    private void Start()
    {
        _motionController = GetComponent<CubismMotionController>();

        //Start the Starting Animations
        PlayMotion(blinking_animation, ANIMATIONLAYER.BLINKING, true); //Blinking
        PlayMotion(body_bounce_animation, ANIMATIONLAYER.BODY_BOUNCE, true); //Body Bounce
        PlayMotion(smile_mouth_animation, ANIMATIONLAYER.MOUTH_IDLE, true, priority: CubismMotionPriority.PriorityIdle); //Idle so it can be overwritten by Layer 4 (Talking)
    }

    private void Update()
    {
        //if (emotion_was_changed) //Continues with the emotion change form the last frame
        //{
        //    PlayEmotion(CurrentEmotion);
        //    emotion_was_changed = false;
        //}
        if (!disable_test_animation_keys) UpdateAnim();
    }

    //Plays Motions specific to the different animation layers. Each layer can only play one animation. 
    //If one layer overwrites parameters of another layer, the motion with the higher priority takes over
    public void PlayMotion(AnimationClip animation, ANIMATIONLAYER animation_layer, bool looping = false, int priority = CubismMotionPriority.PriorityNormal)
    {
        Debug.Log("Play animation on Layer " + animation_layer);
        if ((_motionController == null) || (animation == null))
        {
            Debug.LogWarning("Animation could not be played, no Motion Controller or Animation " + _motionController + " "+ animation);
            return;
        }
        _motionController.PlayAnimation(animation, isLoop: looping, layerIndex: (int)animation_layer); //Enum os changed to int so PlayAnimation can be called
    }

    //Stops Animations, animations take one frame to become stopped
    public void StopMotion(ANIMATIONLAYER animation_layer) 
    {
        Debug.Log("Stop Motion on Layer" + animation_layer);
        _motionController.StopAnimation(1, (int)animation_layer); //I litarally have no fucking clue why there is an animation index (that 3) the function doesnt even use it
    }

    private void UpdateAnim()
    {
        //Debug Input for testing
        //Keep pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartTalking();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            FinishTalking();
        }
        //Press
        if (Input.GetKeyDown(KeyCode.Q)) 
        {
            ChangeEmotion(Emotion.Normal);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            ChangeEmotion(Emotion.Angry);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            ChangeEmotion(Emotion.Bored);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ChangeEmotion(Emotion.Doubtful);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            ChangeEmotion(Emotion.Excited);
        }
    }

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
        Debug.Log("Change to Emotion:" + next_emotion);
        if (CurrentEmotion == next_emotion)
        {
            if (!disable_debug_messages) Debug.Log("Skipped ChangeEmotion(), Emotion was the same ");
            return;
        }

        CurrentEmotion = next_emotion;
        //emotion_was_changed = true;
        PlayEmotion(next_emotion); //Stops Emotion from being played
        //PlayEmotion(CurrentEmotion); //Moved to the following frames Update
    }

    void PlayEmotion(Emotion emotion)
    {
        ANIMATIONLAYER animation_layer = ANIMATIONLAYER.EMOTION;
        ANIMATIONLAYER secondary_animation_layer = ANIMATIONLAYER.EMOTION_SECONDARY;
        if (use_secondary_emotion_layer) 
        {
            animation_layer = ANIMATIONLAYER.EMOTION_SECONDARY;
            secondary_animation_layer = ANIMATIONLAYER.EMOTION;
        }
        use_secondary_emotion_layer = !use_secondary_emotion_layer;

        switch (emotion)
        {
            case Emotion.Null:
                if (!disable_warning_messages) Debug.LogWarning("Emotion was Null");
                break;
            case Emotion.Normal: 
                StopMotion(animation_layer); 
                break;
            case Emotion.Doubtful:
                PlayMotion(doubtful_animation, animation_layer, true);
                break;
            case Emotion.Excited:
                PlayMotion(excited_animation, animation_layer, true);
                break;
            case Emotion.Bored:
                PlayMotion(bored_animation, animation_layer, true);
                break;
            case Emotion.Angry:
                PlayMotion(angry_animation, animation_layer, true);
                break;  
            
            default:
                if (!disable_warning_messages) Debug.LogWarning("Emotion not found: " + emotion);
                break;
        }

        StopMotion(secondary_animation_layer);
    }

    void StartMouthAnimation(Emotion emotion)
    {
        switch (emotion)
        {
            case Emotion.Null:
                if (!disable_warning_messages) Debug.LogWarning("Emotion was Null");
                break;

            case Emotion.Normal:
                PlayMotion(happy_talking_animation, ANIMATIONLAYER.MOUTH_TALKING, true);
                StopMotion(ANIMATIONLAYER.MOUTH_IDLE);
                break;
            case Emotion.Doubtful:
                PlayMotion(sad_talking_animation, ANIMATIONLAYER.MOUTH_TALKING, true);
                StopMotion(ANIMATIONLAYER.MOUTH_IDLE);
                break;
            case Emotion.Excited:
                PlayMotion(happy_talking_animation, ANIMATIONLAYER.MOUTH_TALKING, true);
                StopMotion(ANIMATIONLAYER.MOUTH_IDLE);
                break;
            case Emotion.Bored:
                PlayMotion(sad_talking_animation, ANIMATIONLAYER.MOUTH_TALKING, true);
                StopMotion(ANIMATIONLAYER.MOUTH_IDLE);
                break;
            case Emotion.Angry:
                PlayMotion(sad_talking_animation, ANIMATIONLAYER.MOUTH_TALKING, true);
                StopMotion(ANIMATIONLAYER.MOUTH_IDLE);
                break;
            
            default:
                if (!disable_warning_messages) Debug.LogWarning("Emotion not found: " + emotion);
                break;
        }
    }

    void StopMouthAnimation(Emotion emotion)
    {
        switch (emotion)
        {
            case Emotion.Null:
                if (!disable_warning_messages) Debug.LogWarning("Emotion was Null");
                break;

            case Emotion.Normal:
                PlayMotion(smile_mouth_animation, ANIMATIONLAYER.MOUTH_IDLE, true);
                StopMotion(ANIMATIONLAYER.MOUTH_TALKING);
                break;
            case Emotion.Doubtful:
                PlayMotion(unhappy_mouth_animation, ANIMATIONLAYER.MOUTH_IDLE, true);
                StopMotion(ANIMATIONLAYER.MOUTH_TALKING);
                break;
            case Emotion.Excited:
                PlayMotion(smile_mouth_animation, ANIMATIONLAYER.MOUTH_IDLE, true);
                StopMotion(ANIMATIONLAYER.MOUTH_TALKING);
                break;
            case Emotion.Bored:
                PlayMotion(unhappy_mouth_animation, ANIMATIONLAYER.MOUTH_IDLE, true);
                StopMotion(ANIMATIONLAYER.MOUTH_TALKING);
                break;
            case Emotion.Angry:
                PlayMotion(unhappy_mouth_animation, ANIMATIONLAYER.MOUTH_IDLE, true);
                StopMotion(ANIMATIONLAYER.MOUTH_TALKING);
                break;
            
            default:
                if (!disable_warning_messages) Debug.LogWarning("Emotion not found: " + emotion);
                break;
        }
    }

}