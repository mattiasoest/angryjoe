using UnityEngine;
using UnityEngine.Monetization;

public class AdManager : MonoBehaviour {

    public static AdManager instance;

    public readonly string PLAY_STORE_ID = "3378602";
    public readonly string APP_STORE_ID = "3378603";

    private static readonly string VIDEO_ID = "video";
    private static readonly string REWARDED_VIDEO_ID = "rewardedVideo";
    private static readonly string BANNER_ID = "topBanner";

    public bool isPlayStore;

    public readonly bool isTestAd = true;

    void Awake() {
        instance = this;
        InitalizeVideoAds();
    }

    void Start() {
        StartCoroutine(ShowBannerWhenReady());
    }

    public void PlayVideoAd() {
        if (Monetization.IsReady(VIDEO_ID)) {
            ShowAdPlacementContent videoAd = (ShowAdPlacementContent)Monetization.GetPlacementContent(VIDEO_ID);

            if (videoAd != null) {
                videoAd.Show(HandleRewardResult);
            }
        }
    }

    public void PlayRewardedAd() {
        if (Monetization.IsReady(REWARDED_VIDEO_ID)) {
            ShowAdPlacementContent rewardedAd = (ShowAdPlacementContent)Monetization.GetPlacementContent(REWARDED_VIDEO_ID);

            if (rewardedAd != null) {
                rewardedAd.Show(HandleRewardResult);
            }
        }
    }

    private void InitalizeVideoAds() {
        if (isPlayStore) {
            Monetization.Initialize(PLAY_STORE_ID, isTestAd);

        } else {
            Monetization.Initialize(APP_STORE_ID, isTestAd);
        }
    }

    private System.Collections.IEnumerator ShowBannerWhenReady() {
        while (!UnityEngine.Advertisements.Advertisement.IsReady(BANNER_ID)) {
            yield return new WaitForSeconds(0.5f);
        }

        UnityEngine.Advertisements.Advertisement.Banner.
        SetPosition(UnityEngine.Advertisements.BannerPosition.TOP_CENTER);

        UnityEngine.Advertisements.Advertisement.Banner.Show(BANNER_ID);
    }

    private void HandleRewardResult(ShowResult result) {
        switch (result) {
            case ShowResult.Finished:
                Debug.Log("REWARD");
                break;
            case ShowResult.Skipped:
                Debug.Log("SKIPPED");
                break;
            case ShowResult.Failed:
                Debug.Log("FAILED");
                break;
        }
    }

}