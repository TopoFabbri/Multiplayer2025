using System;

namespace Objects
{
    public class PawnV : BoardPieceV
    {
        public override Type ModelType => typeof(PawnM);
    }
}