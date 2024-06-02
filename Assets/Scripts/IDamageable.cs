using System.Numerics;
using Vector3 = UnityEngine.Vector3;

namespace DefaultNamespace
{
    public interface IDamageable
    {
        void ApplyDamage(int dmg);
        
        void ApplyDamage(int dmg, Vector3 posAttack = new Vector3());

    }
}