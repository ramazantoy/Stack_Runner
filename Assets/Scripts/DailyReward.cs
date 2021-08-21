using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyReward : MonoBehaviour
{
    //Günlük Ödül Script'i
    //Zamanı ifade etmek için ticks kullanılır
    //
    /*
    void Start()
    {
        long time = System.DateTime.Now.Ticks;//günümüz  tarihinin tick tipinde çekimi
    }*/
    public bool initialized;//Tanımlanmış mı
    public long rewardGivingTimeTicks;//ödülü alma zamanı tick'i
    public GameObject rewardMenu;// ödül menusu
    public Text remainingTimeText;
    public void InitalizeDailyReward()//Günlük ödülü  tanımlama fonksiyonu
    {

        if (PlayerPrefs.HasKey("lastDailyReward"))  //Böyle bir tanımlama var mı
        {//long büyük olduğu için string cinsinde saklıyoruz
           rewardGivingTimeTicks = long.Parse(PlayerPrefs.GetString("lastDailyReward"))+864000000000;
            //ödülü en son aldığı zaman + ticks cinsinden bir gün değeri
            long currentTime = System.DateTime.Now.Ticks;//şimdi ki zaman
            if (currentTime >= rewardGivingTimeTicks)//son ödülden bir gün geçmiş ise
            {
                GiveReward();
            }

        }
        else
        {
            GiveReward();
        }
          
        initialized = true;
    }
    public void GiveReward()
    {
        LevelController.Current.giveMoney(100);
        rewardMenu.SetActive(true);
        PlayerPrefs.SetString("lastDailyReward",System.DateTime.Now.Ticks.ToString());// en son alınan tarihin güncellenmesi
        rewardGivingTimeTicks = long.Parse(PlayerPrefs.GetString("lastDailyReward")) + 864000000000;//sonraki ödül zamanı
    }


    void Update()
    {
        if (initialized)
        {
            if (LevelController.Current.startMenu.activeInHierarchy)
            {
                long currentTime = System.DateTime.Now.Ticks;//şimdi ki zaman
                long remainingTime = rewardGivingTimeTicks - currentTime;//kalan zamanın hesaplanması
                if (remainingTime <= 0)
                {
                    GiveReward();
                }
                else
                {
                    System.TimeSpan timeSpan = System.TimeSpan.FromTicks(remainingTime);//time span yardımıyla ticks birimlerinin kolaylıkla dönüşmesi
                    //timeSpan.Hours.ToString("D2")  gelen değer iki basamaktan küçük olsa bile d2 ile 2 basamaklı çıktı alınacak
                    remainingTimeText.text = string.Format("{0}:{1}:{2}", timeSpan.Hours.ToString("D2"), timeSpan.Minutes.ToString("D2"), timeSpan.Seconds.ToString("D2")); 

                }
            }

        }
    }
    public void TapToReturnButton()
    {
        rewardMenu.SetActive(false);
    }
}
