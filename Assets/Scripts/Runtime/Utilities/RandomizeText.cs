using System.Collections.Generic;
using InSun.GameCore.Utilities;
using TMPro;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Utilities
{
    [RequireComponent(typeof(TMP_Text))]
    internal sealed class RandomizeText : MonoBehaviour
    {
        [SerializeField]
        private List<string> entries;

        private TMP_Text textContent;

        private void Awake()
        {
            textContent = GetComponent<TMP_Text>();
        }

        private void Start()
        {
            if (entries.TryGetRandom(out var randomText))
            {
                textContent.text = randomText;
            }
        }
    }
}
