using System;
using Game;
using Game.GameBoard;
using Multiplayer.AuthoritativeServer;
using Multiplayer.Network;
using Multiplayer.Network.Messages;
using Multiplayer.Reflection;

namespace Objects
{
    public class Cursor : PlayerInput
    {
        public event Action<int> Click;
        
        public override void Initialize(int ownerId, int objectId)
        {
            base.Initialize(ownerId, objectId);
            
            if (NetworkManager.Instance.Id != ownerId)
                return;
            
            MouseBoardConverter.MousePos += OnMousePos;
            InputListener.Click += OnClick;
        }
        
        ~Cursor()
        {
            MouseBoardConverter.MousePos -= OnMousePos;
            InputListener.Click -= OnClick;
        }

        [Rpc] public void OnClick()
        {
            // Clicked = true;
            // Click?.Invoke(ObjectId);
            // Clicked = false;
        }

        private void OnMousePos((int x, int y) pos)
        {
            CursorX = pos.x;
            CursorY = pos.y;
            
            SetPosition(CursorX, 0, CursorY);
            NetworkManager.Instance.SendData(new NetPlayerInput(this).Serialize());
        }

        public override void Update()
        {
            base.Update();
            
            if (Clicked)
                OnClick();
        }
    }
}