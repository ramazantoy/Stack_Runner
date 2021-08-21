using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    public static LevelController Current;
    public bool gameActive = false;
    public GameObject startMenu, gameMenu, gameOverMenu, finishMenu;
    public Text scoretext, finishscoreText, currentlevelText, nextlevelText,startingMenuMoneyText,gameoverMenuMoneyText,finishGameMenuMoneyText;
    int currentLevel;
    public int score;
    public Slider levelProgressBar;
    public float maxDistance;
    public GameObject finishLine;
    public AudioSource gameMusicAudioSource;
    public AudioClip victoryAudioClip, gameOverAudioClip;
    public DailyReward dailyReward;
    public Button RewardAdVideoButton;
    void Start()
    {
   
        Current = this;
       currentLevel= PlayerPrefs.GetInt("currentlevel");
        //Debug.Log("start current : " + currentLevel);
            CharacterController.Current = GameObject.FindObjectOfType<CharacterController>();
            GameObject.FindObjectOfType<MarketController>().initalizeMarketController();
            dailyReward.InitalizeDailyReward();
            currentlevelText.text = "" + (currentLevel + 1);
           nextlevelText.text = "" + (currentLevel + 2);
            UpdateMoneyText();
            //giveMoney(3000);
        gameMusicAudioSource = Camera.main.GetComponent<AudioSource>();
        if (AdController.Current.IsReadyInterstitialAd())//geçiş reklamı yüklenmiş mi
        {
            AdController.Current.interstitial.Show();
        }
    }
    public void showRewardedAd()
    {
        if (AdController.Current.rewardedAd.IsLoaded())//ödül reklamı hazır ise
        {
            AdController.Current.rewardedAd.Show();
        }
    }


    void Update()
    {
        if (gameActive)
        {
            CharacterController player = CharacterController.Current;
            float distance= finishLine.transform.position.z - CharacterController.Current.transform.position.z;
            levelProgressBar.value = 1 - (distance / maxDistance);// bar doldurma
        }
        
    }
    public void StartLevel()
    {
        AdController.Current.bannerView.Hide();//oyun başladığında reklamın gizlenmesi amacıyla
        maxDistance = finishLine.transform.position.z - CharacterController.Current.transform.position.z;
        CharacterController.Current.ChangeSpeed(CharacterController.Current.runningSpeed);
        CharacterController.Current.playerAnimator.SetBool("running", true);
        startMenu.SetActive(false);
        gameMenu.SetActive(true);
        gameActive = true;
    }
    public void restartLevel()
    {
        LevelLoader.Current.ChangeLevel(this.gameObject.scene.name);
    }
    public void loadNextLevel()
    {
        // PlayerPrefs.SetInt("currentlevel",currentLevel + 1);
        LevelLoader.Current.ChangeLevel("Level " + (currentLevel + 1));
    }
    public void gameOver()
    {
        if (AdController.Current.IsReadyInterstitialAd())//geçiş reklamı yüklenmiş mi
        {
            AdController.Current.interstitial.Show();
        }
        AdController.Current.bannerView.Show();//reklamın gösterilmesi
        UpdateMoneyText();
        gameMusicAudioSource.Stop();
        gameMusicAudioSource.PlayOneShot(gameOverAudioClip);
        gameMenu.SetActive(false);
        gameOverMenu.SetActive(true);
        gameActive = false;

    }
    public void FinishGame()
    {
        if (AdController.Current.rewardedAd.IsLoaded())
        {
            RewardAdVideoButton.gameObject.SetActive(true);
        }
        else
        {
            RewardAdVideoButton.gameObject.SetActive(false);
        }
        AdController.Current.bannerView.Show();//reklamın gösterilmesi
        giveMoney(score);
        gameMusicAudioSource.Stop();
        gameMusicAudioSource.PlayOneShot(victoryAudioClip);
        PlayerPrefs.SetInt("currentlevel", currentLevel + 1);
      //  Debug.Log("sa");
       // Debug.Log(PlayerPrefs.GetInt("currentlevel"));
        finishscoreText.text = "" + score;
        gameMenu.SetActive(false);
        finishMenu.SetActive(true);
        gameActive = false;

    }
    public void  changeScore(int value)
    {
        score += value;
        scoretext.text = "" + score;
    }
    public void UpdateMoneyText()
    {
        int money = PlayerPrefs.GetInt("money", 0);
        startingMenuMoneyText.text = "" + money;
       gameoverMenuMoneyText.text = "" + money;
       finishGameMenuMoneyText.text = "" + money;
    }
    public void giveMoney(int increment)
    {
        int money = PlayerPrefs.GetInt("money", 0);
        money = Mathf.Max(0,money + increment);
        PlayerPrefs.SetInt("money", money);
        UpdateMoneyText();
    }
}
