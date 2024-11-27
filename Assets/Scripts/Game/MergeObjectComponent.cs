using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeObjectComponent : MonoBehaviour
{
    private string objectID;
    private int objectProgressionIndex;
    bool hovered = false;
    Vector3 startingScale;
    bool selected = false;

    public delegate void HandleSelectedByUser(MergeObjectComponent v_mergeObject);
    public static event HandleSelectedByUser OnSelectedByUser;

    private void Start()
    {
        startingScale = transform.localScale;
    }

    public Vector3 GetStartingScale() => startingScale;
    public void SetObjectID(string v_newID)
    {
        objectID = v_newID;
    }
    public string GetObjectID() => objectID;

    public void SetObjectProgressionIndex(int v_index)
    {
        objectProgressionIndex = v_index;
    }

    public int GetObjectProgressionIndex() => objectProgressionIndex;

    public void OnTouchedByUser()
    {
        if (!selected)
        {
            OnSelectedByUser?.Invoke(this);
            selected = true;
        }
    }

    public void OnHoverStart()
    {
        hovered = true;
        //renderedObject.material = hoveredMaterial;
    }

    public void OnHoverEnd()
    {
        hovered = false;
        //renderedObject.material = normalMaterial;
    }

    public bool IsHovered() => hovered;

    public bool IsSelected() => selected;

    public void Unselect()
    {
        selected = false;
    }
}
