using DoubleD.VerySeriousJamGame.Runtime.Gameplay;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Triggers
{
    internal sealed class PlayerScoreTrigger : MonoBehaviour
    {
        [SerializeField]
        private int scoreValue;

        private void OnTriggerEnter(Collider other)
        {
            var player = other.GetComponentInParent<PlayerActor>();
            if (player == false)
            {
                return;
            }

            player.Score += scoreValue;
        }
    }
}
