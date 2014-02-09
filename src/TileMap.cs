﻿/*!
 * LunaTMX: A tiled map editor file importer for Unity3d
 * https://github.com/suppayami/unity2d-tmx
 * 
 * © 2014, SuppaYami - Cuong Nguyen.
 * Released under the MIT license
 * Check LICENSE for more details.
 */

using UnityEngine;
using System.Collections;


namespace Symphony {
    public class TileMap : MonoBehaviour {

        public float     unit;
        public string    tileFolder; // Tilesets will be put in Resources/tileFolder
        public TextAsset tileXml;    // input xml file

        private TileData _tileData;  // TileData object

        public void Start() {
            this.RenderMap();
        }

        public void RenderMap() {
            if (this._tileData == null)
                this._tileData = new TileData(this.tileXml);
            this._tileData.Reload(this.tileXml);
            this._tileData.RenderMap(gameObject, this.tileFolder, this.unit);
        }

        public void ClearMap() {
            if (this._tileData != null)
                this._tileData.Clear();
        }

    }
}