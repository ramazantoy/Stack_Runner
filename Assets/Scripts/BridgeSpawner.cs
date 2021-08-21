using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//doğru boyutlandırma ve konumlandırma
public class BridgeSpawner : MonoBehaviour
{
    public GameObject startReferance, endReferance;// start ve finish isimli  colider objeleri
    public BoxCollider hiddenPlatform;
    void Start()
    {
        Vector3 direction = endReferance.transform.position - startReferance.transform.position;//aradaki fark yön vektörü
        float distance = direction.magnitude;//tön vektörünün uzunluğu
        direction = direction.normalized;
        hiddenPlatform.transform.forward = direction;//box colider'in yönünün ayarlanması
        hiddenPlatform.size = new Vector3(hiddenPlatform.size.x, hiddenPlatform.size.y, distance);//aradaki mesafe kadar z değişkeni genişlesin
        hiddenPlatform.transform.position = startReferance.transform.position + (direction * distance / 2)+(new Vector3(0,-direction.z,direction.y)*(hiddenPlatform.size.y/2));// pozisyonun iki noktanın ortasına gelmesini sağlamak


                                             
    }

}
