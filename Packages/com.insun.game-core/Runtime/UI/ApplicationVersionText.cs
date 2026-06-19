using TMPro;
using UnityEngine;

namespace InSun.GameCore.UI
{
    internal sealed class ApplicationVersionText : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text contentText;

        private void Reset()
        {
            contentText = GetComponent<TMP_Text>();
        }

        private void Awake()
        {
            if (contentText == false)
            {
                Debug.LogError("No content text set", this);
                enabled = false;
            }
        }

        private void Start()
        {
            contentText.text = Application.version;
        }
    }
}
