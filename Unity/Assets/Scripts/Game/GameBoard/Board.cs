using System.Collections.Generic;
using Multiplayer.Network;
using Objects;
using Random = UnityEngine.Random;

namespace Game.GameBoard
{
    public class Board
    {
        private Tile selected;
        private Tile cursor;

        private readonly List<BoardPiece> objects = new();

        private int width;
        private int height;

        private Tile p1Corner;
        private Tile p2Corner;

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
            if (newState != GameState.InGame)
                GameStateText = string.Empty;
        }

        public void OnMousePos((int x, int y) pos)
        {
            SetMouseGameState();

            if (pos.x < 0 || pos.x >= width || pos.y < 0 || pos.y >= height)
                return;

            if (cursor != null)
            {
                if (pos.x == cursor.X && pos.y == cursor.Y)
                    return;
            }

            cursor = new Tile(pos.x, pos.y);
        }

        private void SetMouseGameState()
        {
            BoardPiece selectedPiece = GetTile(selected);
            BoardPiece hoveredPiece = GetTile(cursor);
            
            if (selectedPiece != null && cursor != null)
            {
                GameStateText = "Move " + selectedPiece.Name + " " + selectedPiece.ObjectId + " to " + cursor.X + ", " + cursor.Y;
            }
            else if (cursor != null)
            {
                if (hoveredPiece != null)
                    GameStateText = hoveredPiece.Name + " " + hoveredPiece.ObjectId;
                else
                    GameStateText = "Tile " + cursor.X + ", " + cursor.Y + " Hovered";
            }
            else
            {
                GameStateText = "Select Tile";
            }
        }

        public void OnClick()
        {
            if (selected == null)
                Select();
            else
                Move();

            SetMouseGameState();
        }

        private void Select()
        {
            if (cursor == null)
                return;

            BoardPiece piece = GetTile(cursor);

            if (piece == null)
                return;

            if (!piece.CanMove)
                return;

            if (piece.Owner != NetworkManager.Instance.Id && NetworkManager.Instance.Id != 0)
                return;

            selected = cursor;
        }

        private void Move()
        {
            if (GetTile(cursor) != null)
                return;

            BoardPiece selectedPiece = GetTile(selected);
            
            if (selectedPiece == null)
                return;
            
            if (!selectedPiece.MoveTo(cursor)) return;

            selected = null;
        }

        public void CreateBoard(int height, int width)
        {
            this.width = width;
            this.height = height;

            p1Corner = new Tile(0, height - 1);
            p2Corner = new Tile(width - 1, 0);
        }

        public void PlaceObject(BoardPiece boardPiece)
        {
            bool ownerIsP1 = boardPiece.Owner == 1;

            if (boardPiece.GetType() == typeof(TowerM))
            {
                if (ownerIsP1)
                    boardPiece.PlaceAt(p1Corner.X, p1Corner.Y);
                else
                    boardPiece.PlaceAt(p2Corner.X, p2Corner.Y);
            }
            else
            {
                int x;
                int y;

                int attempts = 0;
                
                do
                {
                    if (ownerIsP1)
                    {
                        x = Random.Range(0, 5);
                        y = Random.Range(height - 6, height - 1);
                    }
                    else
                    {
                        x = Random.Range(width - 6, width - 1);
                        y = Random.Range(0, 5);
                    }

                    boardPiece.PlaceAt(x, y);
                } while (GetTile(new Tile(x, y)) != null && attempts++ < 100);
                
                if (attempts >= 100)
                {
                    UnityEngine.Debug.LogError("Failed to place object after 100 attempts.");
                    return;
                }
            }

            objects.Add(boardPiece);
        }

        private BoardPiece GetTile(Tile tile)
        {
            BoardPiece piece = null;

            if (tile == null)
                return null;
            
            foreach (BoardPiece boardPiece in objects)
            {
                if (boardPiece.x != tile.X || boardPiece.y != tile.Y) continue;

                piece = boardPiece;
                break;
            }

            return piece;
        }
    }
}