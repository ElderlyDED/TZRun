using UnityEngine;

namespace Game.Triggers
{
    [RequireComponent(typeof(BoxCollider))]
    public class TrapTrigger : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Unit unit))
            {
                unit.Kill();
            }
        }
    }
}