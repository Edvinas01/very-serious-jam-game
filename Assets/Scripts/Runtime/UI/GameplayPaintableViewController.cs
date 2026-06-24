using DoubleD.VerySeriousJamGame.Runtime.Gameplay;
using InSun.GameCore;
using InSun.GameCore.UI;

namespace DoubleD.VerySeriousJamGame.Runtime.UI
{
    internal sealed class GameplayPaintableViewController : ViewController<GameplayPaintableView>
    {
        private GameplaySystem gameplaySystem;

        protected override void Awake()
        {
            base.Awake();

            gameplaySystem = Game.GetObject<GameplaySystem>();
        }

        protected override void OnViewShowEntered()
        {
            base.OnViewShowEntered();

            Game.AddListener<PaintAmountChangedMessage>(OnPaintAmountChanged);
            Game.AddListener<CurrentPaintableChangedMessage>(OnCurrentPaintableChanged);

            View.PaintAmount = gameplaySystem.PaintAmount;

            if (gameplaySystem.TryGetPaintable(out var paintable))
            {
                View.PaintableName = paintable.Data.Name;
                View.PaintableIcon = paintable.Data.Icon;
            }
            else
            {
                View.PaintableName = null;
                View.PaintableIcon = null;
            }
        }

        protected override void OnViewHideEntered()
        {
            base.OnViewHideEntered();

            Game.RemoveListener<PaintAmountChangedMessage>(OnPaintAmountChanged);
            Game.RemoveListener<CurrentPaintableChangedMessage>(OnCurrentPaintableChanged);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Game.RemoveListener<PaintAmountChangedMessage>(OnPaintAmountChanged);
            Game.RemoveListener<CurrentPaintableChangedMessage>(OnCurrentPaintableChanged);
        }

        private void OnPaintAmountChanged(PaintAmountChangedMessage message)
        {
            View.PaintAmount = message.PaintAmount;
        }

        private void OnCurrentPaintableChanged(CurrentPaintableChangedMessage message)
        {
            var paintable = message.Paintable;
            if (paintable)
            {
                View.PaintableName = paintable.Data.Name;
                View.PaintableIcon = paintable.Data.Icon;
            }
            else
            {
                View.PaintableName = null;
                View.PaintableIcon = null;
            }
        }
    }
}
