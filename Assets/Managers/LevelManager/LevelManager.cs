using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private List<string> _levels;

    [SerializeField] private bool _allowToSkipLevels;

    private Animator _levelContainerAnimator;

    private int _currentLevelIndex;

    private PlayerInput _playerInput;

    private ScoreManager _scoreManager;

    // Start is called before the first frame update
    void Start()
    {
        this._currentLevelIndex = this.LevelIndexFromCurrentScene();
        this._playerInput = FindObjectOfType<PlayerInput>();
        this.GetLevelRelatedObjects();

        if (this._allowToSkipLevels)
        {
            this._playerInput.actions.FindActionMap("Player").FindAction("SkipLevel").performed += SkipLevel;
        }
    }

    private void GetLevelRelatedObjects()
    {
        this._levelContainerAnimator = FindObjectOfType<LevelContainer>().GetComponent<Animator>();
        this._scoreManager = FindObjectOfType<ScoreManager>();
    }

    private int LevelIndexFromCurrentScene()
    {
        var currentLevelIndex = this._levels.IndexOf(SceneManager.GetActiveScene().name);
        if (currentLevelIndex == -1) return 0;
        return currentLevelIndex;
    }

    private IEnumerator LoadNextLevelCorunite()
    {
        if (IsCurrentLevelLastLevel()) yield break;
        this._currentLevelIndex++;
        var nameOfSceneToLoad = this._levels[this._currentLevelIndex];

        _levelContainerAnimator.SetTrigger("FadeOut");

        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene(nameOfSceneToLoad);
        yield return new WaitForSeconds(1.0f);

        this.GetLevelRelatedObjects();
    }

    public void LoadNextLevel()
    {
        this._scoreManager.calculateAndAddLevelTimeScore(this._currentLevelIndex);
        // Debug.Log(this._scoreManager.getScoreByLevel(this._currentLevelIndex));
        // Debug.Log(this._scoreManager.getTotalScore());
        StartCoroutine(this.LoadNextLevelCorunite());
    }

    public bool IsCurrentLevelLastLevel()
    {
        return this._currentLevelIndex == this._levels.Count - 1;
    }

    private void SkipLevel(InputAction.CallbackContext context)
    {
        this.LoadNextLevel();
    }

}
