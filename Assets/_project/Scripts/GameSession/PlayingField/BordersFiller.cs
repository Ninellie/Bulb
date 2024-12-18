using System.Collections;
using _project.Scripts.Core.Variables;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Assets.Scripts.GameSession.PlayingField.TilesCameraHelper;

namespace Assets.Scripts.GameSession.PlayingField
{
    public class BordersFiller : MonoBehaviour
    {
        [SerializeField] private TileData _tile;
        [SerializeField] private TileBase _otherTile;
        [SerializeField] [Range(0, 300)] private float _fillingDelay;
        [SerializeField] [Range(-5, 5)] private int _boundsOffset;
        [SerializeField] [Range(0, 5)] private int _fillingFramesCount;
        [SerializeField] [Range(0, 5)] private float _delayBetweenFills;
        [SerializeField] private Vector2Variable _cageCenter;
        [Header("Inner components")]
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private Tilemap _otherTilemap;
        [SerializeField] private TilemapRenderer _tilemapRenderer;
        [SerializeField] private UnityEngine.Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = UnityEngine.Camera.main;
        }

        private void Start()
        {
            if (_tile is null) return;
            StartCoroutine(FillBorderTilesInCameraBounds());
        }

        private IEnumerator FillBorderTilesInCameraBounds()
        {
            yield return new WaitForSeconds(_fillingDelay);
            var boundsInt = GetBoundsIntFromCamera(_mainCamera, _tilemap, _tilemapRenderer.chunkCullingBounds);
            _cageCenter.SetValue(_mainCamera.transform.position);
            StartCoroutine(FillBounds(boundsInt));
        }

        private IEnumerator FillBounds(BoundsInt bounds)
        {
            var min = bounds.min;
            min = new Vector3Int(min.x + _boundsOffset, min.y + _boundsOffset);
            var max = bounds.max;
            max = new Vector3Int(max.x - _boundsOffset, max.y - _boundsOffset);

            bounds.SetMinMax(min, max);

            for (int i = 0; i < _fillingFramesCount; i++)
            {
                StartCoroutine(FillFrame(_tilemap, _tile.tile, bounds, _delayBetweenFills));

                var perimeterTilesCount = bounds.size.x * 2 + bounds.size.y * 2 - 2;
                var frameFillingDelay = perimeterTilesCount * _delayBetweenFills;

                yield return new WaitForSeconds(frameFillingDelay * 1.1f);

                min = bounds.min;
                min = new Vector3Int(min.x + 1, min.y + 1);
                max = bounds.max;
                max = new Vector3Int(max.x - 1, max.y - 1);

                bounds.SetMinMax(min, max);
            }
        }

        private IEnumerator FillFrame(Tilemap tilemap, TileBase tileToFill, BoundsInt bounds, float delay)
        {
            delay = Mathf.Max(0, delay);
            var left = bounds.xMin;
            var right = bounds.xMax - 1;
            var bottom = bounds.yMin;
            var top = bounds.yMax - 1;

            Vector3Int tilePos;

            // Fill bottom row
            for (int i = left; i <= right; i++)
            {
                tilePos = new Vector3Int(i, bottom, 0);
                _otherTilemap.SetTile(tilePos, _otherTile);
                tilemap.SetTile(tilePos, tileToFill);
                if (!(delay > 0)) continue;
                yield return new WaitForSeconds(delay);
            }

            // Fill right column
            for (int i = bottom + 1; i <= top; i++)
            {
                tilePos = new Vector3Int(right, i, 0);
                _otherTilemap.SetTile(tilePos, _otherTile);
                tilemap.SetTile(tilePos, tileToFill);
                if (!(delay > 0)) continue;
                yield return new WaitForSeconds(delay);
            }

            // Fill top row
            if (bottom < top)
            {
                for (int i = right - 1; i >= left; i--)
                {
                    tilePos = new Vector3Int(i, top, 0);
                    _otherTilemap.SetTile(tilePos, _otherTile);
                    tilemap.SetTile(tilePos, tileToFill);
                    if (!(delay > 0)) continue;
                    yield return new WaitForSeconds(delay);
                }
            }

            // Fill left column
            if (left < right)
            {
                for (int i = top - 1; i > bottom; i--)
                {
                    tilePos = new Vector3Int(left, i, 0);
                    _otherTilemap.SetTile(tilePos, _otherTile);
                    tilemap.SetTile(tilePos, tileToFill);
                    if (!(delay > 0)) continue;
                    yield return new WaitForSeconds(delay);
                }
            }
        }
    }
}