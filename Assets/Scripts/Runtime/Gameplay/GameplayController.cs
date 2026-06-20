using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DoubleD.VerySeriousJamGame.Runtime.Utilities;
using InSun.GameCore;
using InSun.GameCore.Scenes;
using InSun.GameCore.Utilities;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    [SelectionBase]
    internal sealed class GameplayController : MonoBehaviour
    {
        [Header("Intro")]
        [SerializeField]
        private CinemachineCamera introCamera;

        [SerializeField]
        private PlayableDirector introPlayable;

        [Header("Gameplay")]
        [SerializeField]
        private float gameplayDuration = 60f;

        [Min(0)]
        [SerializeField]
        private int maxScore = 1000;

        [Header("Scenes")]
        [SerializeField]
        private SceneData gameOverScene;

        [Header("Pedestal Objects")]
        [SerializeField]
        private List<PedestalObjectData> pedestalObjects;

        private GameplaySystem gameplaySystem;
        private ISceneSystem sceneSystem;

        public float GameplayDuration => gameplayDuration;

        public int MaxScore => maxScore;

        private void Awake()
        {
            gameplaySystem = Game.GetObject<GameplaySystem>();
            sceneSystem = Game.GetObject<ISceneSystem>();
        }

        private void Start()
        {
            gameplaySystem.StartGame(this);
        }

        private void OnDestroy()
        {
            gameplaySystem.StopGame();
        }

        public PedestalObjectActor CreatePedestalObject(Transform parent)
        {
            if (pedestalObjects.TryGetRandom(out var pedestalObject))
            {
                return pedestalObject.CreatePedestalObject(
                    parent.position,
                    Quaternion.identity,
                    parent
                );
            }

            throw new Exception("No pedestal object found");
        }

        public void LoadGameOverScene()
        {
            sceneSystem.LoadScene(new SceneLoadArgs(gameOverScene));
        }

        public async UniTask PlayIntroAsync(CancellationToken cancellationToken)
        {
            if (introPlayable)
            {
                introCamera.enabled = true;
                await introPlayable.PlayAsync(cancellationToken);
                introCamera.enabled = false;
            }
        }
    }
}
