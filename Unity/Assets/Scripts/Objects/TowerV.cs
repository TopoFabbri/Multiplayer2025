using System;
using Multiplayer.Network.Objects;
using UnityEngine;
using UnityEngine.UI;

namespace Objects
{
    public class TowerV : BoardPieceV
    {
        public override Type ModelType => typeof(TowerM);

        [SerializeField] private Image healthBar;
        [SerializeField] private Transform healthBarTransform;
        
        private int initLife;
        private Camera mainCamera;

        public override void Initialize(ObjectM model)
        {
            base.Initialize(model);
            mainCamera = Camera.main;

            initLife = ((TowerM)model).Life;
        }

        private void Update()
        {
            healthBar.fillAmount = ((TowerM)Model).Life / (float)initLife;
            healthBarTransform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
        }
    }
}