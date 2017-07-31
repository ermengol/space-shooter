using UnityEngine;
using System.Collections;

public enum ENEMY_TYPE
{
	BASIC,
	MULTIP, // after death, makes appear 3 basics
	MINIS,  // smallers && harders to hit
	ZIGZAG, // their movement would be zig zag
	FAST,   // basics with higher speed
	TELEPORT, // can evade teleporting to another place 1 time, slower
	BARRIER_CREATORS, // on death, create a 3 hit barrier
	BARRIER, //can hold 3 shots
	COUNT
}

//TODO: Fer la clase enemy genèrica.
public class Enemy : MonoBehaviour {

	public ENEMY_TYPE type;
	Vector3 rot;
	public float rotSpeed = 1;
	public float moveSpeed = 10;
	public int score = 1;

	public GameObject objective;
	public GameObject visualObj;

	public GameObject deathParticles;

	bool destroyed = false;

	public EnemyWaveController ewc;
	
	public GameObject miniPrefab;

	void Start()
	{
		rot = new Vector3 (Random.Range (-1, 1) * rotSpeed, Random.Range (-1, 1) * rotSpeed, Random.Range (-1, 1) * rotSpeed);
	}

	void Update()
	{
		if (destroyed)
			return;
		Movement ();
	}

	protected virtual void Movement()
	{
		visualObj.transform.Rotate (rot);

		Vector3 dir = objective.transform.position - transform.position;
		dir.z = 0;
		dir = dir.normalized * moveSpeed * Time.timeScale * 0.005f;
		transform.Translate (dir);
	}

	void OnTriggerEnter(Collider col)
	{
		if(destroyed)
			return;
			
		if (col.GetComponent<Collider> ().tag == "Bullet") 
		{
			AsteroidsGameController.instance.IncreaseScore(score);
			Destroy (col.gameObject);
			StartCoroutine(WaitAndDestroy());
		} 
		else if (col.GetComponent<Collider> ().tag == "Earth") 
		{
			//Gameover
			AsteroidsGameController.instance.StartEndGame();
		}
	}

	IEnumerator WaitAndDestroy()
	{
		destroyed = true;
		Destroy (visualObj);
		deathParticles.SetActive (true);
		DoSomethingAfterDeath ();
		yield return new WaitForSeconds (4f);
		Destroy (gameObject);
	}

	protected virtual void DoSomethingAfterDeath()
	{
		switch(type)
		{
		case ENEMY_TYPE.MULTIP:
			{
			for(int i = 0; i < 3; i++)
			{
				float radius = 0.7f;
				GameObject obj = GameObject.Instantiate (miniPrefab) as GameObject;
				obj.transform.position = transform.position + new Vector3(Random.Range(-radius,radius),Random.Range(-radius,radius),0);
				obj.transform.localRotation = Quaternion.identity;
				obj.GetComponent<Enemy> ().ewc = ewc;
				obj.GetComponent<Enemy> ().objective = objective;
			}
			}
			break;
		default:
			break;
		}
	}

}
