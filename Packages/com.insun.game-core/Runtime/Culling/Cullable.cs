using System;
using UnityEngine;

namespace InSun.GameCore.Culling
{
    public abstract class Cullable : MonoBehaviour, ICullable
    {
        public abstract bool IsVisible { get; }

        public abstract float Radius { set; }

        public abstract Transform ReferenceTransform { set; }

        public abstract Vector3 ReferencePosition { set; }

        public abstract Vector3 Position { set; }

        public abstract event Action OnCullableBecameVisible;

        public abstract event Action OnCullableBecameHidden;
    }
}
