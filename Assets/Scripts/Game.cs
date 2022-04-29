using UnityEngine;
namespace Minesweeper
{
    public class Game : MonoBehaviour
    {
        [SerializeField] int _width = 16, _height = 16, _mineCount = 32;

        Board _board;
        Cell[,] _state;
        bool _gameOver;
        void OnValidate()
        {
            _mineCount = Mathf.Clamp(_mineCount, 1, _width * _height);
        }
        void Awake()
        {
            _board = GetComponentInChildren<Board>();
        }
        void Start()
        {
            NewGame();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
                NewGame();
            if (!_gameOver)
            {
                if (Input.GetMouseButtonDown(1))
                    Flag();
                else if (Input.GetMouseButtonDown(0))
                    Reveal();
            }
        }
        void CheckWinCondition()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    Cell cell = _state[x, y];
                    if (cell.type != Cell.Type.Mine && !cell.revealed)
                        return;
                }
            }
            /****************************************/
            Debug.Log("Winnnnnn!!!!");
            _gameOver = true;

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    Cell cell = _state[x, y];

                    if (cell.type == Cell.Type.Mine)
                    {
                        cell.flagged = true;
                        _state[x, y] = cell;
                    }
                }
            }
        }

        void Flag()
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = _board.tilemap.WorldToCell(worldPos);
            Cell cell = GetCell(cellPos.x, cellPos.y);

            if (cell.type == Cell.Type.Invalid || cell.revealed)
                return;

            cell.flagged = !cell.flagged;
            _state[cellPos.x, cellPos.y] = cell;
            _board.Draw(_state);
        }

        void Reveal()
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = _board.tilemap.WorldToCell(worldPos);
            Cell cell = GetCell(cellPos.x, cellPos.y);

            if (cell.type == Cell.Type.Invalid || cell.revealed || cell.flagged)
                return;

            switch (cell.type)
            {
                case Cell.Type.Empty:
                    Flood(cell);
                    CheckWinCondition();
                    break;
                case Cell.Type.Mine:
                    Explode(cell);
                    break;
                default:
                    cell.revealed = true;
                    _state[cellPos.x, cellPos.y] = cell;
                    CheckWinCondition();
                    break;
            }


            _board.Draw(_state);
        }

        void Flood(Cell _cell)
        {
            if (_cell.revealed)
                return;
            if (_cell.type == Cell.Type.Mine || _cell.type == Cell.Type.Invalid)
                return;
            _cell.revealed = true;/**/
            _state[_cell.position.x, _cell.position.y] = _cell;

            if (_cell.type == Cell.Type.Empty)
            {
                Flood(GetCell(_cell.position.x - 1, _cell.position.y));
                Flood(GetCell(_cell.position.x + 1, _cell.position.y));
                Flood(GetCell(_cell.position.x, _cell.position.y - 1));
                Flood(GetCell(_cell.position.x, _cell.position.y + 1));
            }
        }

        void Explode(Cell _cell)
        {
            /********************************************/
            Debug.Log("Game Overr......");
            _gameOver = true;
            _cell.revealed = true;
            _cell.exploded = true;
            _state[_cell.position.x, _cell.position.y] = _cell;

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    _cell = _state[x, y];

                    if (_cell.type == Cell.Type.Mine)
                    {
                        _cell.revealed = true;
                        _state[x, y] = _cell;
                    }
                }
            }
        }

        Cell GetCell(int x, int y)
        {
            if (IsValid(x, y))
                return _state[x, y];
            else
                return new Cell();
        }

        bool IsValid(int x, int y)
        {
            return x >= 0 && x < _width && y >= 0 && y < _height;
        }

        void NewGame()
        {
            _gameOver = false;

            _state = new Cell[_width, _height];

            GenerateCells();
            GenerateMines();
            GenerateNumbers();

            Camera.main.transform.position = new Vector3(_width / 2f, _height / 2f, -10f);
            _board.Draw(_state);
        }

        void GenerateCells()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    Cell cell = new Cell();
                    cell.position = new Vector3Int(x, y, 0);
                    cell.type = Cell.Type.Empty;
                    _state[x, y] = cell;
                }
            }
        }

        void GenerateMines()
        {
            for (int i = 0; i < _mineCount; i++)
            {
                int x = Random.Range(0, _width);
                int y = Random.Range(0, _height);

                while (_state[x, y].type == Cell.Type.Mine)
                {
                    x++;
                    if (x >= _width)
                    {
                        x = 0;
                        y++;

                        if (y >= _height)
                            y = 0;
                    }
                }
                _state[x, y].type = Cell.Type.Mine;
                //_state[x, y].revealed = true;/**/
            }
        }

        void GenerateNumbers()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    Cell cell = _state[x, y];
                    if (cell.type == Cell.Type.Mine)
                        continue;
                    cell.number = CountMines(x, y);
                    if (cell.number > 0)
                        cell.type = Cell.Type.Number;
                    //cell.revealed = true;/**/
                    _state[x, y] = cell;
                }
            }
        }

        int CountMines(int _cellX, int _cellY)
        {
            int count = 0;

            for (int neighborX = -1; neighborX <= 1; neighborX++)
            {
                for (int neighborY = -1; neighborY <= 1; neighborY++)
                {
                    if (neighborX == 0 && neighborY == 0)
                        continue;

                    int x = _cellX + neighborX;
                    int y = _cellY + neighborY;

                    if (GetCell(x, y).type == Cell.Type.Mine)
                        count++;

                }
            }
            return count;
        }
    }
}