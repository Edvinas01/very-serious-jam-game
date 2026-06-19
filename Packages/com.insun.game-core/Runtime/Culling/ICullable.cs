using System;
using UnityEngine;

namespace InSun.GameCore.Culling
{
    public interface ICullable
    {
        public bool IsVisible { get; }

        public float Radius { set; }

        public Transform ReferenceTransform { set; }

        public Vector3 ReferencePosition { set; }

        public Vector3 Position { set; }

        public event Action OnCullableBecameVisible;

        public event Action OnCullableBecameHidden;
    }
}
