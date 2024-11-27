using UnityEngine;

[CreateAssetMenu(fileName = "New Merge Object", menuName = "Match and Merge/Object Template")]
public class ObjectTemplate : ScriptableObject
{
    public string uniqueID;
    public GameObject[] progressionObjectPrefabs;
    public Sprite[] uiTextures;

    public GameObject NextProgressionObject(int index)
    {
        return progressionObjectPrefabs[index];
    }
}
