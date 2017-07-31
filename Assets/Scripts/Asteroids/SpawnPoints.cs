using UnityEngine;
using System.Collections;

public class SpawnPoints : MonoBehaviour {

	float timeSinceLastSpawn;
	float timeBetweenSpawns = 0.2f;

	public bool Available()
	{
		return Time.time > timeSinceLastSpawn + timeBetweenSpawns;
	}

	public GameObject Spawn(GameObject prefab, GameObject objective, EnemyWaveController ewc)
	{
		if (Time.time < timeSinceLastSpawn + timeBetweenSpawns)
			return null;
		timeSinceLastSpawn = Time.time;

		float radius = gameObject.GetComponent<SphereCollider> ().radius;
		GameObject obj = GameObject.Instantiate (prefab) as GameObject;
		obj.transform.position = transform.position + new Vector3(Random.Range(-radius,radius),Random.Range(-radius,radius),0);
		obj.transform.localRotation = Quaternion.identity;
		obj.GetComponent<Enemy> ().ewc = ewc;
		obj.GetComponent<Enemy> ().objective = objective;

		return obj;
	}
}
