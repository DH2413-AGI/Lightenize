using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPlacementData : MonoBehaviour
{
    private Pose _levelPose = new Pose();

    public Pose LevelPose
    {
        get => _levelPose;
    }

    public void SetLevelPose(Pose pose)
    {
        this._levelPose = pose;
    }
}
