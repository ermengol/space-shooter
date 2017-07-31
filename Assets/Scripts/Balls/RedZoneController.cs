using UnityEngine;
using System.Collections;

public class RedZoneController : MonoBehaviour {

	public GameController gc;

	void OnTriggerEnter2D(Collider2D col) {
		
		if (col.gameObject.tag == "Ball") {
			//Tell gc
			gc.DeleteBall(col.gameObject);
		}
	}
}
