using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace Game.Triggers
{
    public class PointTrigger : MonoBehaviour
    {
        [SerializeField] ParticleSystem _pickUpParticleSystem;
        private bool _triggered;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Unit unit) && !_triggered)
            {
                _triggered = true;
                StartCoroutine(DestroyDelay());
                Game.Instance.PointCounter.Add();
               
            }
        }
        
        IEnumerator DestroyDelay()
        {
            _pickUpParticleSystem.Play();
            yield return new WaitForSeconds(0.15f);
            Destroy(gameObject);
        }
    }
}