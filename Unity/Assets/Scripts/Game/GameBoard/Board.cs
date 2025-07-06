using System.Collections.Generic;
using Multiplayer.Network;
using Objects;
using Random = UnityEngine.Random;

namespace Game.GameBoard
{
    public class Board
    {
        public enum CursorState
        {
            Select,
            Move
        }

        private Tile selected;
        private Tile cursor;

        private readonly List<BoardPiece> objects = new();

        private int width;
        private int height;

        private Tile p1Corner;
        private Tile p2Corner;

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
            if (selected != null)
            {
                BoardPiece selectedPiece = GetTile(selected);
                GameStateText = "Move " + selectedPiece.Name + " " + selectedPiece.ObjectId + " to " + cursor.X + ", " + cursor.Y;
            }
            else if (cursor != null)
            {
                BoardPiece hoveredPiece = GetTile(cursor);
                
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

        private void OnClick()
        {
            if (cursorState == CursorState.Select)
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
                cursorState = CursorState.Move;
            }
            else
            {
                if (GetTile(cursor) != null)
                    return;

                if (!GetTile(selected).MoveTo(cursor)) return;

                selected = null;
                cursorState = CursorState.Select;
            }

            SetMouseGameState();
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
                    
                } while (GetTile(new Tile(x, y)) != null);
            }

            objects.Add(boardPiece);
        }

        private BoardPiece GetTile(Tile tile)
        {
            BoardPiece piece = null;

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