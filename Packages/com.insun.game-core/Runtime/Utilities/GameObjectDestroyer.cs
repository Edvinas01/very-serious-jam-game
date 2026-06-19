using UnityEngine;

namespace InSun.GameCore.Utilities
{
    public sealed class GameObjectDestroyer : MonoBehaviour
    {
        public void DestroyGameObject()
        {
            Destroy(gameObject);
        }
    }
}
