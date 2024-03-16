using UnityEngine;

namespace TheHuntIsOn;

internal class ToggleCameraLock : MonoBehaviour
{
    private GameObject _cameraLock;

    void Start()
    {
        _cameraLock = transform.Find("CamLock Main").gameObject;
    }

    void Update()
    {
        if (_cameraLock.activeSelf && HeroController.instance.transform.position.y > 28)
            _cameraLock.SetActive(false);
        else if (!_cameraLock.activeSelf && HeroController.instance.transform.position.y <= 28)
            _cameraLock.SetActive(true);
    }
}
