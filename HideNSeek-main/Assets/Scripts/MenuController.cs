using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject DailyBonusPanel;
    public GameObject MissionPanel;
    public GameObject RewardPanel;
    public GameObject UnAdsPackagePanel;
    public GameObject CoinPackagePanel;
    public GameObject SavingPackagePanel;

    private GameObject currentPanel;

    public void OnClickDailyBonusPanel()
    {
        currentPanel = DailyBonusPanel;
        DailyBonusPanel.SetActive(true);
    }
    public void OnClickMissionPanel()
    {
        currentPanel = MissionPanel;
        MissionPanel.SetActive(true);
    }
    public void OnClickRewardPanel()
    {
        currentPanel = RewardPanel;
        RewardPanel.SetActive(true);
    }
    public void OnClickUnAdsPackagePanel()
    {
        currentPanel = UnAdsPackagePanel;
        UnAdsPackagePanel.SetActive(true);
    }
    public void OnClickSavingPackagePanel()
    {
        currentPanel = SavingPackagePanel;
        SavingPackagePanel.SetActive(true); 
    }

    public void OnClickClosePanel()
    {
        if(currentPanel!=null)
        currentPanel.SetActive(false);
    }

}
