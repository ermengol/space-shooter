using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyWaveController : MonoBehaviour {

	static public EnemyWaveController instance;
	int numWave = 0;
	int initialEnemiesXWave = 4;
	float amountMultiplierEnemiesXWave = 1.4f;

	public List<SpawnPoints> _spawnPoints = new List<SpawnPoints> ();
	public List<GameObject> _enemyObjs = new List<GameObject> ();
	public List<int> _enemyInitialWave = new List<int> ();
	List<GameObject> _enemiesInWave = new List<GameObject> ();
	List<int> _amountEnemiesInWave = new List<int> ();

	float timeSinceLastWave;
	float timeBetweenWaves = 1f;
	float timeSinceLastSpawn;
	float timeBetweenSpawns = 2f;
	float minTimeBetweenSpawns = 0.1f;

	bool working = false;
	bool enterNextWave = false;

	void Start()
	{
		instance = this;

		Clear ();
	}

	// Use this for initialization
	public void Init () {
		working = true;
	}
	
	List<int> queueEnemiesInWave = new List<int>();
	Stack<int> enemiesInWaveOrder = new Stack<int>();
	
	// Update is called once per frame
	void Update () {
		if(!working)
			return;
			
		if(enterNextWave)
		{
			if(timeSinceLastWave + timeBetweenWaves < Time.time)
			{
				CalculateNextWave();
				enterNextWave = false;
			}
		}
		else
		{
			if(queueEnemiesInWave.Count > 0)
			{
				if(timeSinceLastSpawn + timeBetweenSpawns > Time.time)
					return;
				List<SpawnPoints> al = new List<SpawnPoints>();
				for(int j = 0; j < _spawnPoints.Count; j++)
				{
					if(_spawnPoints[j].Available())
						al.Add(_spawnPoints[j]);
				}
				
				if(al.Count == 0)
				{
					timeSinceLastSpawn += 0.2f;
				}
				else
				{
					int spawn = Random.Range(0,al.Count);
					timeSinceLastSpawn = Time.time;
					//TODO: falta canviar el primer param per a que siga progressiu i tinga tot tipus d'enemics
					al[spawn].Spawn(_enemyObjs[queueEnemiesInWave[0]],AsteroidsGameController.instance.Earth,this);
					queueEnemiesInWave.RemoveAt(0);
				}
				//if we come until here, we have spawned an object or we need to wait
				return;
			}
			
			//if we came this far, we are entering next wave!
			timeSinceLastWave = Time.time;
			enterNextWave = true;
		}
	}
	
	public void Shuffle()
	{
		int n = queueEnemiesInWave.Count;
		
		while (n > 1)
		{
			n--;
			int k = Random.Range(0,n - 1);
			int tmp = queueEnemiesInWave[k];
			queueEnemiesInWave[k] = queueEnemiesInWave[n];
			queueEnemiesInWave[n] = tmp;
		}
	}


	void CalculateNextWave()
	{
		queueEnemiesInWave.Clear();
		numWave++;
		timeBetweenSpawns = Mathf.Clamp(timeBetweenSpawns,minTimeBetweenSpawns,0.9f * timeBetweenSpawns);
		
		for (int i = 0; i < _enemyInitialWave.Count; i++) 
		{
			if(_enemyInitialWave[i] < numWave)
			{
				_amountEnemiesInWave[i] = (_amountEnemiesInWave[i] != 0) ? (int)(_amountEnemiesInWave[i] * amountMultiplierEnemiesXWave):
																		initialEnemiesXWave;
			}
			
			if(_amountEnemiesInWave[i] != 0)
			{
				for(int z = 0; z < _amountEnemiesInWave[i]; z++)
				{
					queueEnemiesInWave.Add(i);
				}
			}
		}
		
		Shuffle();
		
		Debug.LogError(numWave + ": " + timeBetweenSpawns + "  " + _amountEnemiesInWave.Count);	
	}
	
	public int GetWave()
	{
		return numWave;
	}


	public void Clear()
	{
		working = false;
		
		Enemy[] activeEnemies = GameObject.FindObjectsOfType<Enemy> ();
		for (int i = 0; i < activeEnemies.Length; i++) 
		{
			Destroy(activeEnemies[i].gameObject);
		}

		_amountEnemiesInWave.Clear ();
		for (int i= 0; i< _enemyObjs.Count; i++) 
		{
			_amountEnemiesInWave.Add(0);
		}

		_enemiesInWave.Clear ();
		timeSinceLastSpawn = 0;
		timeBetweenWaves = 1f;
		
		timeSinceLastSpawn = 0;
		timeBetweenSpawns = 2f;
		numWave = 0;
		amountMultiplierEnemiesXWave = 1.4f;
		enterNextWave = false;
	}
}
