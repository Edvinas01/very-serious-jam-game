using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    [SelectionBase]
    internal sealed class PaintBrushActor : MonoBehaviour
    {
        [SerializeField]
        private Transform paintPoint;

        [Min(0f)]
        [SerializeField]
        private float paintTriggerRadius = 0.1f;

        [Min(1)]
        [SerializeField]
        private int paintTexelRadius = 20;

        [SerializeField]
        private LayerMask paintLayerMask;

        [SerializeField]
        private Color paintColor = Color.crimson;

        private static readonly RaycastHit[] HitBuffer = new RaycastHit[10];

        private void OnDrawGizmos()
        {
            if (paintPoint == false)
            {
                return;
            }

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(paintPoint.position, paintTriggerRadius);
        }

        private void FixedUpdate()
        {
            var count = Physics.RaycastNonAlloc(
                origin: paintPoint.position,
                direction: paintPoint.forward,
                results: HitBuffer,
                maxDistance: paintTriggerRadius,
                layerMask: paintLayerMask,
                queryTriggerInteraction: QueryTriggerInteraction.Ignore
            );

            for (var index = 0; index < count; index++)
            {
                var hit = HitBuffer[index];
                var pedestalObject = hit.collider.GetComponentInParent<PedestalObjectActor>();
                if (pedestalObject == false)
                {
                    continue;
                }

                pedestalObject.Paint(
                    uv: hit.textureCoord,
                    radius: paintTexelRadius,
                    color: paintColor
                );
            }
        }
    }
}
