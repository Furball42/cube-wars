using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;


public class MapGenerator : MonoBehaviour {

	public Map[] maps;
	public int mapIndex;
	public Transform tilePrefab;
	public Transform obstaclePrefab;
	public Transform wallPrefab;

	List<Coord> allTileCoords;
	Queue<Coord> shuffledTileCoords;

	Map currentMap;

	void Start() {
		GenerateMap();
	}

	public void GenerateMap() {

		currentMap = maps[mapIndex];
		System.Random prng = new System.Random(currentMap.seed);
		GetComponent<BoxCollider>().size = new Vector3(currentMap.mapSize.x, 0.1f, currentMap.mapSize.y);

		//gen coords
		allTileCoords = new List<Coord>();
		for (int x = 0; x < currentMap.mapSize.x; x++) {
			for (int y = 0; y < currentMap.mapSize.y; y++) {
				allTileCoords.Add(new Coord(x, y));
			}
		}
		shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), currentMap.seed));

		//create holder
		string holderName = "Generated Map";
		if(transform.FindChild(holderName)) {
			DestroyImmediate(transform.FindChild(holderName).gameObject);
		}

		Transform mapHolder = new GameObject (holderName).transform;
		mapHolder.parent = transform;

		//spawn tiles
		for (int x = 0; x < currentMap.mapSize.x; x++) {
			for (int y = 0; y < currentMap.mapSize.y; y++) {
				Vector3 tilePos = CoordToPosition(x, y);
				Transform newTile = Instantiate(tilePrefab, tilePos, Quaternion.Euler(Vector3.right * 90)) as Transform;
				newTile.parent = mapHolder;
			}
		}

		//spawn obstacles
		bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];

		int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);
		int currentObstacleCount = 0;

		for (int i = 0; i < obstacleCount; i++) {
			Coord randomCoord = GetRandomCoord();
			obstacleMap[randomCoord.x, randomCoord.y] = true;
			currentObstacleCount++;

			if(CheckCoordAvailability(randomCoord) && MapIsFullyAccessible(obstacleMap, currentObstacleCount)) {
				float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float)prng.NextDouble());
				Vector3 obstaclePos = CoordToPosition(randomCoord.x, randomCoord.y);

				Transform newObstacle = Instantiate(obstaclePrefab, obstaclePos + Vector3.up * obstacleHeight / 2, Quaternion.identity) as Transform;
				newObstacle.parent = mapHolder;
				newObstacle.localScale = new Vector3(1, obstacleHeight, 1);

				Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
				Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
				obstacleMaterial.color = currentMap.obstacleColour;
				obstacleRenderer.sharedMaterial = obstacleMaterial;

			} else {
				obstacleMap[randomCoord.x, randomCoord.y] = false;
				currentObstacleCount--;
			}
		}

		//wall
		for (int x = 0; x < currentMap.mapSize.x; x++) {
			for (int y = 0; y < currentMap.mapSize.y; y++) {
				if (x == 0 || y == 0 || x == currentMap.mapSize.x - 1 || y == currentMap.mapSize.y - 1 && obstacleMap[x, y] == false)
				{
					Vector3 obstaclePos = CoordToPosition(x, y);

					Transform newObstacle = Instantiate(wallPrefab, obstaclePos + Vector3.up * 3 / 2, Quaternion.identity) as Transform;
					newObstacle.parent = mapHolder;
					newObstacle.localScale = new Vector3(1, 3, 1);

					Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
					Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
					obstacleMaterial.color = currentMap.obstacleColour;
					obstacleRenderer.sharedMaterial = obstacleMaterial;
				}
			}
		}

		//player spawn points
		SetSpawnPositions(mapHolder);

	}

	public bool CheckCoordAvailability(Coord randomCoord) {
		if(randomCoord != currentMap.mapCenter && randomCoord != currentMap.spawn1 && randomCoord != currentMap.spawn2 && randomCoord != currentMap.spawn3 && randomCoord != currentMap.spawn4)
			return true;
		else
			return false;
	}

	public Coord GetRandomCoord () {
		Coord randomCoord = shuffledTileCoords.Dequeue();
		shuffledTileCoords.Enqueue(randomCoord);

		return randomCoord;
	}

	bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount) {
		bool[,] mapFlags = new bool[obstacleMap.GetLength(0),obstacleMap.GetLength(1)];
		Queue<Coord> queue = new Queue<Coord>();
		queue.Enqueue(currentMap.mapCenter);
		mapFlags[currentMap.mapCenter.x, currentMap.mapCenter.y] = true;

		int accessibleTileCount = 1;

		while(queue.Count > 0) {
			Coord tile = queue.Dequeue();

			for (int x = -1; x <= 1; x++) {
				for (int y = -1; y <= 1; y++) {
					int neighbourX = tile.x + x;
					int neighbourY = tile.y + y;
					if(x == 0 || y == 0) {
						if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1)) {
							if(!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY]) {
								mapFlags[neighbourX, neighbourY] = true;
								queue.Enqueue(new Coord(neighbourX, neighbourY));
								accessibleTileCount++;
							}
						}
					}
				}
			}
		}

		int targetAccesibleTileCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount);
		return targetAccesibleTileCount == accessibleTileCount;
	}

	Vector3 CoordToPosition(int x, int y) {
		return new Vector3(-currentMap.mapSize.x / 2 + 0.5f + x, 0, -currentMap.mapSize.y / 2 + 0.5f + y);
	}

	Vector3 CoordToPositionSpawn(int x, int y) {
		return new Vector3(-currentMap.mapSize.x / 2 + 0.5f + x, 1, -currentMap.mapSize.y / 2 + 0.5f + y);
	}

	public void SetSpawnPositions(Transform mapHolder) {
		GameObject spawn1 = new GameObject();
		spawn1.name = "Spawn Point 1";
		spawn1.transform.position = CoordToPositionSpawn(currentMap.spawn1.x, currentMap.spawn1.y);
		spawn1.AddComponent<NetworkStartPosition>();
		spawn1.transform.SetParent(mapHolder);

		GameObject spawn2 = new GameObject();
		spawn2.name = "Spawn Point 2";
		spawn2.transform.position = CoordToPositionSpawn(currentMap.spawn2.x, currentMap.spawn2.y);
		spawn2.AddComponent<NetworkStartPosition>();
		spawn2.transform.SetParent(mapHolder);

		GameObject spawn3 = new GameObject();
		spawn3.name = "Spawn Point 3";
		spawn3.transform.position = CoordToPositionSpawn(currentMap.spawn3.x, currentMap.spawn3.y);
		spawn3.AddComponent<NetworkStartPosition>();
		spawn3.transform.SetParent(mapHolder);

		GameObject spawn4 = new GameObject();
		spawn4.name = "Spawn Point 4";
		spawn4.transform.position = CoordToPositionSpawn(currentMap.spawn4.x, currentMap.spawn4.y);
		spawn4.AddComponent<NetworkStartPosition>();
		spawn4.transform.SetParent(mapHolder);
	}

	[System.Serializable]
	public struct Coord {
		public int x;
		public int y;

		public Coord(int _x, int _y) {
			x = _x;
			y = _y;
		}

		public static bool operator == (Coord c1, Coord c2) {
			return c1.x == c2.x && c1.y == c2.y;
		}

		public static bool operator != (Coord c1, Coord c2) {
			return !(c1 == c2);
		}
	}

	[System.Serializable]
	public class Map {

		public Coord mapSize;
		[Range(0, 1)]
		public float obstaclePercent;
		public int seed;
		public float minObstacleHeight;
		public float maxObstacleHeight;
		public float maxOuterWallHeight;
		public Color obstacleColour;

		public Coord mapCenter {
			get {
				return new Coord(mapSize.x / 2, mapSize.y / 2);
			}
		}

		public Coord spawn1 {
			get {
				return new Coord(1, mapSize.y / 2);
			}
		}

		public Coord spawn2 {
			get {
				return new Coord(mapSize.x - 2, mapSize.y / 2);
			}
		}

		public Coord spawn3 {
			get {
				return new Coord(mapSize.x / 2, 1);
			}
		}

		public Coord spawn4 {
			get {
				return new Coord(mapSize.x / 2, mapSize.y - 2);
			}
		}

	}
}
