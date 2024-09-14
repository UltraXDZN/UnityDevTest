using UnityEngine;

namespace UDT.Gameplay.AI{
    public class UDT_AIGuard : UDT_AINPC
    {
        protected override void Start()
        {
            base.Start();
        }

        protected void Update()
        {
            Debug.Log("Guard AI Update");
        }
    }
}