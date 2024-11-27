using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestObject : MonoBehaviour
{
    public string uniqueID;
    public int index;
    public Button claimButton;
    MergeObjectComponent questObject;
    bool isCompleted = false;

    public delegate void HandleQuestClaimed(MergeObjectComponent v_component);
    public static HandleQuestClaimed OnQuestClaimed;

    private void OnEnable()
    {
        GameManager.OnNewObjectCreated += CompareQuestToNewObject;
    }

    private void OnDisable()
    {
        GameManager.OnNewObjectCreated -= CompareQuestToNewObject;
    }

    void CompareQuestToNewObject(MergeObjectComponent v_component)
    {
        if (uniqueID == v_component.GetObjectID())
        {
            if (index == v_component.GetObjectProgressionIndex())
            {
                SoundManager.Instance.PlayQuestCompletedSound();
                questObject = v_component;
                claimButton.interactable = true;
            }
        }
    }

    public void CompleteQuest()
    {
        isCompleted = true;
        claimButton.interactable = false;
        OnQuestClaimed?.Invoke(questObject);
        SoundManager.Instance.PlayQuestClaimedSound();
        StartCoroutine(TurnOffDelayed());
    }

    public bool IsQuestCompleted() => isCompleted;

    IEnumerator TurnOffDelayed()
    {
        yield return new WaitForSeconds(2.5f);
        gameObject.SetActive(false);
    }
}
