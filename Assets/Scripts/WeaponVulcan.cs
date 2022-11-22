using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponVulcan : WeaponBase
{
    public BulletVulcan _bulletObject;
    public GameObject _muzzle;
    public GameObject _fireFX;
    public AudioSource _fireLoop;
    public float _damage = 10.0f;
    public float _bulletSpeed = 10.0f;
    public float _fireTerm = 0.1f;

    float _lastFireTime = 0.0f;
    bool _isFire = false;

    public override void SetFire(bool fire, bool playSound)
    {
        if(playSound)
        {
            if(fire)
            {
                AudioManager.SetAudioVolume(_fireLoop, 1.0f);
                AudioManager.PlayAudio(_fireLoop);
            }
            else
            {
                StartCoroutine(AudioManager.FadeAudio(_fireLoop, 0.1f, 0.0f, true));
            }
        }

        _isFire = fire;
    }

    void FixedUpdate()
    {
        if(_isFire)
        {
            float currentTime = Time.time;
            float nextFireTime = _lastFireTime + _fireTerm;
            if(nextFireTime < currentTime)
            {
                BulletVulcan bullet = Instantiate(_bulletObject);
                bullet.CreateBulletVulcan(GetIsOwnerPlayer(), _muzzle.transform, _damage, _bulletSpeed);
                _lastFireTime = currentTime;
            }
        }
    }
}
