using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickLightUI : MonoBehaviour
{

    [SerializeField]
    GameObject _pickUpHand;

    [SerializeField]
    GameObject _grabbingHand;
    public void toggleGrabbing(bool enable, bool isGrabbing)
    {
        this._pickUpHand.SetActive(enable ? !isGrabbing : false);
        this._grabbingHand.SetActive(enable ? isGrabbing : false);
    }
}
