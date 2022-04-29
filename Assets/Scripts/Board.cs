using UnityEngine;
using UnityEngine.Tilemaps;
namespace Minesweeper
{
    public class Board : MonoBehaviour
    {
        public Tilemap tilemap { get; private set; }

        [SerializeField] Tile _tileUnknown, _tileEmpty, _tileMine, _tileExploded, _tileFlag;
        [SerializeField] Tile _tileNum1, _tileNum2, _tileNum3, _tileNum4, _tileNum5, _tileNum6, _tileNum7, _tileNum8    ;

        void Awake()
        {
            tilemap = GetComponent<Tilemap>();
        }

        public void Draw(Cell[,] _state)
        {
            int width = _state.GetLength(0);
            int height = _state.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Cell cell = _state[x, y];
                    tilemap.SetTile(cell.position, GetTile(cell));
                }
            }
        }

        Tile GetTile(Cell _cell)
        {

            if (_cell.revealed)
            {
                return GetRevealedTile(_cell);
            }
            else if (_cell.flagged)
                return _tileFlag;
            else
                return _tileUnknown;
        }

        Tile GetRevealedTile(Cell _cell)
        {
            switch (_cell.type)
            {
                case Cell.Type.Empty:
                    return _tileEmpty;
                case Cell.Type.Mine:
                    return _cell.exploded ? _tileExploded : _tileMine;
                case Cell.Type.Number:
                    return GetNumberTile(_cell);
                default:
                    return null;
            }

        }

        private Tile GetNumberTile(Cell _cell)
        {
            switch (_cell.number)
            {
                case 1:
                    return _tileNum1;
                case 2:
                    return _tileNum2;
                case 3:
                    return _tileNum3;
                case 4:
                    return _tileNum4;
                case 5:
                    return _tileNum5;
                case 6:
                    return _tileNum6;
                case 7:
                    return _tileNum7;
                case 8:
                    return _tileNum8;
                default:
                    return null;
            }
        }
    }
}
