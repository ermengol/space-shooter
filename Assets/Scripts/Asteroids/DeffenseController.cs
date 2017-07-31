using UnityEngine;
using System.Collections;

public enum TypeShot
{
	SINGLE,    // one shot, pistol
	DOUBLE,	   // two shots
	TRIPLE,    //three shots
	FREEZER,   // one shot, slow, freezes
	EXPLODING, // destroy elements in radius
	LASER,     // x seconds laser ??
	COUNT,
	SPREAD //???
}

public class DeffenseController : MonoBehaviour 
{
	static public DeffenseController Instance;

	public Transform shotPoint;
	public GameObject shot;

	string recoilTimeKey = "recoilTime";
	string typeShotKey = "typeShot";

	float timeBetweenShots = 1f;
	float timeLastShot = 0;
	float speedShot = 2f;

	TypeShot shotType;
	
	public bool stop;

	void Start()
	{
		Instance = this;
	}
	
	public void InitDeffense()
	{
		if(PlayerPrefs.HasKey(typeShotKey))
		{
			shotType = (TypeShot)PlayerPrefs.GetInt(typeShotKey);
		}
	
		if(PlayerPrefs.HasKey(recoilTimeKey))
		{
			timeBetweenShots = PlayerPrefs.GetFloat(recoilTimeKey);
		}
	}
	
	public void Equip(TypeShot type, float aspeedShot)
	{
		shotType = type;
		speedShot = aspeedShot;
		PlayerPrefs.SetInt(typeShotKey,(int)shotType);
		PlayerPrefs.SetFloat(recoilTimeKey,speedShot);
	}
	
	public bool IsEquiped(TypeShot type)
	{
		return shotType == type;
	}

	// Update is called once per frame
	void Update () 
	{
		if(stop)
			return;
		if(Input.GetMouseButton(0) || Input.touchCount > 0)
		{
			Vector3 touchPos = Vector3.zero;
			if (Input.GetMouseButton (0)) 
			{
				touchPos = new Vector3(Input.mousePosition.x,Input.mousePosition.y) - new Vector3(Screen.width /2f, Screen.height/2f);
			}

			if (Input.touchCount > 0)
			{
				touchPos = new Vector3(Input.GetTouch(0).position.x,Input.GetTouch(0).position.y) - new Vector3(Screen.width /2f, Screen.height/2f);
			}

			Vector3 dir = new Vector3(touchPos.x,touchPos.y);

			transform.up = dir;

			Shot();
		}
	}

	void Shot()
	{
		if (Time.time > timeLastShot + timeBetweenShots) 
		{
			ShotSimple();
		}
	}

	void ShotSimple()
	{
		timeLastShot = Time.time;
		GameObject newShot = GameObject.Instantiate(shot);
		newShot.transform.parent = shotPoint;
		newShot.transform.localRotation = Quaternion.identity;
		newShot.transform.localPosition = Vector3.zero;
		newShot.transform.parent = null;
		newShot.GetComponent<Shot> ().speed = speedShot;
	}
}
