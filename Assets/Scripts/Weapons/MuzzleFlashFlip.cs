using UnityEngine;

public class MuzzleFlashFlip : MonoBehaviour
{
    [SerializeField] Vector3 muzzlePosition = Vector3.zero;
    [SerializeField] Vector3 muzzleOffset = Vector3.zero;
    [SerializeField] SpriteRenderer _weaponSR;
    [SerializeField] Transform _muzzleTR;

    // Update is called once per frame
    void Update()
    {
        if (_weaponSR.flipY)
        {
            _muzzleTR.localPosition = muzzlePosition + muzzleOffset;
        }
        else
        {
            _muzzleTR.localPosition = muzzlePosition;
        }
    }
}
