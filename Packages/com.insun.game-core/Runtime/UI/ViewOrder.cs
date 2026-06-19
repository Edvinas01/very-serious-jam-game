using InSun.GameCore.SunnyInspector;
using InSun.GameCore.Utilities;
using UnityEngine;

namespace InSun.GameCore.UI
{
    [SunnySettings(MenuPath = "UI")]
    [CreateAssetMenu(
        menuName = MenuConstants.BaseAssetMenuName + "/UI/View Order",
        fileName = MenuConstants.BaseAssetFileName + "Data_ViewOrder"
    )]
    public sealed class ViewOrder : ScriptableObject
    {
        [Min(0)]
        [SerializeField]
        private int sortingOrder;

        public int SortingOrder => sortingOrder;
    }
}
