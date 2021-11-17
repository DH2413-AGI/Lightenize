using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelContainer : MonoBehaviour
{
    private LevelPlacementData _levelPlacementData;

    void Start()
    {
        this._levelPlacementData = FindObjectOfType<LevelPlacementData>();
    }

    void Update()
    {
        this.transform.SetPositionAndRotation(
            this._levelPlacementData.LevelPose.position,
            this._levelPlacementData.LevelPose.rotation
        );
    }
}
