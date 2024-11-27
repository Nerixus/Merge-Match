using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameInput gameInput;
    public SceneObjectsPositionReference sceneObjectsReference;
    public LevelInfo levelToLoad;
    public QuestObject[] levelQuests;

    [Header("GameFeel")]
    public float lerpSpeed_ObjFound;
    public float lerpSpeed_ObjMatch;
    public Vector3 offsetHolderPosition;
    public Vector3 compareRotation;
    public float arrangeListWait;
    public Animator forgeDoorAnimator;
    public AnimationClip forgeAnimation;
    public Quaternion finalRewardRotation;
    public float finalRewardPresentScale;

    [Header("Particles")]
    public GameObject failParticles;
    public GameObject successParticles;

    [Header("UI")]
    public GameObject winScreen;

    List<MergeObjectComponent> sceneObjects;
    MergeObjectComponent firstSelectedObject;
    MergeObjectComponent secondSelectedObject;

    private void Start()
    {
        InstantiateSetOfObjects();
    }

    private void OnEnable()
    {
        MergeObjectComponent.OnSelectedByUser += OnObjectSelectedByUser;
        QuestObject.OnQuestClaimed += CompleteQuestHandleItem;
    }

    private void OnDisable()
    {
        MergeObjectComponent.OnSelectedByUser -= OnObjectSelectedByUser;
        QuestObject.OnQuestClaimed -= CompleteQuestHandleItem;
    }
    void InstantiateSetOfObjects()
    {
        sceneObjects = new List<MergeObjectComponent>();
        float newScale = levelToLoad.objectScaleMultiplier;
        for (int i = 0; i < levelToLoad.levelObjectTemplates.Count; i++)
        {
            string uniqueID = levelToLoad.levelObjectTemplates[i].uniqueID;
            GameObject objToInstantiate = levelToLoad.levelObjectTemplates[i].progressionObjectPrefabs[0];
            for (int j = 0; j < Mathf.Pow(2, levelToLoad.levelObjectTemplates[i].progressionObjectPrefabs.Length - 1); j++)
            {
                GameObject current = Instantiate(objToInstantiate,
                    new Vector3(Random.Range(sceneObjectsReference.LeftSideLimit(), sceneObjectsReference.RightSideLimit()),
                    Random.Range(1f, 2f),
                    Random.Range(sceneObjectsReference.NearPlaneLimit(), sceneObjectsReference.FarPlaneLimit())),
                    objToInstantiate.transform.rotation, sceneObjectsReference.objectsHolder);
                MergeObjectComponent currentComponent = current.GetComponent<MergeObjectComponent>();
                currentComponent.SetObjectID(uniqueID);
                currentComponent.SetObjectProgressionIndex(0);
                sceneObjects.Add(currentComponent);
                current.transform.localScale = current.transform.localScale * newScale;
            }
        }
    }

    public delegate void HandleObjectSelected(string v_uniqueID);
    public static event HandleObjectSelected OnObjectSelected;

    public delegate void HandleMatchFound();
    public static HandleMatchFound OnMatchFound;
    void OnObjectSelectedByUser(MergeObjectComponent v_mergeComponent)
    {
        OnObjectSelected?.Invoke(v_mergeComponent.GetObjectID());
        if (firstSelectedObject == null)
        {
            firstSelectedObject = v_mergeComponent;
            StartCoroutine(LerpToFurnace(firstSelectedObject, sceneObjectsReference.objectOneCompareSpot.position));
        }
        else
        {
            if (firstSelectedObject != v_mergeComponent)
            {
                secondSelectedObject = v_mergeComponent;
                StartCoroutine(LerpToFurnace(secondSelectedObject, sceneObjectsReference.objectTwoCompareSpot.position, true));
            }
        }
    }

    void ResetSelections()
    {
        firstSelectedObject.Unselect();
        secondSelectedObject.Unselect();
        firstSelectedObject = null;
        secondSelectedObject = null;
    }

    IEnumerator LerpToFurnace(MergeObjectComponent v_mergeComponent, Vector3 v_targetPosition, bool shouldCompareObjs = false)
    {
        SoundManager.Instance.PlayRandomObjectMoveSound();
        Transform match3objTransform = v_mergeComponent.transform;
        v_mergeComponent.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        float _current = 0f;
        float _target = 1f;
        Vector3 originPos = match3objTransform.position;
        Quaternion originRotation = match3objTransform.rotation;
        Vector3 originScale = match3objTransform.localScale;
        gameInput.inputActive = false;
        while (_current != _target)
        {
            _current = Mathf.MoveTowards(_current, _target, lerpSpeed_ObjFound * Time.deltaTime);
            match3objTransform.position = Vector3.Lerp(originPos, v_targetPosition, _current);
            match3objTransform.rotation = Quaternion.Lerp(originRotation, Quaternion.Euler(compareRotation), _current);
            match3objTransform.localScale = Vector3.Lerp(originScale, v_mergeComponent.GetStartingScale(), _current);
            yield return null;
        }
        if (shouldCompareObjs)
        {
            
            if (IsMatchValid())
            {
                //We create a new Object
                Debug.Log("Cook new object");
                OnMatchFound?.Invoke();
                StartCoroutine(CreateNewObjectFromMerge(firstSelectedObject, secondSelectedObject));
            }
            else
            {
                //We reject the selected objects
                Debug.Log("Spit both objects");
                SpitObjects();
            }
            ResetSelections();
        }
        gameInput.inputActive = true;
        yield return null;
    }

    bool IsMatchValid()
    {
        if (secondSelectedObject.GetObjectID() == firstSelectedObject.GetObjectID())//compare first if they are the same ID
        {
            if (secondSelectedObject.GetObjectProgressionIndex() == firstSelectedObject.GetObjectProgressionIndex())
            {
                SoundManager.Instance.PlayMergeSound();
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    ObjectTemplate templateToUse;

    public delegate void HandleNewObjectCreated(MergeObjectComponent v_component);
    public static HandleNewObjectCreated OnNewObjectCreated;
    IEnumerator CreateNewObjectFromMerge(MergeObjectComponent obj1, MergeObjectComponent obj2)
    {
        float _current = 0f;
        float _target = 1f;
        Vector3 originPosOne = obj1.transform.position;
        Vector3 originPosTwo = obj2.transform.position;
        while (_current != _target)
        {
            _current = Mathf.MoveTowards(_current, _target, lerpSpeed_ObjMatch * Time.deltaTime);
            obj1.transform.position = Vector3.Lerp(originPosOne, sceneObjectsReference.furnaceDoorEntrance.position, _current);
            obj2.transform.position = Vector3.Lerp(originPosTwo, sceneObjectsReference.furnaceDoorEntrance.position, _current);
            yield return null;
        }
        _current = 0f;
        _target = 1f;
        originPosOne = obj1.transform.position;
        originPosTwo = obj2.transform.position;
        while (_current != _target)
        {
            _current = Mathf.MoveTowards(_current, _target, lerpSpeed_ObjMatch * Time.deltaTime);
            obj1.transform.position = Vector3.Lerp(originPosOne, sceneObjectsReference.mergeObjectsPosition.position, _current);
            obj2.transform.position = Vector3.Lerp(originPosTwo, sceneObjectsReference.mergeObjectsPosition.position, _current);
            yield return null;
        }

        //forgeDoorAnimator.SetBool("Open", false);
        //forgeDoorAnimator.SetBool("Close", true);
        foreach (ObjectTemplate OT in levelToLoad.levelObjectTemplates)
        {
            if (OT.uniqueID == obj1.GetObjectID())
            {
                templateToUse = OT;
                Debug.Log("Found Template");
            }
        }

        yield return new WaitForSeconds(0.9f);
        Instantiate(successParticles, sceneObjectsReference.mergedObjectSpawner.position, Quaternion.identity);
        yield return new WaitForSeconds(0.1f);

        if (obj1.GetObjectProgressionIndex() < templateToUse.progressionObjectPrefabs.Length - 1)
        {
            SoundManager.Instance.PlayNewObjectSound();
            GameObject current = Instantiate(templateToUse.progressionObjectPrefabs[obj1.GetObjectProgressionIndex() + 1],
                sceneObjectsReference.mergedObjectSpawner.position,
                Quaternion.identity,
                sceneObjectsReference.objectsHolder);
            MergeObjectComponent currentComponent = current.GetComponent<MergeObjectComponent>();
            
            currentComponent.SetObjectID(templateToUse.uniqueID);
            currentComponent.SetObjectProgressionIndex(obj1.GetObjectProgressionIndex() + 1);
            sceneObjects.Add(currentComponent);
            current.transform.localScale = current.transform.localScale * levelToLoad.objectScaleMultiplier;
            current.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-50, -300), Random.Range(50, 150), Random.Range(-100, 100)));
            OnNewObjectCreated?.Invoke(currentComponent);
        }
        //gameInput.inputActive = true;
        //forgeDoorAnimator.SetBool("Open", true);
        //forgeDoorAnimator.SetBool("Close", false);
        Destroy(obj1.gameObject);
        Destroy(obj2.gameObject);
    }

    void SpitObjects()
    {
        SoundManager.Instance.PlayMergeFailSound();
        Instantiate(failParticles, sceneObjectsReference.furnaceDoorEntrance.position, Quaternion.identity);
        firstSelectedObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        secondSelectedObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        firstSelectedObject.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-100, 100), Random.Range(100, 250), Random.Range(10, 200)));
        secondSelectedObject.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-100, 100), Random.Range(100, 250), Random.Range(10, 200)));
        //gameInput.inputActive = true;
    }

    void CompleteQuestHandleItem(MergeObjectComponent v_mergeComponent)
    {
        StartCoroutine(FinalRewardAnimation(v_mergeComponent.transform));
        ReviewGameStatus();
    }

    void ReviewGameStatus()
    {
        int questsCompleted = 0;
        foreach (QuestObject QO in levelQuests)
        {
            if (QO.IsQuestCompleted())
                questsCompleted++;
        }
        if (questsCompleted == levelQuests.Length)
        {
            winScreen.SetActive(true);
        }
    }

    IEnumerator FinalRewardAnimation(Transform v_tranformFinalReward)
    {
        v_tranformFinalReward.transform.rotation = Quaternion.identity;
        v_tranformFinalReward.gameObject.AddComponent<ConstantRotate>();
        v_tranformFinalReward.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        float _current = 0f;
        float _target = 1f;
        Vector3 originPos = v_tranformFinalReward.position;
        Quaternion originRotation = v_tranformFinalReward.rotation;
        Vector3 originScale = v_tranformFinalReward.localScale;
        while (_current != _target)
        {
            _current = Mathf.MoveTowards(_current, _target, lerpSpeed_ObjFound * Time.deltaTime);
            v_tranformFinalReward.position = Vector3.Lerp(originPos, sceneObjectsReference.rewardsPresentation.position, _current);
            v_tranformFinalReward.rotation = Quaternion.Lerp(originRotation, finalRewardRotation, _current);
            v_tranformFinalReward.localScale = Vector3.Lerp(originScale, originScale * finalRewardPresentScale, _current);
            yield return null;
        }
        _current = 0f;
        _target = 1f;
        originPos = v_tranformFinalReward.position;
        originRotation = v_tranformFinalReward.rotation;
        Vector3 newScale = v_tranformFinalReward.localScale;
        yield return new WaitForSeconds(1);
        while (_current != _target)
        {
            _current = Mathf.MoveTowards(_current, _target, lerpSpeed_ObjFound * Time.deltaTime);
            v_tranformFinalReward.position = Vector3.Lerp(originPos, sceneObjectsReference.rewardsExitScreen.position, _current);
            v_tranformFinalReward.rotation = Quaternion.Lerp(originRotation, Quaternion.identity, _current);
            v_tranformFinalReward.localScale = Vector3.Lerp(newScale, originScale, _current);
            yield return null;
        }
        Destroy(v_tranformFinalReward.gameObject);
        yield return null;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
