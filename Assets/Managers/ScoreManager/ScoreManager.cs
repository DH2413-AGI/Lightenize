using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScore
{
    public LevelScore(int levelIndex, int score)
    {
        this.levelIndex = levelIndex;
        this.score = score;
    }

    public int levelIndex { get; }
    public int score { get; set; }

}

public class ScoreManager : MonoBehaviour
{
    [Tooltip("The base score, i.e. the score you'd get if you cleared the level in 0 seconds.")]
    [SerializeField] private int _baseScore;

    [Tooltip("Time in seconds, used to calculate the level score if there are no base time defined for the particular level index")]
    [SerializeField] private int _defaultLevelBaseTime;

    [Tooltip("Defines the base time in seconds per level, used to calculate the level score. Element 0 is for the level placement.")]
    [SerializeField] private List<int> _levelIndexBaseTimes = new List<int> { 5 };
    private List<LevelScore> _levelScores = new List<LevelScore>();

#nullable enable
    private LevelScore? getLevelScoreObject(int levelIndex)
    {
        return this._levelScores.Find(levelScore => levelScore.levelIndex == levelIndex);
    }
#nullable disable
    /* 
        The score is calculated based on how many seconds the player has spent on the level. 
        First, we get the percentage of how long it took them to complete it based on the base time.
        If they went over the base time, we give them 5 points. Otherwise we give them the base score minus
        the base score times the percentage, plus 5.
    */
    private int getTimeBasedScore(int baseTime)
    {
        float timeClearPercentage = Time.timeSinceLevelLoad / baseTime;
        if (timeClearPercentage >= 1) return 5;
        return this._baseScore - Mathf.FloorToInt(timeClearPercentage * this._baseScore) + 5;
    }
    private int getLevelTimeScore(int levelIndex)
    {
        if (levelIndex < this._levelIndexBaseTimes.Count)
        {
            return getTimeBasedScore(this._levelIndexBaseTimes[levelIndex]);
        }
        return getTimeBasedScore(this._defaultLevelBaseTime);
    }

    public int getScoreByLevel(int levelIndex)
    {
        return this._levelScores.Find(levelScore => levelScore.levelIndex == levelIndex).score;
    }

    public int getTotalScore()
    {
        int totalScore = 0;
        foreach (LevelScore levelScore in this._levelScores)
        {
            totalScore += levelScore.score;
        }
        return totalScore;
    }

    /* I added this method if we want to add to the score when we clear sensors instead.*/
    public void addToLevelScore(int levelIndex, int scoreToAdd)
    {
        if (this.getLevelScoreObject(levelIndex) == null)
        {
            this._levelScores.Add(new LevelScore(levelIndex, scoreToAdd));
        }
        else
        {
            this.getLevelScoreObject(levelIndex).score += scoreToAdd;
        }

    }

    public void calculateAndAddLevelTimeScore(int levelIndex)
    {
        if (this.getLevelScoreObject(levelIndex) != null) return;
        this._levelScores.Add(new LevelScore(levelIndex, getLevelTimeScore(levelIndex)));
    }

}
