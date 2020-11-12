using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour {
	public Text opponentScore;
	public Text myScore;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		var mySelf = PoolGameController.GameInstance.mySelf;
		var opponent = PoolGameController.GameInstance.opponent;
		myScore.text = mySelf.Points.ToString();
		opponentScore.text = opponent.Points.ToString();
	}
}
