using UnityEngine;
using UnityEngine.UI;

public class SoulBarUI : MonoBehaviour
{
    [SerializeField] private Image soulFill;
    [SerializeField] private Image ultimateOverlay;

    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerUltimate playerUltimate;

    private float soulPercent;

    public void UpdateBarState()
    {
        soulPercent = UpdateSoulBar(soulPercent);
        UpdateUltimateZone(soulPercent);
    }

    private float UpdateSoulBar(float soulPercent)
    {
        soulPercent =
            (float)playerController.points / playerController.MaxPoints;

        soulFill.fillAmount = soulPercent;
        return soulPercent;
    }

    private void UpdateUltimateZone(float soulPercent)
    {
        float costPercent =
            (float)playerUltimate.UltimateCost / playerController.MaxPoints;

        ultimateOverlay.fillAmount = costPercent;

        if (soulPercent > costPercent)
        {
            ultimateOverlay.transform.SetAsLastSibling();
            ultimateOverlay.color = Color.yellow;
            ultimateOverlay.material.SetColor("_Color", new Color(5, 5, 5, 0));

        }
        else
        {
            ultimateOverlay.transform.SetAsFirstSibling();
            ultimateOverlay.color = new Color32(12, 28, 79, 255);
            ultimateOverlay.material.SetColor ("_Color", new Color(0, 0, 0, 0));
        }
    }
}