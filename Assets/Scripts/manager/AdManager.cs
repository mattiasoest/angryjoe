using System;
using UnityEngine;
using UnityEngine.Monetization;

public class AdManager : MonoBehaviour {

    public static AdManager instance;

    public readonly string PLAY_STORE_ID = "3378602";
    public readonly string APP_STORE_ID = "3378603";

    private static readonly string VIDEO_ID = "video";
    private static readonly string REWARDED_VIDEO_ID = "rewardedVideo";
    private static readonly string BANNER_ID = "topBanner";

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
                videoAd.Show(result => {
                    // TODO ? Dont care?
                });
            }
        }
    }

    public void PlayRewardedAd(Action<ShowResult> resultHandler) {
        if (Monetization.IsReady(REWARDED_VIDEO_ID)) {
            ShowAdPlacementContent rewardedAd = (ShowAdPlacementContent)Monetization.GetPlacementContent(REWARDED_VIDEO_ID);

            if (rewardedAd != null) {
                rewardedAd.Show((result) => {
                    resultHandler(result);
                });
            }
        }
    }

    public void DestroyBannerAd() {
        UnityEngine.Advertisements.Advertisement.Banner.Hide(true);
    }

    private void InitalizeVideoAds() {
#if UNITY_ANDROID
        Monetization.Initialize(PLAY_STORE_ID, isTestAd);
#endif
#if UNITY_IOS
        Monetization.Initialize(APP_STORE_ID, isTestAd);
#endif
    }

    private System.Collections.IEnumerator ShowBannerWhenReady() {
        while (!UnityEngine.Advertisements.Advertisement.IsReady(BANNER_ID)) {
            yield return new WaitForSeconds(0.5f);
        }

        UnityEngine.Advertisements.Advertisement.Banner.
        SetPosition(UnityEngine.Advertisements.BannerPosition.TOP_CENTER);

        UnityEngine.Advertisements.Advertisement.Banner.Show(BANNER_ID);
    }
}