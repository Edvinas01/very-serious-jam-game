using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    internal sealed class BrushActor : MonoBehaviour
    {
        [SerializeField]
        private Transform paintPoint;

        [Min(0f)]
        [SerializeField]
        private float paintRadius = 0.1f;

        [SerializeField]
        private LayerMask layerMask;

        private static readonly RaycastHit[] HitBuffer = new RaycastHit[10];

        private void OnDrawGizmos()
        {
            if (paintPoint == false)
            {
                return;
            }

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(paintPoint.position, paintRadius);
        }

        private void FixedUpdate()
        {
            var count = Physics.SphereCastNonAlloc(
                origin: paintPoint.position,
                radius: paintRadius,
                direction: paintPoint.forward,
                layerMask: layerMask,
                results: HitBuffer,
                maxDistance: 0f,
                queryTriggerInteraction: QueryTriggerInteraction.Ignore
            );

            for (var index = 0; index < count; index++)
            {
                var hit = HitBuffer[index];
                if (hit.collider.TryGetComponent<PedestalObjectActor>(out var pedestalObject) == false)
                {
                    continue;
                }

                Paint(pedestalObject, hit);
            }
        }

        private void Paint(PedestalObjectActor pedestalObject, RaycastHit hit)
        {
            Debug.Log($"Paint at {hit.point}", pedestalObject);
        }
    }
}
