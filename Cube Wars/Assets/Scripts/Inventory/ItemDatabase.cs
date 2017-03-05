using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;

public class ItemDatabase : MonoBehaviour {
	List<Item> database = new List<Item>();
	JSONNode jsonData;

	void Start() {

		jsonData = JSON.Parse(File.ReadAllText(Application.dataPath + "/StreamingAssets/itemJson.json"));
	
		ConstructItemDatabase();
	}

	public Item FetchItemByID(int id) {

		for(int i = 0; i < database.Count; i++) {
			if (database[i].ID == id) return database[i];
		}

		return new Item();
	}

	void ConstructItemDatabase() {
		for(int i = 0; i < jsonData.Count; i++) {

			database.Add(new Item(jsonData[i]["id"].AsInt, jsonData[i]["title"].Value, jsonData[i]["value"].AsInt,
				jsonData[i]["stats"]["cooldown"].AsFloat, jsonData[i]["stats"]["muzzleVelocity"].AsFloat, jsonData[i]["stats"]["burstAmount"].AsInt, jsonData[i]["stats"]["damage"].AsFloat,
				jsonData[i]["stackable"].AsBool, jsonData[i]["dropPercentage"].AsFloat, jsonData[i]["slug"].Value, jsonData[i]["type"].Value));
		}
	}
}

public class Item
{
	public int ID { get; set; }
	public string Title { get; set; }
	public int Value { get; set; }
	public float Cooldown { get; set; }
	public float Damage { get; set; }
	public int BurstAmount {get; set; }
	public float MuzzleVelocity { get; set; }
	public bool Stackable { get; set; }
	public float DropPercentage { get; set; }
	public string Slug { get; set; }
	public Sprite Sprite { get; set; }
	public string Type {get; set; }
	public GameObject Prefab {get; set;}

	public Item(int id, string title, int value, float cooldown, float muzzleVelocity, int burstAmount, float damage, bool stackable, float dropPercentage, string slug, string type) 
	{
		this.ID = id;
		this.Title = title;
		this.Value = value;
		this.Cooldown = cooldown;
		this.Damage = damage;
		this.BurstAmount = burstAmount;
		this.MuzzleVelocity = muzzleVelocity;
		this.Stackable = stackable;
		this.DropPercentage = dropPercentage;
		this.Slug = slug;
		this.Sprite = Resources.Load<Sprite>("Icons/" + slug);
		this.Type = type;
		this.Prefab = Resources.Load<GameObject>("Weapons/" + slug + "_prefab");
	}

	public Item() 
	{
		this.ID = -1;
	}
}
