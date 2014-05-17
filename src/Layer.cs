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


namespace Symphony {
    public class Layer {

        private int _layerWidth;
        private int _layerHeight;
        private int _tileWidth;
        private int _tileHeight;

        private string[]    _layerData;
        private XmlNodeList _objectData;
        private string      _name;

        private bool _collision;

        public GameObject layerObject;
        public Texture2D  layerTexture;

        static private Color _alpha = new Color(0.0f, 0.0f, 0.0f, 0.0f);

        public Layer(int layerWidth, int layerHeight,
                     int tileWidth, int tileHeight, string csvData, string name) {
            this._layerWidth  = layerWidth;
            this._layerHeight = layerHeight;
            this._tileWidth   = tileWidth;
            this._tileHeight  = tileHeight;
            this._layerData   = csvData.Split(',');
            this._name        = name;

            if (this._name.Substring(0, 3).ToUpper() == "[C]")
                this._collision = true;
        }

        public Layer(int layerWidth, int layerHeight,
                     int tileWidth, int tileHeight, XmlNodeList objectData, string name) {
            this._layerWidth  = layerWidth;
            this._layerHeight = layerHeight;
            this._tileWidth   = tileWidth;
            this._tileHeight  = tileHeight;
            this._objectData  = objectData;
            this._name        = name;

            if (this._name.Substring(0, 3).ToUpper() == "[C]")
                this._collision = true;
        }

        public void RenderLayer(GameObject parent, List<TileSet> tilesets, float unit) {
            this._RenderTexture(tilesets);
            this._RenderSprite(parent, unit);
        }

        public void ClearLayer() {
            if (this.layerObject)
                Object.DestroyImmediate(this.layerObject);
            if (this.layerTexture)
                Object.DestroyImmediate(this.layerTexture);

            this.layerTexture = null;
            this.layerObject  = null;
        }

        private void _RenderTexture(List<TileSet> tilesets) {
            int textureWidth  = this._layerWidth * this._tileWidth;
            int textureHeight = this._layerHeight * this._tileHeight;

            this.layerTexture = new Texture2D(textureWidth, textureHeight, 
                                              TextureFormat.ARGB32, false, false);
            this.layerTexture.filterMode = FilterMode.Point;
            this.layerTexture.wrapMode   = TextureWrapMode.Clamp;

            if (this._layerData != null)
                this._RenderTiles(tilesets);
            else if(this._objectData != null)
                this._RenderObjects(tilesets);

            this.layerTexture.Apply(false);
            this.layerTexture.Compress(true);
        }

        private void _RenderTiles(List<TileSet> tilesets) {
            int totalTiles    = this._layerWidth * this._layerHeight;

            Color[] alphaLayer = new Color[this._tileWidth * this._tileHeight];
            for (int p = 0; p < this._tileWidth * this._tileHeight; p++)
                alphaLayer[p] = Layer._alpha;

            for (int i = 0; i < totalTiles; i++) {
                int mapX = i % this._layerWidth;
                int mapY = this._layerHeight - Mathf.FloorToInt(i / this._layerWidth) - 1;
                mapX = mapX * this._tileWidth;
                mapY = mapY * this._tileHeight;
                
                int tileId = int.Parse(this._layerData[i].ToString().Trim());
                
                if (tileId == 0) {
                    this.layerTexture.SetPixels(mapX, mapY, 
                                                this._tileWidth, this._tileHeight, alphaLayer);
                } else {
                    foreach (TileSet tileset in tilesets) {
                        if (tileset.IsTileset(tileId)) {
                            Color[] pixels = tileset.GetTilePixels(tileId);
                            this.layerTexture.SetPixels(mapX, mapY, 
                                                        tileset._tileWidth, tileset._tileHeight, pixels);
                            break;
                        } // if tileset
                    } // foreach tileset
                } // check tileId valid
            } // for totalTiles
        }

        private void _RenderObjects(List<TileSet> tilesets) {
            int totalTiles    = this._layerWidth * this._layerHeight;

            Color[] alphaLayer = new Color[this._tileWidth * this._tileHeight];
            for (int p = 0; p < this._tileWidth * this._tileHeight; p++)
                alphaLayer[p] = Layer._alpha;
            
            for (int i = 0; i < totalTiles; i++) {
                int mapX = i % this._layerWidth;
                int mapY = this._layerHeight - Mathf.FloorToInt(i / this._layerWidth) - 1;
                mapX = mapX * this._tileWidth;
                mapY = mapY * this._tileHeight;
                
                this.layerTexture.SetPixels(mapX, mapY, 
                                            this._tileWidth, this._tileHeight, alphaLayer);
            }

            foreach (XmlNode nodeData in this._objectData) {
                int tileId = int.Parse(nodeData.Attributes["gid"].InnerText);
                int mapX   = int.Parse(nodeData.Attributes["x"].InnerText);
                int mapY   = int.Parse(nodeData.Attributes["y"].InnerText);

                int textureHeight = this._layerHeight * this._tileHeight;

                mapY = textureHeight - mapY - 1;

                foreach (TileSet tileset in tilesets) {
                    if (tileset.IsTileset(tileId)) {
                        Color[] pixels = tileset.GetTilePixels(tileId);
                        this.layerTexture.SetPixels(mapX, mapY, 
                                                    tileset._tileWidth, tileset._tileHeight, pixels);
                        break;
                    } // if tileset
                } // foreach tileset
            } // foreach object node
        }

        private void _RenderSprite(GameObject parent, float unit) {
            this.layerObject = new GameObject();
            this.layerObject.name = this._name;
            this.layerObject.transform.parent = parent.transform;

            Vector3 pos = parent.transform.position;
            pos.z -= parent.transform.childCount;
            this.layerObject.transform.position = pos;

            SpriteRenderer render = this.layerObject.GetComponent<SpriteRenderer>();
            if (render) 
                Object.Destroy(render);

            float textureWidth  = (float) this._layerWidth * this._tileWidth;
            float textureHeight = (float) this._layerHeight * this._tileHeight;

            Rect rect     = new Rect(0.0f, 0.0f, textureWidth, textureHeight);
            Vector2 pivot = new Vector2(0.0f, 0.0f);

            render = this.layerObject.AddComponent<SpriteRenderer>();
            render.sprite = Sprite.Create(this.layerTexture, rect, pivot, unit);

            PolygonCollider2D collider = this.layerObject.GetComponent<PolygonCollider2D>();
            if (collider) 
                Object.Destroy(collider);

            if (this._collision)
                collider = this.layerObject.AddComponent<PolygonCollider2D>();
        }

    } // class Layer
} // namespace Symphony