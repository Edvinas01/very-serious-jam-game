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

        [Header("Scenes")]
        [SerializeField]
        private SceneData victoryScene;

        [SerializeField]
        private SceneData gameOverScene;

        [Header("Pedestal Objects")]
        [SerializeField]
        private List<PedestalObjectData> pedestalObjects;

        private GameplaySystem gameplaySystem;
        private ISceneSystem sceneSystem;

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
                var instance = pedestalObject.CreatePedestalObject(
                    parent.position,
                    Quaternion.identity,
                    parent
                );

                instance.name = $"{nameof(PedestalObjectActor)}_{pedestalObjects.Count}";
                return instance;
            }

            throw new Exception("No pedestal object found");
        }

        public void LoadVictoryScene()
        {
            sceneSystem.LoadScene(new SceneLoadArgs(victoryScene));
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
