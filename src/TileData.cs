/*!
 * LunaTMX: A tiled map editor file importer for Unity3d
 * https://github.com/suppayami/unity2d-tmx
 * 
 * © 2014, SuppaYami - Cuong Nguyen.
 * Released under the MIT license
 * Check LICENSE for more details.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;


namespace Symphony {
    public class TileData {

        private List<TileSet> _tileSets;
        private List<Layer>   _layersList;
        private TextAsset     _tileXml;

        private int _mapWidth;
        private int _mapHeight;
        private int _tileWidth;
        private int _tileHeight;

        public TileData(TextAsset tileXml) {
            this.Reload(tileXml);
        }

        public void Reload(TextAsset tileXml) {
            this._Clear();
            this._InitProperties();
            this._Reload(tileXml);
            this._ParseData();
        }

        public void Clear() {
            this._Clear();
        }

        public void RenderMap(GameObject parent, string relativePath, float unit) {
            this._LoadTilesets(relativePath);
            this._RenderLayers(parent, unit);
        }

        private void _RenderLayers(GameObject parent, float unit) {
            foreach (Layer layer in this._layersList) {
                layer.RenderLayer(parent, this._tileSets, unit);
            }
        }

        private void _LoadTilesets(string relativePath) {
            foreach (TileSet tileset in this._tileSets) {
                tileset.LoadTexture(relativePath);
            }
        }

        private void _InitProperties() {
            this._tileSets   = new List<TileSet>();
            this._layersList = new List<Layer>();
        }

        private void _Reload(TextAsset tileXml) {
            this._tileXml = tileXml;
        }

        private void _Clear() {
            if (this._layersList != null)
                foreach (Layer layer in this._layersList)
                    layer.ClearLayer();
            this._tileXml    = null;
            this._tileSets   = null;
            this._layersList = null;
        }

        private void _ParseData() {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(_tileXml.text);
            XmlNodeList nodeList = xmlDoc.DocumentElement.ChildNodes;

            // get mapWidth, mapHeight, tileWidth, tileHeight
            XmlNode mapNode = xmlDoc.DocumentElement;
            this._mapWidth   = int.Parse(mapNode.Attributes["width"].InnerText);
            this._mapHeight  = int.Parse(mapNode.Attributes["height"].InnerText);
            this._tileWidth  = int.Parse(mapNode.Attributes["tilewidth"].InnerText);
            this._tileHeight = int.Parse(mapNode.Attributes["tileheight"].InnerText);
            
            foreach (XmlNode nodeData in nodeList) {
                // if node is tileset
                if (nodeData.Name == "tileset") {
                    // gets tileset parameters: firstgid, tile sizes, source name, source sizes
                    XmlNode source = nodeData.SelectSingleNode("image");

                    int gid        = int.Parse(nodeData.Attributes["firstgid"].InnerText);
                    int tileWidth  = int.Parse(nodeData.Attributes["tilewidth"].InnerText);
                    int tileHeight = int.Parse(nodeData.Attributes["tileheight"].InnerText);

                    int sourceWidth  = int.Parse(source.Attributes["width"].InnerText);
                    int sourceHeight = int.Parse(source.Attributes["height"].InnerText);
                    string filename  = source.Attributes["source"].InnerText;

                    filename = Path.GetFileNameWithoutExtension(filename); // gets filename only, remove relative path

                    TileSet tileset = new TileSet(gid, tileWidth, tileHeight, 
                                                  sourceWidth, sourceHeight, filename);

                    this._tileSets.Add(tileset);
                } // endif tileset

                // if node is layer
                if (nodeData.Name == "layer") {
                    XmlNode data    = nodeData.SelectSingleNode("data");
                    string  csvData = data.InnerText;

                    int layerWidth  = this._mapWidth;
                    int layerHeight = this._mapHeight;
                    int tileWidth   = this._tileWidth;
                    int tileHeight  = this._tileHeight;

                    string name    = nodeData.Attributes["name"].InnerText;

                    if (nodeData.Attributes["visible"] != null 
                        && nodeData.Attributes["visible"].InnerText == "0")
                        continue;

                    Layer layer = new Layer(layerWidth, layerHeight,
                                            tileWidth, tileHeight, csvData, name);

                    this._layersList.Add(layer);
                } // endif layer

                // if node is objectgroup
                if (nodeData.Name == "objectgroup") {
                    XmlNodeList data    = nodeData.ChildNodes;
                    
                    int layerWidth  = this._mapWidth;
                    int layerHeight = this._mapHeight;
                    int tileWidth   = this._tileWidth;
                    int tileHeight  = this._tileHeight;
                    
                    string name    = nodeData.Attributes["name"].InnerText;
                    
                    if (nodeData.Attributes["visible"] != null 
                        && nodeData.Attributes["visible"].InnerText == "0")
                        return;
                    
                    Layer layer = new Layer(layerWidth, layerHeight,
                                            tileWidth, tileHeight, data, name);
                    
                    this._layersList.Add(layer);
                } // endif layer
            } // end foreach node
        }

    } // class TileData
} // namespace Symphony