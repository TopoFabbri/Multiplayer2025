using System;
using System.Collections.Generic;
using Game;
using UnityEngine;

namespace UI
{
    public class ColorPicker : MonoBehaviour
    {
        [SerializeField] private List<Color> colors;

        public static event Action<Color> ColorPicked;

        private void Awake()
        {
            GameStateController.StateChanged += OnStateChanged;
        }

        private void OnDestroy()
        {
            GameStateController.StateChanged -= OnStateChanged;
        }

        private void OnStateChanged(GameState newState)
        {
            gameObject.SetActive(newState == GameState.ColorPick);
        }

        public void OnColorClicked(int colorIndex)
        {
            ColorPicked?.Invoke(colors[colorIndex]);
        }
    }
}