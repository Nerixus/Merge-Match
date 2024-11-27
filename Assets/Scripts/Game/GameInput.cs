using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public float scaleChange;
    float previousScale;
    MergeObjectComponent currentObjectHovered;
    public bool inputActive = true;

    // Update is called once per frame

    void DragFinger(Vector3 v_pos)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(v_pos);
        if (Physics.Raycast(ray, out hit))
        {
            MergeObjectComponent match3Component = hit.transform.GetComponent<MergeObjectComponent>();
            if (match3Component != null)
            {
                if (currentObjectHovered == null)
                {
                    currentObjectHovered = match3Component;
                    if (!currentObjectHovered.IsHovered())
                    {
                        currentObjectHovered.OnHoverStart();
                        previousScale = currentObjectHovered.transform.localScale.x;
                        currentObjectHovered.transform.localScale = Vector3.one * previousScale * scaleChange;
                    }

                }
                else if (match3Component != currentObjectHovered)
                {
                    currentObjectHovered.OnHoverEnd();
                    currentObjectHovered.transform.localScale = Vector3.one * previousScale;
                    currentObjectHovered = match3Component;
                    if (!currentObjectHovered.IsHovered())
                    {
                        currentObjectHovered.OnHoverStart();
                        previousScale = currentObjectHovered.transform.localScale.x;
                        currentObjectHovered.transform.localScale = Vector3.one * previousScale * scaleChange;
                    }
                }
            }
            else
            {
                if (currentObjectHovered != null)
                {
                    currentObjectHovered.OnHoverEnd();
                    currentObjectHovered.transform.localScale = Vector3.one * previousScale;
                    currentObjectHovered = null;
                }
            }
        }
    }

    void FingerUp(Vector3 v_pos)
    {
        if (currentObjectHovered != null)
        {
            currentObjectHovered.OnHoverEnd();
            currentObjectHovered.transform.localScale = Vector3.one * previousScale;
            currentObjectHovered = null;
        }
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(v_pos);
        if (Physics.Raycast(ray, out hit))
        {
            MergeObjectComponent mergeObject = hit.transform.GetComponent<MergeObjectComponent>();
            if (mergeObject != null)
            {
                if (!mergeObject.IsSelected())
                {
                    mergeObject.transform.localScale = Vector3.one * previousScale * scaleChange;
                    mergeObject.OnTouchedByUser();
                }
            }
        }
    }
    void Update()
    {
        if (inputActive)
        {
            if (Application.isMobilePlatform)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.touches[0];
                    Vector3 pos = touch.position;
                    if (touch.phase == TouchPhase.Began)
                    {
                        DragFinger(pos);
                    }
                    else if (touch.phase == TouchPhase.Moved)
                    {
                        DragFinger(pos);
                    }
                    else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)//End drag or tap
                    {
                        FingerUp(pos);
                    }
                }
            }
            else
            {
                Vector3 pos = Input.mousePosition;

                if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.A))
                {
                    DragFinger(pos);
                }
                if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.S))
                {
                    FingerUp(pos);
                }
            }
        }
    }
}
