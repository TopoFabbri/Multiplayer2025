using UnityEngine;

namespace Game
{
    public class BoardVisualizer : MonoBehaviour
    {
        private void Start()
        {
            GameStateController.StateChanged += OnGameStateChanged;
        }

        private void OnDestroy()
        {
            GameStateController.StateChanged -= OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState newState)
        {
            if (newState == GameState.InGame)
                InputListener.MousePos += OnMousePosition;
            else
                InputListener.MousePos -= OnMousePosition;
        }

        private void OnMousePosition(Vector2 pos)
        {
            if (GameStateController.State != GameState.InGame)
                return;

            if (!Camera.main) return;
            
            Ray ray = Camera.main.ScreenPointToRay(pos);
            
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                TileVisualizer hoveredTile = hit.collider.GetComponent<TileVisualizer>();
                
                if (!hoveredTile)
                    return;

                TileVisualizer.Hovered = hoveredTile;
            }
            else
            {
                TileVisualizer.Hovered = null;
            }
        }
    }
}