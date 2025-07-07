using System;
using Multiplayer.Network.Objects;
using UnityEngine;
using UnityEngine.UI;

namespace Objects
{
    public class PawnV : BoardPieceV
    {
        public override Type ModelType => typeof(PawnM);

        [SerializeField] private Image healthBar;
        [SerializeField] private Transform healthBarTransform;
        
        private int initLife;
        private Camera mainCamera;

        public override void Initialize(ObjectM model)
        {
            base.Initialize(model);

            initLife = ((PawnM)model).Life;
            mainCamera = Camera.main;
        }

        private void Update()
        {
            healthBar.fillAmount = ((PawnM)Model).Life / (float)initLife;
            healthBarTransform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);
        }
    }
}