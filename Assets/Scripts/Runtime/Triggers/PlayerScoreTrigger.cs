using DoubleD.VerySeriousJamGame.Runtime.Gameplay;
using InSun.GameCore;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Triggers
{
    internal sealed class PlayerScoreTrigger : MonoBehaviour
    {
        [SerializeField]
        private int scoreValue;

        private GameplaySystem gameplaySystem;

        private void Awake()
        {
            gameplaySystem = Game.GetObject<GameplaySystem>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<PlayerActor>() == false)
            {
                return;
            }

            gameplaySystem.Score += scoreValue;
        }
    }
}
