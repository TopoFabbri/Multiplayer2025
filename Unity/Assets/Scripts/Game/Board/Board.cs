using Objects;
using UnityEngine;
using Random = UnityEngine.Random;

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

        private CursorState cursorState = CursorState.Select;
        
        public static string GameStateText { get; private set; }
        
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
                MouseBoardConverter.MousePos += OnMousePos;
                InputListener.Click += OnClick;
            }
            else
            {
                MouseBoardConverter.MousePos -= OnMousePos;
                InputListener.Click -= OnClick;
                
                GameStateText = string.Empty;
            }
        }

        private void OnMousePos((int x, int y) pos)
        {
            SetMouseGameState();
            
            pos.x = Mathf.FloorToInt(pos.x);
            pos.y = Mathf.FloorToInt(pos.y);
            
            if (pos.x >= 0 && pos.x < tiles.GetLength(0) && pos.y >= 0 && pos.y < tiles.GetLength(1))
                hoveredTile = tiles[pos.x, pos.y];
            else
                hoveredTile = null;
        }

        private void SetMouseGameState()
        {
            if (selectedTile != null)
            {
                GameStateText = "Object " + selectedTile.ContainingObject.ObjectId + " Selected";
            }
            else if (hoveredTile != null)
            {
                if (hoveredTile.ContainingObject != null)
                    GameStateText = "Object " + hoveredTile.ContainingObject.ObjectId;
                else
                    GameStateText = "Tile " + hoveredTile.PosX + ", " + hoveredTile.PosY + " Hovered";
            }
            else
            {
                GameStateText = "Select Tile";
            }
        }

        private void OnClick()
        {
            if (cursorState == CursorState.Select)
            {
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