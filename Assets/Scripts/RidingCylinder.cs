using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RidingCylinder : MonoBehaviour
{
    private bool _filled;
    private float _value;//şişme değeri
  public void IncrementCylinderVolume(float value)//silindir hacmini arttırma fonksiyonu
    {
        _value += value;
        if (_value >1)//silindirin boyunu 1 yap 1'den ne kadar büyük ise yeni bir silinir yap
        {
            float leftValue = _value - 1;//kalan değer
            int cylinderCount = CharacterController.Current.cylinders.Count;
            transform.localPosition = new Vector3(transform.localPosition.x,-0.5f * (cylinderCount - 1)- 0.25f, transform.localPosition.z);
            transform.localScale = new Vector3(0.5f, transform.localScale.y, 0.5f);// tam şişirme değerleri
            CharacterController.Current.CreateCylinder(leftValue);//kalan büyüklükte bir silindir yarat 
        }
        else if (_value < 0)
        {
            //silindiri yok et
            CharacterController.Current.DestroyCylinder(this);
        }
        else
        {
            //(silindirsayisi-1) * -0.5f  + büyüklükDeğeri * -0.25f
            //0.5*büyüklük değeri
            //silindir boyutunu güncelle
            int cylinderCount = CharacterController.Current.cylinders.Count;
            transform.localPosition = new Vector3(transform.localPosition.x, -0.5f * (cylinderCount - 1) - 0.25f*_value, transform.localPosition.z);
            transform.localScale = new Vector3(0.5f*_value, transform.localScale.y, 0.5f*_value);// tam şişirme değerleri
        }
    }

}
