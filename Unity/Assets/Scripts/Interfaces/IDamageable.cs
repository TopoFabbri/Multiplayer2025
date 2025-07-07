using UnityEngine;

namespace Interfaces
{
    public interface IDamageable
    {
        int Life { get; set; }

        void ReceiveDamage();
    }
}