using UnityEngine;
using System.Collections;


//TODO: fer-ho genèric per als diferents tipus de bales
public class Shot : MonoBehaviour {

	public float speed = 2;
	public float timeToDestroy = 5f;
	float timeSinceCreation;
	// Use this for initialization
	void Start () {
		timeToDestroy = (speed > 1) ? 5f : 5f / speed;
		timeSinceCreation = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		transform.localPosition += transform.up * speed * Time.deltaTime;

		if (Time.time > timeSinceCreation + timeToDestroy) 
		{
			Destroy(gameObject);
		}
	}
}
