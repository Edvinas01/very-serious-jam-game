using UnityEngine;

namespace InSun.GameCore.Audio
{
    public struct PlayAudioEventArgs
    {
        public IAudioEvent AudioEvent { get; }

        public Vector3 Position { get; set; }

        public Rigidbody RigidBody { get; set; }

        public GameObject Owner { get; }

        public bool IsFollowOwner { get; set; }

        public PlayAudioEventArgs(IAudioEvent audioEvent, GameObject owner)
        {
            AudioEvent = audioEvent;
            Position = default;
            RigidBody = null;
            Owner = owner;
            IsFollowOwner = false;
        }
    }
}
