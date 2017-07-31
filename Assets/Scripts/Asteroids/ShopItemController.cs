using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

//TODO: repensar un poc com montar el tema de la tenda

public class ShopItem
{
	TypeShot _type;
	int _level;
	float _speedShot;
	float multiplier = 0.2f;
	int _basePrice;
	
	public void SetUserValues(int level)
	{
		_level = level;
	}
	
	public ShopItem(TypeShot type, int level, int price, float speedShot)
	{
		_type = type;
		_level = level;
		_basePrice = price;
		_speedShot = speedShot;
	}
	
	public int GetPrice()
	{
		return _basePrice * (_level + 1) - (int)(_level*_basePrice*multiplier);
	}
	
	public bool CanBuyItem()
	{
		return AsteroidsGameController.instance.GetPlayerCoins() > GetPrice();
	}
	
	public void Buy()
	{
		_level++;
		PlayerPrefs.SetInt(_type.ToString(),_level);
		AsteroidsGameController.instance.RemovePlayerCoins(GetPrice());
	}
	
	public float GetSpeedLevel()
	{
		return _speedShot * (_level + 1) - _level*_speedShot*multiplier; 
	}
	
	public float GetRecoil()
	{
		float bulletsXMin = GetSpeedLevel();
		return 60f/bulletsXMin;
	}
}

public class ShopData
{
	Dictionary<TypeShot,ShopItem> shopDic = new Dictionary<TypeShot, ShopItem>();
	
	ShopItem GetInitialDataGivenType(TypeShot type)
	{
		ShopItem si = null;
		switch(type)
		{
			case TypeShot.SINGLE:
			si = new ShopItem(type,1,10, 20);
			break;
		case TypeShot.DOUBLE:
			si = new ShopItem(type,0,500, 20);
			break;
		case TypeShot.TRIPLE:
			si = new ShopItem(type,0,100000, 20);
			break;
		case TypeShot.FREEZER:
			si = new ShopItem(type,0,5000, 20);
			break;
		case TypeShot.EXPLODING:
			si = new ShopItem(type,0,20000, 10);
			break;
		case TypeShot.LASER:
			si = new ShopItem(type,0,1000000, 20);
			break;
		}
		
		return si;
	}
	
	public ShopData()
	{
		for(int i = 0; i < (int)TypeShot.COUNT; i++)
		{
			TypeShot type = (TypeShot)i;
			if(PlayerPrefs.HasKey(type.ToString()))
			{
				ShopItem item = GetInitialDataGivenType(type);
				item.SetUserValues(PlayerPrefs.GetInt(type.ToString()));
				shopDic.Add (type,item);
			}
			else
			{
				shopDic.Add(type,GetInitialDataGivenType(type));
			}
		}
	}
	
	public void BuyItem(TypeShot type)
	{
		if(shopDic[type].CanBuyItem())
		{
			shopDic[type].Buy();
		}
	}
}

public class ShopItemController : MonoBehaviour {

	public TypeShot element;
	public Text nameLabel;
	public Text speedText;
	public Button equip;
	public Button buy;
	
	public void SetItem(TypeShot aType)
	{
	}
}
