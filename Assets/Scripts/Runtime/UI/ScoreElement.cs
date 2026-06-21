using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DoubleD.VerySeriousJamGame.Runtime.UI
{
    internal sealed class ScoreElement : MonoBehaviour
    {
        [SerializeField]
        private Image iconImage;

        [SerializeField]
        private TMP_Text scoreText;

        public Sprite Icon
        {
            set => iconImage.sprite = value;
        }

        public string ScoreText
        {
            set => scoreText.text = value;
        }
    }
}
