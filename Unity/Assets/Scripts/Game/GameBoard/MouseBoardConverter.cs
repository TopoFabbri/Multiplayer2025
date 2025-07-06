using System;
using UnityEngine;

namespace Game.GameBoard
{
    public class MouseBoardConverter : MonoBehaviour
    {
        [SerializeField] private Transform tile00;
        [SerializeField] private Transform tile11;
        [SerializeField] private int boardWidth = 30;
        [SerializeField] private int boardHeight = 30;
        
        public static event Action<(int x, int y)> MousePos;

        private void Start()
        {
            GameStateController.StateChanged += OnStateChanged;
        }

        private void OnStateChanged(GameState newState)
        {
            if (newState == GameState.InGame)
            {
                InputListener.MousePos += OnMousePos;
            }
            else
            {
                InputListener.MousePos -= OnMousePos;
            }
        }

        private void OnMousePos(Vector2 mousePos)
        {
            if (Camera.main == null) return;

            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            if (!Physics.Raycast(ray, out RaycastHit hit)) return;

            Vector3 hitPoint = hit.point;
            
            float xPos = hitPoint.x;
            float yPos = hitPoint.z;

            float tile00X = tile00.position.x;
            float tile00Y = tile00.position.z;
            float tile11X = tile11.position.x;
            float tile11Y = tile11.position.z;

            float minX = Mathf.Min(tile00X, tile11X);
            float maxX = Mathf.Max(tile00X, tile11X);
            float minY = Mathf.Min(tile00Y, tile11Y);
            float maxY = Mathf.Max(tile00Y, tile11Y);

            if (xPos < minX || xPos > maxX || yPos < minY || yPos > maxY) return;

            float normalizedX = (xPos - minX) / (maxX - minX);
            float normalizedY = (yPos - minY) / (maxY - minY);

            int x = Mathf.FloorToInt(normalizedX * boardWidth);
            int y = Mathf.FloorToInt(normalizedY * boardHeight);
            
            MousePos?.Invoke((x, y));
        }
    }
}