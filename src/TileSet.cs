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


namespace Symphony {
    public class TileSet {
        
        public int  _tileWidth  {get; private set;}
        public int  _tileHeight {get; private set;}

        private int _firstId; // firstgid

        private int    _sourceWidth;
        private int    _sourceHeight;
        private string _filename;

        private Texture2D _sourceTexture;
        private Color[]   _lastTile;
        private int       _lastTileX = -1;
        private int       _lastTileY = -1;

        public TileSet(int firstId, int tileWidth, int tileHeight, 
                       int sourceWidth, int sourceHeight, string filename) {
            this._firstId    = firstId;
            this._tileWidth  = tileWidth;
            this._tileHeight = tileHeight;

            this._sourceWidth  = sourceWidth;
            this._sourceHeight = sourceHeight;
            this._filename     = filename;
        }

        public void LoadTexture(string relativePath) {
            string resPath;
            if (relativePath == "") {
                resPath = this._filename;
            } else {
                resPath = relativePath + "/" + this._filename;
            }
            this._sourceTexture = Resources.Load<Texture2D>(resPath);
        }

        public Color[] GetTilePixels(int tileId) {
            int realId   = tileId - this._firstId;
            int tmxTileY = Mathf.FloorToInt(realId / this._CountTilesHorizontal());

            int tileX = realId % this._CountTilesHorizontal();
            int tileY = this._CountTilesVertical() - tmxTileY - 1;

            tileX = tileX * this._tileWidth;
            tileY = tileY * this._tileHeight;

            if (this._lastTileX != tileX || this._lastTileY != tileY) {
                this._lastTile = this._sourceTexture.GetPixels(tileX, tileY, 
                                                               this._tileWidth, this._tileHeight);
                this._lastTileX = tileX;
                this._lastTileY = tileY;
            }

            return this._lastTile;
        }

        public bool IsTileset(int tileId) {
            int totalTiles = this._CountTilesVertical() * this._CountTilesHorizontal();
            int lastId     = this._firstId + totalTiles;
            return tileId >= this._firstId && tileId < lastId;
        }

        private int _CountTilesHorizontal() {
            return Mathf.FloorToInt(this._sourceWidth / this._tileWidth);
        }
        
        private int _CountTilesVertical() {
            return Mathf.FloorToInt(this._sourceHeight / this._tileHeight);
        }

    } // class TileSet
} // namespace Symphony