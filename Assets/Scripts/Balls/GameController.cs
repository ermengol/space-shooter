using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class GameController : MonoBehaviour {

	float speedBall = 2f;

	Vector2 initialDir = new Vector2(0,-1f);
	

	public GameObject playerBar;

	public Transform leftWall;
	public Transform rightWall;
	public Transform topWall;

	//Ball
	public GameObject ballsParent;
	public GameObject prefabBall;
	int countBalls = 0;
	public Dictionary<int,GameObject> actualBalls = new Dictionary<int, GameObject>();

	//Buffs
	public GameObject buffContainer;
	public List<GameObject> prefabsBuffs;

	List<int> probabilities = new List<int>();
	
	float timeBuffDisappear = 15;
	float randOffsetBufDisappear = 7;
	
	float timeToBuffAppear;
	float timeBuffAppear = 8;
	float randBuffAppear = 2;

	public bool gameStarted = false;


	//Info
	string scoreKey = "maxScore";
	int maxScore;


	//UI
	public GameObject tutorialUI;

	public GameObject inGameUI;
	public Text scoreLabel; 

	public GameObject endUI;
	public Text endScoreLabel; 
	public Text maxScoreLabel; 

	public void IncreaseScore()
	{
		playerBar.GetComponent<BarController> ().score++;
		UpdateScore (playerBar.GetComponent<BarController> ().score);
	}

	// Use this for initialization
	void Start () {
				//4 tiers of probs
		//1: large,small - 45%; 2:HS,LS - 30%; 3:duplicate 15%; 4:X2 - 5%
		probabilities.Add (25);
		probabilities.Add (45);
		probabilities.Add (60);
		probabilities.Add (80);
		probabilities.Add (90);
		probabilities.Add (100);

		if (PlayerPrefs.HasKey (scoreKey))
			maxScore = PlayerPrefs.GetInt (scoreKey);

		tutorialUI.SetActive (true);
		inGameUI.SetActive (false);
		endUI.SetActive (false);

		//ADVERTISING


		if (Advertisement.isSupported) {
			Advertisement.allowPrecache = true;
			Advertisement.Initialize ("131622966",true);
		}
	}

	public void CreateBall(Vector3 pos, Vector3 dir, float aSpeed)
	{
		GameObject auxBall = GameObject.Instantiate (prefabBall);
		auxBall.transform.parent = ballsParent.transform;
		auxBall.transform.localPosition = pos;
		actualBalls.Add (countBalls, auxBall);
		auxBall.GetComponent<BallController> ().SetId (countBalls);
		auxBall.GetComponent<BallController> ().SetSpeed (aSpeed);
		auxBall.GetComponent<BallController> ().SetForce (dir);

		countBalls++;
	}

	public void DeleteBall(GameObject ball)
	{
		if (actualBalls.ContainsKey( ball.GetComponent<BallController> ().GetId ())) 
		{
			actualBalls.Remove(ball.GetComponent<BallController> ().GetId ());
			if(actualBalls.Count == 0)
				StartEndGame();
		}

		Destroy (ball);
	}

	public void UpdateScore(int score)
	{
		scoreLabel.text = score.ToString ();
	}

	int countToShowAd = 0;

	void StartEndGame()
	{

		if (countToShowAd % 3 == 0) 
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
		if (playerBar.GetComponent<BarController> ().score > maxScore) 
		{
			maxScore = playerBar.GetComponent<BarController> ().score;
		}
		PlayerPrefs.SetInt (scoreKey, maxScore);

		endScoreLabel.text = scoreLabel.text;
		maxScoreLabel.text = maxScore.ToString ();

		BuffController[] activeBuffs = GameObject.FindObjectsOfType<BuffController> ();
		for (int i = 0; i < activeBuffs.Length; i++) 
		{
			Destroy(activeBuffs[i].gameObject);
		}
	}

	public void InitGame()
	{
		tutorialUI.SetActive (false);
		scoreLabel.text = "0";
		inGameUI.SetActive (true);
		endUI.SetActive (false);

		timeToBuffAppear = Time.time + 8;
		gameStarted = true;
		playerBar.GetComponent<BarController> ().Init ();
		playerBar.GetComponent<BarController> ().score = 0;

		foreach (GameObject ball in actualBalls.Values) {
			Destroy(ball);		
		}
		actualBalls.Clear ();

		//Create first ball
		CreateBall (new Vector2 (0, -1), initialDir, speedBall);
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
			if(timeToBuffAppear < Time.time)
			{
				float timeToDisappear = Time.time + timeBuffDisappear + Random.Range(-randOffsetBufDisappear,randOffsetBufDisappear);
				timeToBuffAppear = Time.time + timeBuffAppear + Random.Range(-randBuffAppear,randBuffAppear);

				int buff = Random.Range(0,100) +1;
				for(int i = 0; i < probabilities.Count; i++)
				{
					if(probabilities[i] > buff)
					{
						GameObject buffObj = GameObject.Instantiate(prefabsBuffs[i]);
						buffObj.transform.parent = buffContainer.transform;
						buffObj.GetComponent<BuffController>().gc = this;
						buffObj.GetComponent<BuffController>().endTimestamp = timeToDisappear;

						float offsetX = leftWall.gameObject.GetComponent<Collider2D>().bounds.size.x/2 + buffObj.GetComponent<Collider2D>().bounds.size.x/2;
						float offsetY = topWall.gameObject.GetComponent<Collider2D>().bounds.size.x/2 + buffObj.GetComponent<Collider2D>().bounds.size.y/2;
						float randX = Random.Range(leftWall.transform.localPosition.x + offsetX,rightWall.transform.localPosition.x - offsetX);
						float randY = Random.Range(topWall.transform.localPosition.y - offsetY, playerBar.transform.localPosition.y + 1.4f);
					
						Vector3 pos = new Vector3(randX,randY,0);
						buffObj.transform.localPosition = pos;
						break;
					}
				}
			}
		}
	}

	public void IncreaseSpeed()
	{
		foreach(GameObject ball in actualBalls.Values)
		{
			BallController bc = ball.GetComponent<BallController>();
			bc.ChangeSpeed(1.5f);
		}
	}

	public void DecreaseSpeed()
	{
		foreach(GameObject ball in actualBalls.Values)
		{
			BallController bc = ball.GetComponent<BallController>();
			bc.ChangeSpeed(0.6f);
		}
	}

	public void DecreaseSize()
	{
		if (playerBar.transform.localScale.x == 1.4f) 
		{
			playerBar.transform.localScale = new Vector3(1,1,1);
		}
		else if (playerBar.transform.localScale.x == 1f) 
		{
			playerBar.transform.localScale = new Vector3(0.5f,1,1);
		}
	}

	public void IncreaseSize()
	{
		if (playerBar.transform.localScale.x == 0.5f) 
		{
			playerBar.transform.localScale = new Vector3(1,1,1);
		}
		else if (playerBar.transform.localScale.x == 1f) 
		{
			playerBar.transform.localScale = new Vector3(1.4f,1,1);
		}
	}

	public void ScoreX2()
	{
		playerBar.GetComponent<BarController> ().score *= 2;
		UpdateScore (playerBar.GetComponent<BarController> ().score);
	}
}
