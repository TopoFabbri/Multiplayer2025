using Objects;

namespace Game
{
    public class Tile
    {
        public int PosX { get; }
        public int PosY { get; }

        public BoardPiece ContainingObject { get; private set; }

        public Tile(int x, int y)
        {
            PosX = x;
            PosY = y;
        }
        
        public bool PlaceObject(BoardPiece boardPiece)
        {
            if (ContainingObject != null)
                return false;

            ContainingObject = boardPiece;
            boardPiece.SetPosition(PosX, 0, PosY);
            return true;
        }
        
        public void RemoveObject()
        {
            if (ContainingObject == null)
                return;

            ContainingObject = null;
        }
    }
}