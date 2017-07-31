using UnityEngine;
using System.Collections;

public class BarController : MonoBehaviour {

	public int score;
	public GameController gc;
	Vector3 desiredPos;
	float sizeColX = 1.55f;
	// Use this for initialization
	void Start () {
		desiredPos = new Vector3 (0, -4.09f, 0);
	}

	public void Init()
	{
		score = 0;
		desiredPos = new Vector3 (0, -4.09f, 0);
		transform.localPosition = desiredPos;
	}
	
	// Update is called once per frame
	void Calc () {
	
		if (gc.gameStarted) 
		{
			if (Input.touchCount > 0) {
					SetPos (Input.GetTouch (0).position);
			}
#if UNITY_EDITOR
			if (Input.GetMouseButton (0)) {
					SetPos (Input.mousePosition);
			}
#endif
		}

	}



	void FixedUpdate()
	{
		Calc ();
		transform.localPosition = desiredPos;
	}
	
	float speed = 0.1f;
	void SetPos(Vector3 newPos)
	{
		Vector3 actualPos = gameObject.transform.localPosition;
		newPos = Camera.main.ScreenToWorldPoint(new Vector3(newPos.x, newPos.y, 0));
		newPos = new Vector3 (newPos.x, -4.09f, 0);

		float leftWallBoundary = gc.leftWall.transform.localPosition.x + 0.5f * 2.5f;

		float rightWallBoundary = gc.rightWall.transform.localPosition.x - 0.5f * 2.5f;
		newPos.x = Mathf.Clamp (newPos.x,leftWallBoundary + sizeColX * transform.localScale.x / 2, rightWallBoundary - sizeColX * transform.localScale.x/2);

		float dirx = newPos.x - transform.localPosition.x;
		bool negative = dirx < 0;

		if (dirx < speed && dirx> -speed)
			return;

		if (negative) 
		{
			desiredPos -= Vector3.right * speed;
		}
		else 
		{
			desiredPos += Vector3.right * speed;
		}

		float afterdirX = newPos.x - transform.localPosition.x;
		if (negative != (afterdirX < 0)) 
		{
			desiredPos = newPos;		
		}


	}

	Vector2 getDirection(Vector3 ballPos, Vector3 racketPos, float racketWidth) {
		float dx = (ballPos.x - racketPos.x) / racketWidth;
		Vector2 dir = new Vector2(-dx, 1).normalized;
		return dir;
	}
	
	void OnCollisionEnter2D(Collision2D col) {
	
		if (col.gameObject.tag == "Ball") {
			score++;
			gc.UpdateScore(score);
			col.gameObject.GetComponent<BallController>().SetForce(getDirection(transform.position,
			                                                                    col.transform.position,
			                                                                    col.collider.bounds.size.x));
		}
	}
}
