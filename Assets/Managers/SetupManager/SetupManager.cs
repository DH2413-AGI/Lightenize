using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

public class SetupManager : MonoBehaviour
{
    private static bool SetupCompleted = false;

    public static bool IsSetupCompleted
    {
        get => SetupCompleted;
    }

    [SerializeField] private GameObject _arSessionPrefab;

    [SerializeField] private GameObject _compatibilityCheckerPrefab;

    [SerializeField] private GameObject _levelPlacementDataPrefab;

    [SerializeField] private GameObject _levelManagerPrefab;

    [SerializeField] private GameObject _inputManager;

    [SerializeField] private GameObject _scoreManager;

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
        CompatibilityChecker.AddListener(delegate (CompatibilityChecker.DeviceSupport supportedDevice)
        {
            SetupCompleted = true;
        });
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
        SpawnPersistantManagerOnce(_levelPlacementDataPrefab);
        SpawnPersistantManagerOnce(_levelManagerPrefab);
        SpawnPersistantManagerOnce(_inputManager);
        SpawnPersistantManagerOnce(_scoreManager);
    }

    private void SpawnPersistantManagerOnce(GameObject prefab)
    {
        var spawnedObject = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        DontDestroyOnLoad(spawnedObject);
    }

    private void EnableARMode()
    {
        Debug.Log("Device mode: AR");
        Screen.orientation = ScreenOrientation.LandscapeLeft;
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
