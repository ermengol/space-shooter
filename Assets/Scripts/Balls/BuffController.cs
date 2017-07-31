using UnityEngine;
using System.Collections;

public enum TypeBuff
{
	x2,
	Ball,
	Faster,
	Slower,
	Larger,
	Smaller
}

public class BuffController : MonoBehaviour {

	public GameController gc;

	public TypeBuff type;

	public float endTimestamp;

	public void Init(GameController aGC)
	{
		gc = aGC;
	}

	// Use this for initialization
	void OnCollisionEnter2D(Collision2D col) {
		
		if (col.gameObject.tag == "Ball") {
			Action(col);
		}
	}

	void Action(Collision2D col)
	{
		gc.IncreaseScore ();
		switch (type) 
		{
		case TypeBuff.Ball:
			gc.CreateBall(col.gameObject.transform.localPosition,
			              col.gameObject.GetComponent<BallController>().GetDir(),
			              col.gameObject.GetComponent<BallController>().GetSpeed());
			break;
		case TypeBuff.Faster:
			gc.IncreaseSpeed();
			break;
		case TypeBuff.Slower:
			gc.DecreaseSpeed();
			break;
		case TypeBuff.Larger:
			gc.IncreaseSize();
			break;
		case TypeBuff.Smaller:
			gc.DecreaseSize();
			break;
		case TypeBuff.x2:
			gc.ScoreX2();
			break;
		}

		Destroy (gameObject);
	}

	void Update()
	{
		if (Time.time > endTimestamp)
			Destroy (gameObject);
	}
}
