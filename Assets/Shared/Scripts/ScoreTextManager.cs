using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreTextManager : MonoBehaviour
{

	[SerializeField] private TMP_Text _scoreText;
	
    public void ShowFinalScore(int score)
    {
    	this._scoreText.text = $"Game Completed! Your score is: {score}";
    }

}



