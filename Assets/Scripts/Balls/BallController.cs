using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour {

	float speed = 1.2f;
	float maxSpeed = 10f;
	int id;

	public void SetId(int aId)
	{
		id = aId;
	}

	public int GetId()
	{
		return id;
	}
	
	public void SetForce(Vector3 dir)
	{
		//Debug.LogError (speed.ToString());
		GetComponent<Rigidbody2D> ().velocity = Vector3.zero;
		GetComponent<Rigidbody2D> ().angularVelocity = 0;
		GetComponent<Rigidbody2D>().AddForce( dir * speed);
		speed = Mathf.Clamp(speed + 0.2f,0,maxSpeed);

	}

	public void ChangeSpeed(float amount)
	{
		Vector2 actualDir = GetDir();
		GetComponent<Rigidbody2D> ().velocity = Vector3.zero;
		GetComponent<Rigidbody2D> ().angularVelocity = 0;
		speed = Mathf.Clamp(speed*amount,2,maxSpeed);
		GetComponent<Rigidbody2D>().AddForce( actualDir * speed);
	}

	void FixedUpdate()
	{
		if (GetComponent<Rigidbody2D> ().velocity.y <= 0.3f && GetComponent<Rigidbody2D> ().velocity.y >= -0.3f) 
		{
			speed = Mathf.Clamp(speed - 0.2f,0,maxSpeed);
			SetForce(GetDir() + new Vector3(0,(GetComponent<Rigidbody2D> ().velocity.y > 0)? 0.3f:-0.3f));
		}
	}

	public Vector3 GetDir()
	{
		return GetComponent<Rigidbody2D> ().velocity.normalized;
	}

	public void SetSpeed(float aSpeed)
	{
		speed = aSpeed;
	}


	public float GetSpeed()
	{
		return speed;
	}

}
