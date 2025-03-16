using UnityEngine;
using UnityEngine.UI;

public class RefreshUI : MonoBehaviour
{
    void LateUpdate()
    {
        //Force redraw all UI child

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);

        var self = transform as RectTransform;
        self.ForceUpdateRectTransforms();
    }
}
