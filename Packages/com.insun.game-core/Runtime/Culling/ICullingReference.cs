using UnityEngine;

namespace InSun.GameCore.Culling
{
    internal interface ICullingReference
    {
        public Transform ReferenceTransform { get; }

        public Camera ReferenceCamera { get; }
    }
}
