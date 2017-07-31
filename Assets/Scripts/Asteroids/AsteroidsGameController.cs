using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Advertisements;


public class AsteroidsGameController : MonoBehaviour 
{
	static public AsteroidsGameController instance;
	public GameObject Earth;
	public DeffenseController Player;



	public bool gameStarted = false;

	//Info
	string scoreKey = "maxScore";
	int maxScore;
	int score = 0;
	
	string coinsKey = "coins";
	int totalCoins;
	//UI
	public GameObject tutorialUI;
	
	public GameObject inGameUI;
	public Text scoreLabel; 
	
	public GameObject endUI;
	public Text endScoreLabel; 
	public Text maxScoreLabel; 
	
	public void IncreaseScore(int value)
	{
		score += value;
		UpdateScore (score);
	}
	void Awake()
	{
		instance = this;
	}
	// Use this for initialization
	void Start () {
		
		if (PlayerPrefs.HasKey (scoreKey))
			maxScore = PlayerPrefs.GetInt (scoreKey);
		if(PlayerPrefs.HasKey(coinsKey))
			totalCoins = PlayerPrefs.GetInt(coinsKey);
		tutorialUI.SetActive (true);
		inGameUI.SetActive (false);
		endUI.SetActive (false);
		
		
		Player.stop = true;
		//ADVERTISING
		
		
		if (Advertisement.isSupported) {
			Advertisement.allowPrecache = true;
			Advertisement.Initialize ("131622966",true);
		}
	}

	public void UpdateScore(int score)
	{
		scoreLabel.text = score.ToString ();
	}
	
	int countToShowAd = 1;
	
	public void StartEndGame()
	{
		EnemyWaveController.instance.Clear();
		DeffenseController.Instance.enabled =false;
		if (countToShowAd % 4 == 0) 
		{
			if(Advertisement.isReady())
				Advertisement.Show(null, new ShowOptions {
					pause = true,
					resultCallback = result => {
						Debug.Log(result.ToString());
					}
				});
		}
		countToShowAd++;
		
		inGameUI.SetActive (false);
		endUI.SetActive (true);
		
		gameStarted = false;
		totalCoins += score;
		PlayerPrefs.SetInt(coinsKey,totalCoins);
		
		endScoreLabel.text = scoreLabel.text;
		maxScoreLabel.text = totalCoins.ToString ();
	}
	
	public int GetPlayerCoins()
	{
		return PlayerPrefs.GetInt(coinsKey);
	}
	
	public void RemovePlayerCoins(int value)
	{
		totalCoins = PlayerPrefs.GetInt(coinsKey);
		totalCoins += score;
		PlayerPrefs.SetInt(coinsKey,totalCoins);
	}
	
	public void InitGame()
	{
		Player.stop = false;
		DeffenseController.Instance.enabled = true;
		Player.InitDeffense();
		tutorialUI.SetActive (false);
		scoreLabel.text = "0";
		inGameUI.SetActive (true);
		endUI.SetActive (false);
		score = 0;
		gameStarted = true;
		EnemyWaveController.instance.Init();

	}
	
	// Update is called once per frame
	void Update () {
		if (!gameStarted && !endUI.activeInHierarchy) 
		{
			if (Input.touchCount > 0) {
				InitGame();
			}
			#if UNITY_EDITOR
			if(Input.GetMouseButton(0))
			{
				InitGame();
			}
			#endif
		}
		else if (gameStarted) 
		{
		
		}
	}
}
