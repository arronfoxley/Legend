using FGame.Objects;
using Legend.Core.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;

namespace Legend.Core.Data {

    //TODO XMLParser
    public class XMLParser {

        public static GridData ParseGridXML(string xmlURL)
        {

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlURL);

            XmlNode gridNode = xmlDocument.DocumentElement;

            GridData gridConfigData = new GridData();

            gridConfigData.cellsX = int.Parse(gridNode.Attributes.GetNamedItem("CellsX").InnerText);
            gridConfigData.cellsY = int.Parse(gridNode.Attributes.GetNamedItem("CellsY").InnerText);
            gridConfigData.cellSize = int.Parse(gridNode.Attributes.GetNamedItem("CellSize").InnerText);

            return gridConfigData;

        }

        public static List<MapData> ParseMapXML(string xmlURL)
        {

            List<MapData> mapData = new List<MapData>();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlURL);

            XmlNode root = xmlDocument.DocumentElement;
            XmlNodeList nodeList = root.ChildNodes;

            foreach (XmlNode parentNode in nodeList)
            {

                MapData md = new MapData();

                md.textureId = int.Parse(parentNode.Attributes.GetNamedItem("TextureID").InnerText);
                md.cellX = int.Parse(parentNode.Attributes.GetNamedItem("X").InnerText);
                md.cellY = int.Parse(parentNode.Attributes.GetNamedItem("Y").InnerText);

                mapData.Add(md);

            }

            return mapData;

        }

        public static Dictionary<int, TextureData> ParseTexturesXML(string xmlURL)
        {

            Dictionary<int, TextureData> textures = new Dictionary<int, TextureData>();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlURL);

            XmlNode root = xmlDocument.DocumentElement;
            XmlNodeList nodeList = root.ChildNodes;

            foreach (XmlNode parentNode in nodeList)
            {

                TextureData textureData = new TextureData();

                textureData.id = int.Parse(parentNode.Attributes.GetNamedItem("ID").InnerText);
                textureData.textureName = parentNode.Attributes.GetNamedItem("TextureName").InnerText;
                textureData.textureName = parentNode.Attributes.GetNamedItem("TexturePath").InnerText;

                textures.Add(textureData.id, textureData);

            }

            return textures;

        }

        public static List<TerrainCellData> ParseTerrainCellXML(string xmlURL)
        {

            List<TerrainCellData> terrainData = new List<TerrainCellData>();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlURL);

            XmlNode root = xmlDocument.DocumentElement;
            XmlNodeList nodeList = root.ChildNodes;

            foreach (XmlNode parentNode in nodeList)
            {

                foreach (XmlNode childNode in parentNode.ChildNodes)
                {

                    TerrainCellData td = new TerrainCellData();

                    td.cellX = int.Parse(childNode.Attributes.GetNamedItem("CellX").InnerText);
                    td.cellY = int.Parse(childNode.Attributes.GetNamedItem("CellY").InnerText);
                    td.type = childNode.Attributes.GetNamedItem("Type").InnerText;
                    td.textureID = int.Parse(childNode.Attributes.GetNamedItem("TextureID").InnerText);

                    //Check if node isn't passable exists in XML
                    if (childNode.Attributes.GetNamedItem("IsPassable") != null)
                    {

                        td.isPassable = Boolean.Parse(childNode.Attributes.GetNamedItem("IsPassable").InnerText);

                    }
                    //Check if node isn't passable exists in XML
                    if (childNode.Attributes.GetNamedItem("ShadowTextureID") != null)
                    {

                        Debug.WriteLine("Has shadow");
                        td.shadowTextureID = int.Parse(childNode.Attributes.GetNamedItem("ShadowTextureID").InnerText);

                    }

                    terrainData.Add(td);

                }

            }

            return terrainData;

        }

        public static List<UnitData> ParseUnitXML(string xmlURL)
        {

            List<UnitData> textureData = new List<UnitData>();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlURL);

            XmlNode root = xmlDocument.DocumentElement;
            XmlNodeList nodeList = root.ChildNodes;

            foreach (XmlNode childNode in nodeList)
            {

                    UnitData unitData = new UnitData();

                    unitData.type = childNode.Attributes.GetNamedItem("Type").InnerText;
                    unitData.textureId = int.Parse(childNode.Attributes.GetNamedItem("TextureID").InnerText);

                    textureData.Add(unitData);


            }

            return textureData;

        }

    }

}
