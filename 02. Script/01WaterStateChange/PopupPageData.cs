using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPopupPageData", menuName = "Popup/PopupPageData")]
public class PopupPageData : ScriptableObject
{
    [System.Serializable]
    public class Page
    {
        [TextArea(3, 6)]
        public string pageText;
    }

    public List<Page> pages = new List<Page>();
}