using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private float _creatingBridgeTimer;
    //3f ,-5.5f
    public static CharacterController Current;
   public float limitX;
  public float runningSpeed;
    public float xSpeed;
    private float _currentRunnigSpeed;
    public GameObject ridingCylinderPrefab;
    public List<RidingCylinder> cylinders;
    private bool _spawnbridge;// köprü oluşturulsun mu
    public GameObject bridgePiecePrefab;//köprü parçası
    private BridgeSpawner _bridgeSpawner;
    public Animator playerAnimator;
    private bool _finished;
    private float _scoreTimer;
    private float _lastTouchX;
    public AudioSource cylinderAudioSource,triggerAudioSource,itemAudioSource;
    public AudioClip getherAudioClip, dropAudioClip,coinAudioClip,buyAudioClip,equipItemAudioClip,unEquipAudioClip;
    private float _dropsoundTimer;
    public List<GameObject> wearSpots;


    void Update()
    {
        if (LevelController.Current == null || !LevelController.Current.gameActive)
        {
            return;//Update fonksiyonunu burada bitirmek için
        }
        float newX = 0;
        float touchXDelta = 0;
        if (Input.touchCount > 0 )//ekrana ddokunma var ve dokunan parmak hareket halinde ise
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                _lastTouchX = Input.GetTouch(0).position.x;
            }
            else if(Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                touchXDelta = 5*(Input.GetTouch(0).position.x- _lastTouchX  ) / Screen.width;// oyuncunun parmağını ne kadar sağa sola kaydırdığını anlamak amacıyla
                _lastTouchX = Input.GetTouch(0).position.x;

            }

        }/*
        else if (Input.GetMouseButtonDown(0))
        {
            touchXDelta = Input.GetAxisRaw("Mouse X");//mouse'un x düzleminde ne kadar hareket ettiği bilgisini tutar

        }*/
        newX = transform.position.x +( xSpeed * touchXDelta * Time.deltaTime);
        newX = Mathf.Clamp(newX, -5.5f, 3f);//değeri sınırlandırma
        Vector3 newPosition = new Vector3(newX, transform.position.y,transform.position.z+ _currentRunnigSpeed * Time.deltaTime);
        transform.position = newPosition;
        if (_spawnbridge)
        {
            PlayDropSound();
            _creatingBridgeTimer -= Time.deltaTime;
            if (_creatingBridgeTimer < 0)
            {
                _creatingBridgeTimer = 0.01f;
                IncrementCylinderVolume(-0.01f);//silindir değeri küçültme
                GameObject createdBridgePiece = Instantiate(bridgePiecePrefab,this.transform);
                createdBridgePiece.transform.SetParent(null);
                Vector3 direction = _bridgeSpawner.endReferance.transform.position - _bridgeSpawner.startReferance.transform.position;//aradaki fark yön vektörü
                float distance = direction.magnitude;//tön vektörünün uzunluğu
                direction = direction.normalized;
                createdBridgePiece.transform.forward = direction;
                float characterDistance = transform.position.z - _bridgeSpawner.startReferance.transform.position.z;
                characterDistance = Mathf.Clamp(characterDistance, 0, distance);
                Vector3 newPiecePosition = _bridgeSpawner.startReferance.transform.position + direction*characterDistance;
                newPiecePosition.x = transform.position.x;
                createdBridgePiece.transform.position = newPiecePosition;
                if (_finished)
                {
                    _scoreTimer -= Time.deltaTime;
                    if (_scoreTimer < 0)
                    {
                        _scoreTimer = 0.3f;
                        LevelController.Current.changeScore(1);
                    }
                }


            }

        }
    }
    public void ChangeSpeed(float value)
    {
        _currentRunnigSpeed = value;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag== "AddCylinder")
        {
            cylinderAudioSource.PlayOneShot(getherAudioClip, 0.1f);
            IncrementCylinderVolume(0.1f);
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "SpawnBridge")
        {
            StartspawnBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }
        else if (other.gameObject.tag == "StopSpawnBridge")
        {
            StopSpawningBridge();
            if (_finished)
            {
                LevelController.Current.FinishGame();
            }
        }
        else if (other.gameObject.tag == "Finish")
        {
            _finished = true;
            StartspawnBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }
        else if (other.gameObject.tag == "Coin")
        {
            triggerAudioSource.PlayOneShot(coinAudioClip,0.1f);
            other.tag = "Untagged";
            LevelController.Current.changeScore(10);
            Destroy(other.gameObject);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (LevelController.Current.gameActive)
        {
            if (other.gameObject.tag == "Trap")
            {
                PlayDropSound();
                IncrementCylinderVolume(-Time.fixedDeltaTime);
            }
        }
    
    }
    public void IncrementCylinderVolume(float value)//silindir hacmini arttırma fonksiyonu
    {
        if (cylinders.Count == 0)//ayağımızın altında hiç silindir yok ise
        {
            if (value > 0)
            {
                CreateCylinder(value);
            }
            else
            {
                if (_finished)
                {
                    LevelController.Current.FinishGame();
                }
                else
                {
                    Die();
                }
            }
        }
        else
        {
            cylinders[cylinders.Count - 1].IncrementCylinderVolume(value);// girilen değere göre en alttaki silindirin büyüklüğünün güncellenmesi
        }

    }
    public void Die()
    {

        playerAnimator.SetBool("dead", true);
        gameObject.layer = 8;
        Camera.main.transform.SetParent(null);
        LevelController.Current.gameOver();
    }
    public void CreateCylinder(float value)
    {
        RidingCylinder createdCylinder = Instantiate(ridingCylinderPrefab, transform).GetComponent<RidingCylinder>();
        cylinders.Add(createdCylinder);
        createdCylinder.IncrementCylinderVolume(value);
    }
    public void DestroyCylinder(RidingCylinder cylinder)
    {
        cylinders.Remove(cylinder);
        Destroy(cylinder.gameObject);
    }
    public void StartspawnBridge(BridgeSpawner spawner)
    {
        _bridgeSpawner = spawner;
        _spawnbridge = true;
    }
    public void StopSpawningBridge()
    {
        _spawnbridge = false;
    }
    public void PlayDropSound()
    {
        _dropsoundTimer -= Time.deltaTime;
        if (_dropsoundTimer < 0)
        {
            _dropsoundTimer = 0.15f;
            cylinderAudioSource.PlayOneShot(dropAudioClip, 0.1f);
        }


    }
}
