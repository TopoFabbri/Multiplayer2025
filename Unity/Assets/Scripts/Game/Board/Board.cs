using Objects;
using UnityEngine;

namespace Game
{
    public class Board
    {
        public enum CursorState
        {
            Select,
            Move
        }
        
        private Tile[,] tiles;

        private Tile hoveredTile;
        private Tile selectedTile;

        private int width;
        private int height;
        
        private (int x, int y) p1Corner;
        private (int x, int y) p2Corner;
        
        private (int x, int y) mousePos;

        private CursorState cursorState = CursorState.Select;
        
        public Board()
        {
            GameStateController.StateChanged += OnStateChanged;
        }

        ~Board()
        {
            GameStateController.StateChanged -= OnStateChanged;
        }

        private void OnStateChanged(GameState newState)
        {
            if (newState == GameState.InGame)
            {
                InputListener.MousePos += OnMousePos;
                InputListener.Click += OnClick;
            }
            else
            {
                InputListener.MousePos -= OnMousePos;
                InputListener.Click -= OnClick;
            }
        }

        private void OnMousePos(Vector2 pos)
        {
            if (Camera.main == null)
            {
                hoveredTile = null;
                return;
            }
            
            Ray ray = Camera.main.ScreenPointToRay(pos);

            if (!Physics.Raycast(ray, out RaycastHit hit))
            {
                hoveredTile = null;
                return;
            }

            int x = Mathf.FloorToInt(hit.point.x / 2);
            int y = Mathf.FloorToInt(hit.point.z / 2);
            
            mousePos.x = x;
            mousePos.y = y;

            if (x >= 0 && x < tiles.GetLength(0) && y >= 0 && y < tiles.GetLength(1))
                hoveredTile = tiles[x, y];
            else
                hoveredTile = null;
        }

        private void OnClick()
        {
            if (cursorState == CursorState.Select)
            {
                Debug.Log("Clicked on tile: " + mousePos.x + ", " + mousePos.y);
                if (hoveredTile?.ContainingObject == null)
                    return;
                
                selectedTile = hoveredTile;
                cursorState = CursorState.Move;
            }
            else
            {
                if (!MovePiece(hoveredTile.PosX, hoveredTile.PosY)) return;
                
                selectedTile = null;
                cursorState = CursorState.Select;
            }
        }

        private bool MovePiece(int x, int y)
        {
            if (selectedTile == null || hoveredTile == null)
                return false;

            int selectedX = selectedTile.PosX;
            int selectedY = selectedTile.PosY;

            if (Mathf.Abs(selectedX - x) > 1 || Mathf.Abs(selectedY - y) > 1) return false;
            
            hoveredTile.PlaceObject(selectedTile.ContainingObject);
            selectedTile.RemoveObject();

            return true;
        }
        
        public void CreateBoard(int height, int width)
        {
            this.width = width;
            this.height = height;
            
            tiles = new Tile[width, height];

            p1Corner.x = 0;
            p1Corner.y = height - 1;
            p2Corner.x = width - 1;
            p2Corner.y = 0;
            
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    tiles[j, i] = new Tile(j, i);
                }
            }
        }
        
        public void PlaceObject(BoardPiece boardPiece)
        {
            bool ownerIsP1 = boardPiece.Owner == 1;
            
            if (boardPiece.GetType() == typeof(TowerM))
            {
                if (ownerIsP1)
                    tiles[p1Corner.x, p1Corner.y].PlaceObject(boardPiece);
                else
                    tiles[p2Corner.x, p2Corner.y].PlaceObject(boardPiece);
            }
            else
            {
                int x;
                int y;
                
                do
                {
                    x = ownerIsP1 ? Random.Range(0, 5) : Random.Range(width - 6, width - 1);
                    y = ownerIsP1 ? Random.Range(height - 6, height - 1) : Random.Range(0, 5);
                } while (!tiles[x, y].PlaceObject(boardPiece));
            }
        }
    }
}