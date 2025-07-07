using System;
using Multiplayer.Network.Objects;
using UnityEngine;

namespace Objects
{
    public abstract class BoardPieceV : ObjectV
    {
        public override Type ModelType => typeof(BoardPiece);

        [SerializeField] private Material faction1Material;
        [SerializeField] private Material faction2Material;
        [SerializeField] private Renderer meshRenderer;
        
        public override void Initialize(ObjectM model)
        {
            base.Initialize(model);

            meshRenderer.material = model.Owner switch
            {
                1 => faction1Material,
                2 => faction2Material,
                _ => meshRenderer.material
            };
        }

        protected override void Update()
        {
            base.Update();
            
            BoardPiece pieceModel = Model as BoardPiece;
            
            meshRenderer.material.color = new Color(pieceModel.color.R, pieceModel.color.G, pieceModel.color.B, pieceModel.color.A);
        }
    }
}