using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SetupManager : MonoBehaviour
{
    private static bool SetupCompleted = false;

    [SerializeField] private GameObject _arSessionPrefab;

    [SerializeField] private GameObject _compatibilityCheckerPrefab;

    private CrossPlatformCamera _crossPlatformCamera;

    void Awake()
    {
        if (SetupCompleted) return;
        this.SpawnManagers();
    }


    // Start is called before the first frame update
    void Start()
    {
        if (SetupCompleted) return;
        CompatibilityChecker.AddListener(ManagerDeviceSupport);
        SetupCompleted = true;
    }

    private void ManagerDeviceSupport(CompatibilityChecker.DeviceSupport deviceSupport)
    {
        switch (deviceSupport)
        {
            case CompatibilityChecker.DeviceSupport.AR:
                EnableARMode();
                return;
            default:
                EnableDesktopMode();
                return;
        }
    }

    private void SpawnManagers()
    {
        SpawnPersistantManagerOnce(_arSessionPrefab);
        SpawnPersistantManagerOnce(_compatibilityCheckerPrefab);
    }

    private void SpawnPersistantManagerOnce(GameObject prefab)
    {
        var spawnedObject = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        DontDestroyOnLoad(spawnedObject);
    }

    private void EnableARMode()
    {
        Debug.Log("Device mode: AR");
        this.EnableARSessionScripts();
    }

    private void EnableARSessionScripts()
    {
        FindObjectOfType<ARSession>().enabled = true;
        FindObjectOfType<ARInputManager>().enabled = true;
    }

    private void EnableDesktopMode()
    {
        Debug.Log("Device mode: Desktop");
    }
}
