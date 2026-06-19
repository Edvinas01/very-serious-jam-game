using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Playables;

namespace DoubleD.VerySeriousJamGame.Runtime.Utilities
{
    internal static class AnimationUtilities
    {
        public static UniTask PlayAsync(this PlayableDirector director, CancellationToken cancellationToken = default)
        {
            var completion = new UniTaskCompletionSource();

            cancellationToken.RegisterWithoutCaptureExecutionContext(() =>
                {
                    director.stopped -= OnStopped;
                    director.Stop();
                    completion.TrySetCanceled();
                }
            );

            director.stopped += OnStopped;
            director.Play();

            return completion.Task;

            void OnStopped(PlayableDirector _)
            {
                director.stopped -= OnStopped;
                completion.TrySetResult();
            }
        }
    }
}
