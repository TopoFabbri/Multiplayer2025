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

            if (NetworkManager.Instance is not ClientNetManager clientManager) return;

            if (clientManager.IsAuthoritative)
                InputListener.Click += OnClick;
            else
                InputListener.Click += SendClick;
        }
        
        ~Cursor()
        {
            MouseBoardConverter.MousePos -= OnMousePos;
            
            if (NetworkManager.Instance is not ClientNetManager clientManager) return;

            if (clientManager.IsAuthoritative)
                InputListener.Click -= OnClick;
            else
                InputListener.Click -= SendClick;
        }

        [Rpc] public void OnClick()
        {
            Click?.Invoke(objectId);
            Clicked = false;
        }

        private void SendClick()
        {
            Clicked = true;
            NetworkManager.Instance.SendData(new NetPlayerInput(this).Serialize());
            Clicked = false;
        }

        private void OnMousePos((int x, int y) pos)
        {
            CursorX = pos.x;
            CursorY = pos.y;
            
            SetPosition(CursorX, 0, CursorY);
            NetworkManager.Instance.SendData(new NetPlayerInput(this).Serialize());
        }
    }
}