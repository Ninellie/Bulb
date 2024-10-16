using System.Collections.Generic;
using UnityEngine;

namespace FirearmComponents
{
    public class AmmoAdder : MonoBehaviour
    {
        public List<AmmoData> ammo;
        public Magazine magazine;

        public void AddAmmo()
        {
            magazine.AddAmmoData(ammo);
        }
    }
}