using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Settings settings;

    public float cameraSpeed = 1;
    public float timeSpeed = 1;
    public float daylightCycleSpeed = 1;
    public int renderDistance;

    public Entity[] allEntities;
    public Item[] allItems;

    public Building[] allBuildings;
    public int selectedBuildingID = 16;

    public Transform player;
    public Transform pauseMenu;
    public Transform mainMenu;
    public Transform continueButton;
    public Transform sun;
    public Transform Ground;
    public Transform Indicator;

    public LayerMask groundMask;

    public int worldSize = 100;
    public PlacedBuilding[,] placedBuildings;
    public Pixel[] groundTexture;

    public List<Transform> loadedEntities = new List<Transform>();
    public List<Transform> loadedItems = new List<Transform>();
    public List<EntityMetaData> unloadedEntities = new List<EntityMetaData>();
    public List<ItemMetaData> unloadedItems = new List<ItemMetaData>();

    public static Vector2 mousePos = new Vector2();
    public static GameController ins;

    private void Awake()
    {
        if (ins == null)
        {
            ins = this;
        }

        groundTexture = new Pixel[worldSize * worldSize];
        placedBuildings = new PlacedBuilding[worldSize, worldSize];
    }

    private void Start()
    {
        player.position = new Vector3(worldSize / 2, 0, worldSize / 2);

        timeSpeed = 0;
        if (File.Exists(Application.persistentDataPath + "/world.xml"))
        {
            continueButton.gameObject.SetActive(true);
        }
    }

    public void GenerateWorldFromSaveFile(SaveFile saveFile)
    {
        timeSpeed = 1;
        settings = saveFile.settings;
        sun.rotation = saveFile.sunRotation;
        worldSize = saveFile.worldSize;
        Debug.Log("WORLDSIZE: " + worldSize);
        groundTexture = saveFile.groundTexture;
        Ground.localScale = new Vector3(worldSize, 1, worldSize);
        float blockOffset = worldSize % 2 == 0 ? -0.5f : 0;
        Ground.position = new Vector3(worldSize / 2 + blockOffset, -0.5f, worldSize / 2 + blockOffset);
        Ground.gameObject.GetComponent<Renderer>().material.mainTexture = GetGroundTextureFromPixelArray(saveFile.groundTexture);
        player.position = saveFile.playerPosition;
        Inventory.ins.items = saveFile.inventory.ToList();

        foreach (ItemMetaData i_meta in saveFile.itemData)
        {
            unloadedItems.Add(i_meta);
        }

        foreach (EntityMetaData e_meta in saveFile.entityData)
        {
            unloadedEntities.Add(e_meta);
        }

        placedBuildings = new PlacedBuilding[worldSize, worldSize];

        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                placedBuildings[x, y] = null;

                if (saveFile.buildingData[x * worldSize + y].isNull == false)
                {
                    //Debug.Log("Loading empty space: Placed " + saveFile.buildingData[x * worldSize + y].buildingID);
                    placedBuildings[x, y] = new PlacedBuilding();
                    //placedBuildings[x, y].transform = Instantiate(allBuildings[0].prefab, new Vector3(x, 0, y), Quaternion.identity).transform;
                    placedBuildings[x, y].meta = saveFile.buildingData[x * worldSize + y];
                }
            }
        }

        Debug.Log(CountBuildings() + " buildings placed in total");
    }

    public void SpawnEntity(EntityMetaData e_meta)
    {
        Transform entityObj = Instantiate(GetEntityByID(e_meta.entityID).prefab, e_meta.position, e_meta.rotation).transform;
        entityObj.gameObject.GetComponent<EntityBehaviour>().entity = GetEntityByID(e_meta.entityID);
        loadedEntities.Add(entityObj);
    }

    public void SpawnEntity(Entity entity, Vector2Int pos)
    {
        GameObject entityObj = Instantiate(entity.prefab, new Vector3(pos.x, 0, pos.y), GetRandomRotation());
        entityObj.gameObject.GetComponent<EntityBehaviour>().entity = entity;
        loadedEntities.Add(entityObj.transform);
    }

    public int CountBuildings()
    {
        int count = 0;

        foreach (PlacedBuilding p in placedBuildings)
        {
            if (p != null)
            {
                count++;
            }
        }

        return count;
    }

    public int GetEntityID(Entity entity)
    {
        int index = 0;

        foreach (Entity e in allEntities)
        {
            if (e == entity)
            {
                return index;
            }

            index++;
        }

        throw new MissingReferenceException("No entity with this ID");
    }

    public int GetItemID(Item item)
    {
        int index = 0;

        foreach (Item i in allItems)
        {
            if (i == item)
            {
                return index;
            }

            index++;
        }

        throw new MissingReferenceException("No item with this ID");
    }

    public Entity GetEntityByID(int id)
    {
        return allEntities[id];
    }

    public Item GetItemByID(int id)
    {
        return allItems[id];
    }

    public void GenerateNewWorld()
    {
        timeSpeed = 1;
        mainMenu.gameObject.SetActive(false);

        float blockOffset = worldSize % 2 == 0 ? -0.5f : 0;

        Ground.localScale = new Vector3(worldSize, 1, worldSize);
        Ground.position = new Vector3(worldSize / 2 + blockOffset, -0.5f, worldSize / 2 + blockOffset);
        Ground.gameObject.GetComponent<Renderer>().material.mainTexture = GenerateGroundTexture(10, 1, 0.9f, 1, 0.05f);

        GenerateEntitiesInWorld(allEntities[0], 0.3f);
        GenerateEntitiesInWorld(allEntities[1], 0.3f);

        GenerateBuildingsInWorldFromPerlinNoise(allBuildings[1], 0.55f, 30);//Random.Range(15,35)
        GenerateBuildingsInWorld(allBuildings[2], 5);
        GenerateBuildingsInWorld(allBuildings[15], 2);
        GenerateBuildingsInWorld(allBuildings[4], 20);

        GenerateBuildingsInWorld(allBuildings[5], 10);
        GenerateBuildingsInWorld(allBuildings[6], 5);
        GenerateBuildingsInWorld(allBuildings[7], 3);

        GenerateBuildingsInWorld(allBuildings[8], 10);
        GenerateBuildingsInWorld(allBuildings[9], 5);
        GenerateBuildingsInWorld(allBuildings[10], 3);

        GenerateBuildingsInWorld(allBuildings[11], 10);
        GenerateBuildingsInWorld(allBuildings[12], 10);
    }


    private void Update()
    {
        Time.timeScale = timeSpeed;

        if (mainMenu.gameObject.activeSelf)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveController.SaveGame();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePaus();
        }

        if (pauseMenu.gameObject.activeSelf)
        {
            return;
        }

        LoadItemsWithinRange(renderDistance);
        LoadEntitiesWithinRange(renderDistance);
        LoadBuildingsWithinRange(renderDistance);

        RaycastHit hit;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask))
        {
            mousePos = new Vector2(hit.point.x, hit.point.z);

            if (allBuildings[selectedBuildingID].fastPlacementOn)
            {
                if (Input.GetMouseButton(0))
                {
                    PlaceBuilding(allBuildings[selectedBuildingID], hit.point);
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    PlaceBuilding(allBuildings[selectedBuildingID], hit.point);
                }
            }

            Vector2Int gridPos = ToGridValue(hit.point);
            Indicator.transform.position = new Vector3(gridPos.x, 0.01f, gridPos.y);

            if (placedBuildings[gridPos.x, gridPos.y] != null)
            {
                if (GetBuildingByID(placedBuildings[gridPos.x, gridPos.y].meta.buildingID).fastPlacementOn)
                {
                    if (Input.GetMouseButton(1))
                    {
                        DestroyBuilding(hit.point);
                    }
                }
                else
                {
                    if (Input.GetMouseButtonDown(1))
                    {
                        DestroyBuilding(hit.point);
                    }
                }
            }
        }
        else
        {
            Indicator.transform.position = new Vector3(0,1000,0);
        }

        sun.Rotate(daylightCycleSpeed * Time.deltaTime, 0, 0);
    }

    public void TogglePaus()
    {
        pauseMenu.gameObject.SetActive(!pauseMenu.gameObject.activeSelf);
        timeSpeed = pauseMenu.gameObject.activeSelf ? 0 : 1;
    }

    public void SaveAndQuit()
    {
        SaveController.SaveGame();
        Application.Quit();
    }

    public void LoadBuildingsWithinRange(int range)
    {
        Vector2Int playerPos = ToGridValue(GameObject.FindGameObjectWithTag("Player").transform.position);

        int xMin = playerPos.x - range;
        int xMax = playerPos.x + range;

        int yMin = playerPos.y - range;
        int yMax = playerPos.y + range;

        for (int x = xMin; x < xMax; x++)
        {
            for (int y = yMin; y < yMax; y++)
            {
                if (x >= 0 && x <= worldSize - 1 && y >= 0 && y <= worldSize - 1)
                {
                    LoadBuildingOnCoords(new Vector2Int(x, y));
                }
            }
        }
    }

    public void LoadEntitiesWithinRange(int range)
    {
        EntityMetaData[] data = unloadedEntities.ToArray();

        foreach (EntityMetaData m in data)
        {
            if (Vector3.Distance(m.position, player.position) <= range)
            {
                unloadedEntities.Remove(m);
                SpawnEntity(m);
            }
        }
    }

    public void LoadItemsWithinRange(int range)
    {
        ItemMetaData[] data = unloadedItems.ToArray();

        foreach (ItemMetaData m in data)
        {
            if (Vector3.Distance(m.position, player.position) <= range)
            {
                unloadedItems.Remove(m);
                DropItem(m);
            }
        }
    }

    public void LoadBuildingOnCoords(Vector2Int gridPos)
    {
        if (placedBuildings[gridPos.x, gridPos.y] != null && placedBuildings[gridPos.x, gridPos.y].transform == null)
        {
            Debug.Log("105 it wasn't null");

            PlacedBuildingMetaData meta = placedBuildings[gridPos.x, gridPos.y].meta;

            Building building = GetBuildingByID(meta.buildingID);
            Debug.Log(meta.buildingID);

            //Random placement and scale
            GameObject buildingObject = Instantiate(building.prefab, new Vector3(gridPos.x + Random.Range(-building.randomCenterOffset, building.randomCenterOffset), 0, gridPos.y + Random.Range(-building.randomCenterOffset, building.randomCenterOffset)), Quaternion.identity);
            buildingObject.transform.localScale = new Vector3(buildingObject.transform.localScale.x, buildingObject.transform.localScale.y * (Random.Range(-building.randomLengthDifference, building.randomLengthDifference) + 1), buildingObject.transform.localScale.z);

            //Getting color
            Color currentColor = buildingObject.gameObject.GetComponentInChildren<Renderer>().material.color;
            Color offsetColor = GetOffsetColor(currentColor, meta.colorOffset);
            buildingObject.gameObject.GetComponentInChildren<Renderer>().material.color = offsetColor;
            placedBuildings[gridPos.x, gridPos.y].transform = buildingObject.transform;

            //Getting Rotation
            buildingObject.transform.rotation = meta.rotation;

            //Setting Building
            try
            {
                buildingObject.GetComponent<BuildingBehaviour>().building = building;
            }
            catch
            {
                buildingObject.AddComponent<BuildingBehaviour>().building = building;
            }

            //Add necessary components
            if (building.GetType() == typeof(GrowingBuilding))
            {
                //Debug.Log("IS Growing");
                buildingObject.AddComponent<Growing>();
            }
        }
        else
        {
            //Debug.Log("105 it was null");
        }
    }

    public Vector3 GetRandomColorOffset(float randomColorOffset)
    {
        return new Vector3(Random.Range(-randomColorOffset, randomColorOffset), Random.Range(-randomColorOffset, randomColorOffset), Random.Range(-randomColorOffset, randomColorOffset));
    }

    public Color GetOffsetColor(Color color, Vector3 offset)
    {
        return new Color(Mathf.Clamp(color.r + offset.x, 0, 1), Mathf.Clamp(color.g + offset.y, 0, 1), Mathf.Clamp(color.b + offset.z, 0, 1), 1);
    }

    public Vector2Int ToGridValue(Vector3 pos)
    {
        return new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z));
    }

    public Texture2D GetGroundTextureFromPixelArray(Pixel[] pixelArray)
    {
        Texture2D texture = new Texture2D(worldSize, worldSize);
        Color[] pixels = new Color[worldSize * worldSize];

        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                Pixel pixel = pixelArray[y * texture.width + x];

                Color color = new Color(pixel.r, pixel.g, pixel.b);
                pixels[y * texture.width + x] = color;
            }
        }

        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Point;
        texture.SetPixels(pixels);
        texture.Apply();

        return texture;
    }

    public Texture2D GenerateGroundTexture(float size, float multiplier = 1, float minColor = 0, float maxColor = 1, float randomColorByPixelOffset = 0)
    {
        Texture2D texture = new Texture2D(worldSize, worldSize);
        Color[] pixels = new Color[worldSize * worldSize];
        Vector2 offset = new Vector2(Random.Range(0, 1000000), Random.Range(0, 1000000));

        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                float xCoord = (float)offset.x + ((float)x / (float)worldSize * (float)size);
                float yCoord = (float)offset.y + ((float)y / (float)worldSize * (float)size);
                float sample = Mathf.Clamp(Mathf.PerlinNoise(xCoord, yCoord) * multiplier, minColor, maxColor);
                //Debug.Log(sample);

                Color color = GetOffsetColor(new Color(sample, sample, sample), GetRandomColorOffset(randomColorByPixelOffset));
                pixels[y * texture.width + x] = color;
            }
        }

        int index = 0;
        foreach (Color c in pixels)
        {
            groundTexture[index] = new Pixel();
            groundTexture[index].r = c.r;
            groundTexture[index].g = c.g;
            groundTexture[index].b = c.b;
            index++;
        }

        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Point;
        texture.SetPixels(pixels);
        texture.Apply();

        return texture;
    }

    public void GenerateBuildingsInWorld(Building building, float percentage)
    {
        Debug.Log("Started Counting");

        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                int rnd = Random.Range(0,101);

                if (rnd <= percentage)
                {
                    PlaceBuilding(building, new Vector3(x, 0, y));
                } 
            }
        }
    }

    public void GenerateEntitiesInWorld(Entity entity, float percentage)
    {
        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                float rnd = Random.Range(0.0f, 100.0f);

                if (rnd <= percentage)
                {
                    SpawnEntity(entity, new Vector2Int(x,y));
                }
            }
        }
    }

    public void DropItems(Building building, Vector2Int gridPos)
    {
        Debug.Log("DROP: Dropping item");

        foreach (Drop drop in building.drops)
        {
            int amount = Random.Range(drop.minAmount, drop.maxAmount);
            Debug.Log("DROP: " + amount);

            for (int i = 0; i < amount; i++)
            {
                DropItem(drop.item, gridPos);
            }
        }
    }

    public void DropItem(Item item, Vector2Int gridPos, bool pickupable = true)
    {
        Debug.Log("DROP: Dropped " + item.itemName + " at position " + gridPos.ToString());
        GameObject itemInstance = Instantiate(item.itemPrefab, new Vector3(gridPos.x, 1, gridPos.y), Quaternion.Euler(Random.Range(0, 359), Random.Range(0, 359), Random.Range(0, 359)));

        ItemBehaviour behaviour = null;
        Rigidbody rigidbody = null;

        if (itemInstance.GetComponent<ItemBehaviour>())
        {
            behaviour = itemInstance.GetComponent<ItemBehaviour>();
        }
        else
        {
            behaviour = itemInstance.AddComponent<ItemBehaviour>();
        }

        if (itemInstance.GetComponent<Rigidbody>())
        {
            rigidbody = itemInstance.GetComponent<Rigidbody>();
        }
        else
        {
            rigidbody = itemInstance.AddComponent<Rigidbody>();
        }

        itemInstance.gameObject.tag = "item";

        behaviour.pickupable = pickupable;
        behaviour.item = item;
        loadedItems.Add(itemInstance.transform);
    }

    public void DropItem(ItemMetaData meta)
    {
        Item item = GetItemByID(meta.itemID);

        GameObject itemInstance = Instantiate(item.itemPrefab, meta.position, meta.rotation);

        ItemBehaviour behaviour = null;
        Rigidbody rigidbody = null;

        if (itemInstance.GetComponent<ItemBehaviour>())
        {
            behaviour = itemInstance.GetComponent<ItemBehaviour>();
        }
        else
        {
            behaviour = itemInstance.AddComponent<ItemBehaviour>();
        }

        if (itemInstance.GetComponent<Rigidbody>())
        {
            rigidbody = itemInstance.GetComponent<Rigidbody>();
        }
        else
        {
            rigidbody = itemInstance.AddComponent<Rigidbody>();
        }

        itemInstance.gameObject.tag = "item";
        behaviour.item = item;
        loadedItems.Add(itemInstance.transform);
    }

    public void GenerateBuildingsInWorldFromPerlinNoise(Building building, float percentage, float size = 10)
    {
        Vector2 offset = new Vector2(Random.Range(0, 1000000), Random.Range(0, 1000000));

        Debug.Log("Started Counting Perlin");

        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                float xCoord = (float)offset.x + ((float)x /100 * (float)size);
                float yCoord = (float)offset.y + ((float)y /100 * (float)size);
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                //Debug.Log(sample);

                if (sample >= percentage)
                {
                    PlaceBuilding(building, new Vector3(x, 0, y));
                }
            }
        }
    }

    public void DestroyBuilding(Vector2Int gridPos)
    {
        Building building = GetBuildingByID(placedBuildings[gridPos.x, gridPos.y].meta.buildingID);

        AudioController.PlaySound(2);
        DropItems(building, gridPos);
        if (placedBuildings[gridPos.x, gridPos.y] != null && placedBuildings[gridPos.x, gridPos.y].transform != null) Destroy(placedBuildings[gridPos.x, gridPos.y].transform.gameObject);
        placedBuildings[gridPos.x, gridPos.y] = null;
    }

    public void DestroyBuilding(Vector3 position)
    {
        DestroyBuilding(ToGridValue(position));
    }

    public void PlaceBuilding(Building building, Vector3 position, bool instantiate = false, bool destroy = false)
    {
        Vector2Int gridPos = ToGridValue(position);

        if(placedBuildings[gridPos.x, gridPos.y] != null && placedBuildings[gridPos.x, gridPos.y].transform != null)
        {
            if (GetBuildingByID(placedBuildings[gridPos.x, gridPos.y].meta.buildingID).destroyWhenPlacedOn)
            {
                DropItems(GetBuildingByID(placedBuildings[gridPos.x, gridPos.y].meta.buildingID), gridPos);
                destroy = true;
            }
        }

        if (destroy)
        {
            if(placedBuildings[gridPos.x, gridPos.y] != null && placedBuildings[gridPos.x, gridPos.y].transform != null) Destroy(placedBuildings[gridPos.x, gridPos.y].transform.gameObject);
            placedBuildings[gridPos.x, gridPos.y] = null;
        }
        else if (placedBuildings[gridPos.x, gridPos.y] != null)
        {
            return;
        }

        AudioController.PlaySound(1);

        placedBuildings[gridPos.x, gridPos.y] = new PlacedBuilding();
        PlacedBuildingMetaData metaData = new PlacedBuildingMetaData();

        //Random placement and scale
        Vector3 newPos = new Vector3(gridPos.x + Random.Range(-building.randomCenterOffset, building.randomCenterOffset), 0, gridPos.y + Random.Range(-building.randomCenterOffset, building.randomCenterOffset));
        Vector3 newScale = new Vector3(building.prefab.transform.localScale.x, building.prefab.transform.localScale.y * (Random.Range(-building.randomLengthDifference, building.randomLengthDifference) + 1), building.prefab.transform.localScale.z);

        metaData.position = newPos;
        metaData.scale = newScale;

        //Random color
        Vector3 colorOffset = GetRandomColorOffset(building.randomColorOffset);
        metaData.colorOffset = colorOffset;

        //Random Rotation
        Quaternion rot = Quaternion.identity;

        if (building.rotationType == RotationType.FullyRandom)
        {
            rot = GetRandomRotation();
            metaData.rotation = rot;
        }
        else if (building.rotationType == RotationType.RestrictedRandom)
        {
            rot = GetRestrictedRandomRotation();
            metaData.rotation = rot;
        }

        placedBuildings[gridPos.x, gridPos.y].meta = metaData;

        metaData.buildingID = GetBuildingID(building);

        if (instantiate)
        {
            GameObject buildingObject = Instantiate(building.prefab, newPos, Quaternion.identity);
            buildingObject.transform.localScale = newScale;

            Color currentColor = buildingObject.gameObject.GetComponentInChildren<Renderer>().material.color;
            Color offsetColor = GetOffsetColor(currentColor, colorOffset);
            buildingObject.gameObject.GetComponentInChildren<Renderer>().material.color = offsetColor;
            placedBuildings[gridPos.x, gridPos.y].transform = buildingObject.transform;

            buildingObject.transform.rotation = rot;

            //Setting Building
            try
            {
                buildingObject.GetComponent<BuildingBehaviour>().building = building;
            }
            catch
            {
                buildingObject.AddComponent<BuildingBehaviour>().building = building;
            }

            //Add necessary components
            if (building.GetType() == typeof(GrowingBuilding))
            {
                Debug.Log("IS Growing");
                buildingObject.AddComponent<Growing>();
            }
        }
    }

    public Building GetBuildingByID(int id)
    {
        return allBuildings[id];
    }

    public int GetBuildingID(Building building)
    {
        int index = 0;

        foreach (Building b in allBuildings)
        {
            Debug.Log(b.buildingName);
            if (b == building)
            {
                return index;
            }
            index++;
        }

        throw new MissingReferenceException("No building with this ID exists");
    }

    public void ContinueFromSaveFile()
    {
        mainMenu.gameObject.SetActive(false);

        SaveController.LoadFromSaveFile();
    }

    public bool DoHaveSameContent(string[] list1, string[] list2)
    {
        foreach (string s in list1)
        {
            if (list2.ToList().Contains(s))
            {
                return true;
            }
        }

        return false;
    }

    public string[] TagsInSurroundingBuildings(Vector2Int position)
    {
        HashSet<string> tags = GetTagsFromSurroundingBuilding(position, new Vector2Int(1, 0));
        tags.UnionWith(GetTagsFromSurroundingBuilding(position, new Vector2Int(-1, 0)));
        tags.UnionWith(GetTagsFromSurroundingBuilding(position, new Vector2Int(0, 1)));
        tags.UnionWith(GetTagsFromSurroundingBuilding(position, new Vector2Int(0, -1)));

        return tags.ToArray();
    }

    HashSet<string> GetTagsFromSurroundingBuilding(Vector2Int position, Vector2Int offset)
    {
        HashSet<string> tags = new HashSet<string>();

        if (position.x + offset.x > worldSize - 1 || position.x + offset.x < 0)
        {
            return new HashSet<string>();
        }

        if (position.y + offset.y > worldSize - 1 || position.y + offset.y < 0)
        {
            return new HashSet<string>();
        }

        if (placedBuildings[position.x + offset.x, position.y + offset.y] != null && GameController.ins.GetBuildingByID(placedBuildings[position.x + offset.x, position.y + offset.y].meta.buildingID).tags.Length > 0)
        {
            foreach (string t in GameController.ins.GetBuildingByID(placedBuildings[position.x + offset.x, position.y + offset.y].meta.buildingID).tags)
            {
                tags.Add(t);
            }
        }

        return tags;
    }

    public Quaternion GetRandomRotation()
    {
        return Quaternion.Euler(0, Random.Range(0,359), 0);
    }

    public Quaternion GetRestrictedRandomRotation()
    {
        int rnd = Random.Range(0,4);

        switch (rnd)
        {
            case 0:
                return Quaternion.Euler(0, 0, 0);
            case 1:
                return Quaternion.Euler(0, 90, 0);
            case 2:
                return Quaternion.Euler(0, 180, 0);
            case 3:
                return Quaternion.Euler(0, 270, 0);
            default:
                return Quaternion.Euler(0, 0, 0);
        }
    }
}
