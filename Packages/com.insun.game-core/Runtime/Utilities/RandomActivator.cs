using System.Collections.Generic;
using UnityEngine;

namespace InSun.GameCore.Utilities
{
    public sealed class RandomActivator : MonoBehaviour
    {
        private enum ActivationMode
        {
            None = 0,
            Start = 1,
            Awake = 2,
        }

        [SerializeField]
        private ActivationMode activationMode = ActivationMode.Start;

        [SerializeField]
        private List<GameObject> gameObjects;

        private void Awake()
        {
            if (activationMode == ActivationMode.Awake)
            {
                Activate();
            }
        }

        private void Start()
        {
            if (activationMode == ActivationMode.Start)
            {
                Activate();
            }
        }

        public void Activate()
        {
            foreach (var obj in gameObjects)
            {
                obj.SetActive(false);
            }

            if (gameObjects.TryGetRandom(out var randomObj))
            {
                randomObj.SetActive(true);
            }
        }

        public void Deactivate()
        {
            foreach (var obj in gameObjects)
            {
                obj.SetActive(false);
            }
        }
    }
}
