using System;
using UnityEngine;

namespace Game
{
    public class TileVisualizer : MonoBehaviour
    {
        [SerializeField] private Material normalMat;
        [SerializeField] private Material hoveredMat;
        [SerializeField] private new Renderer renderer;

        private static TileVisualizer _hovered;

        public static TileVisualizer Hovered
        {
            get => _hovered;
            internal set
            {
                if (value == null)
                    return;

                if (_hovered == null)
                {
                    _hovered = value;
                    _hovered.OnHoveredStart();
                    return;
                }

                if (Equals(_hovered, value))
                    return;

                _hovered.OnHoveredEnd();
                _hovered = value;
                _hovered.OnHoveredStart();
            }
        }

        private void OnHoveredStart()
        {
            renderer.material = hoveredMat;
        }

        private void OnHoveredEnd()
        {
            renderer.material = normalMat;
        }
    }
}