using System;
using System.Collections.Generic;
using UnityEngine;

namespace SubjectModel.Scripts.Firearms
{
    /*
    new Gun {name = "模板1", damage = 90f, loadingTime = .8f, clipCapacity = 4, clipSwitchTime = 5f}
    */
    public interface IFiller
    {
        public void OnBulletHit(GameObject target);
        public string GetFillerName();
        public bool Equals(IFiller other);
        public int GetCount();
        public void CountAppend(int count);
    }

    [Serializable]
    public class GunData
    {
        public List<FirearmTemple> firearmTemples;
        public List<MagazineTemple> magazineTemples;
        public List<BulletTemple> bulletTemples;
    }
}