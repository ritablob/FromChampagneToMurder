using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Character", order = 1)]
public class Character : ScriptableObject
{
    public string characterName;
    public TextAsset jsonFile;
    [Tooltip("Index of the character's level scene in the Build Settings.")]
    public int sceneIndex;
}
