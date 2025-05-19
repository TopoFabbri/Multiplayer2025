using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class ColorPicker : MonoBehaviour
    {
        [SerializeField] private List<Color> colors;

        public static event Action<Color> ColorPicked; 
        
        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public void OnColorClicked(int colorIndex)
        {
            ColorPicked?.Invoke(colors[colorIndex]);
        }
    }
}