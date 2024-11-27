using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObjectsPositionReference : MonoBehaviour
{
    public Transform objectOneCompareSpot;
    public Transform objectTwoCompareSpot;
    public Transform furnaceDoorEntrance;
    public Transform mergeObjectsPosition;
    public Transform mergedObjectSpawner;
    public Transform rewardsPresentation;
    public Transform rewardsExitScreen;

    public float wallOffset;
    public Transform sideLeftWall;
    public Transform sideRightWall;
    public Transform farPlaneWall;
    public Transform nearPlaneWall;

    public Transform objectsHolder;

    

    public float LeftSideLimit() => sideLeftWall.position.x - wallOffset;
    public float RightSideLimit() => sideRightWall.position.x + wallOffset;
    public float FarPlaneLimit() => farPlaneWall.position.z + wallOffset;
    public float NearPlaneLimit() => nearPlaneWall.position.z - wallOffset;
}
