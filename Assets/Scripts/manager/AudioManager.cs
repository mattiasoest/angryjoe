using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour {

    public static AudioManager instance;

    [HideInInspector]
    public bool isSfxOn = true;
    [HideInInspector]
    public bool isScoreSfxOn = true;

    public Sound[] sounds;

    private void Awake() {
        int sfxVal = PlayerPrefs.GetInt("all_sfx");
        int scoreSfxVal = PlayerPrefs.GetInt("score_sfx");
        // 0 is default for playerprefs, we use 1 and -1 to define the val.
        if (sfxVal != 0) {
            isSfxOn = sfxVal == 1;
        }
        if (scoreSfxVal != 0) {
            isScoreSfxOn = scoreSfxVal == 1;
        }

        instance = this;
    }

    void Start() {
        foreach (Sound sound in sounds) {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
        }
    }

    public void PlayJump() {
        Play($"jump0");
    }

    public void PlayDownForce() {
        Play($"jump1");
    }

    public void PlaySlide() {
        Play("slide0");
    }

    public void PlayDeath() {
        //string index = Random.Range(0, 2).ToString();
        //Play($"death{index}");
        Play("death0");
    }

    public void PlayScore() {
        if (isScoreSfxOn) {
            Play("score0");
        }
    }

    public void PlayStartGame() {
        Play("start0");
    }

    public void PlayHit() {
        Play("hit0");
    }

    public void PlayLeaderboard() {
        Play("leaderboard");
    }

    public void PlayPopup() {
        Play("regular_popup");
    }

    public void PlayToggle() {
        Play("toggle0");
    }

    public void PlayNormalButton() {
        Play("click0");
    }

    public void PlayCloseButton() {
        Play("click1");
    }

    public void PlayStartButton() {
        Play("play_button");
    }

    public void PlayLogin() {
        Play("login0");
    }

    public void PlayGrantItem() {
        Play("reward0");
    }

    public void PlayContinueGame() {
        Play("reward_continue");
    }

    private void Play(string clipName) {
        if (isSfxOn) {
            Sound clip = Array.Find(sounds, s => s.name == clipName);
            if (clip == null) {
                Debug.Log($"Could not find sound name: {clipName}");
            } else {
                clip.source.Play();
            }
        }
    }
}