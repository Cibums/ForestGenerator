using System;
using System.Collections.Generic;
using UnityEngine;

public static class SaveController
{
    private static PlacedBuildingMetaData[] GetBuildingMetaData()
    {
        PlacedBuildingMetaData[] data = new PlacedBuildingMetaData[GameController.ins.worldSize * GameController.ins.worldSize];

        for (int x = 0; x < GameController.ins.worldSize; x++)
        {
            for (int y = 0; y < GameController.ins.worldSize; y++)
            {
                if (GameController.ins.placedBuildings[x, y] == null)
                {
                    data[x * GameController.ins.worldSize + y] = new PlacedBuildingMetaData();
                    data[x * GameController.ins.worldSize + y].isNull = true;
                }
                else
                {
                    data[x * GameController.ins.worldSize + y] = GameController.ins.placedBuildings[x, y].meta;
                }
            }
        }

        return data;
    }

    private static ItemMetaData[] GetItemMetaData()
    {
        List<ItemMetaData> data = new List<ItemMetaData>();

        foreach (Transform i in GameController.ins.loadedItems)
        {
            ItemMetaData itemData = new ItemMetaData();
            itemData.position = i.position;
            itemData.rotation = i.rotation;
            itemData.itemID = GameController.ins.GetItemID(i.gameObject.GetComponent<ItemBehaviour>().item);

            data.Add(itemData);
        }

        foreach (ItemMetaData i_data in GameController.ins.unloadedItems)
        {
            data.Add(i_data);
        }

        return data.ToArray();
    }

    private static EntityMetaData[] GetEntityMetaData()
    {
        List<EntityMetaData> data = new List<EntityMetaData>();

        foreach (Transform e in GameController.ins.loadedEntities)
        {
            EntityMetaData eData = new EntityMetaData();
            eData.position = e.position;
            eData.rotation = e.rotation;
            eData.entityID = GameController.ins.GetEntityID(e.gameObject.GetComponent<EntityBehaviour>().entity);

            data.Add(eData);
        }

        foreach (EntityMetaData e_data in GameController.ins.unloadedEntities)
        {
            data.Add(e_data);
        }

        return data.ToArray();
    }

    public static void SaveGame()
    {
        SaveFile saveFile = new SaveFile();
        saveFile.settings = GameController.ins.settings;
        saveFile.sunRotation = GameController.ins.sun.rotation;
        saveFile.worldSize = GameController.ins.worldSize;
        saveFile.playerPosition = GameController.ins.player.position;
        saveFile.groundTexture = GameController.ins.groundTexture;
        saveFile.buildingData = GetBuildingMetaData();
        saveFile.entityData = GetEntityMetaData();
        saveFile.itemData = GetItemMetaData();
        saveFile.inventory = Inventory.ins.items.ToArray();

        System.Xml.Serialization.XmlSerializer writer =
            new System.Xml.Serialization.XmlSerializer(typeof(SaveFile));

        var path = Application.persistentDataPath + "/world.xml";
        System.IO.FileStream file = System.IO.File.Create(path);

        writer.Serialize(file, saveFile);
        file.Close();
    }

    public static void LoadFromSaveFile()
    {
        System.Xml.Serialization.XmlSerializer reader =
            new System.Xml.Serialization.XmlSerializer(typeof(SaveFile));
        System.IO.StreamReader file = new System.IO.StreamReader(
            Application.persistentDataPath + "/world.xml");
        SaveFile saveFile = (SaveFile)reader.Deserialize(file);
        file.Close();

        GameController.ins.GenerateWorldFromSaveFile(saveFile);
    }

}
