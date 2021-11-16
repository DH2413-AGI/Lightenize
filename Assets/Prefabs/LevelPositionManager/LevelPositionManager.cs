using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPositionManager : MonoBehaviour
{

    private Pose _levelSpawnPosition = new Pose();

    public Pose LevelSpawnPosition
    {
        get => _levelSpawnPosition;
    }

    public void UpdateLevelSpawnPosition(Pose pose)
    {
        this._levelSpawnPosition = pose;
    }

    void Start() {
        DontDestroyOnLoad(this.gameObject);
    }

}
