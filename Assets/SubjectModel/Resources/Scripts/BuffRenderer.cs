using System;
using System.Collections.Generic;
using System.Linq;
using Bolt;
using UnityEngine;

namespace SubjectModel
{
    [RequireComponent(typeof(Variables))]
    public class BuffRenderer : MonoBehaviour
    {
        private List<IBuff> buffs;

        private void Start()
        {
            buffs = new List<IBuff>();
        }

        private void Update()
        {
            for (var i = 0; i < buffs.Count; i++)
            {
                var buff = buffs[i];
                buff.Update(gameObject);
                if (buff.AfterDelay(gameObject)) buff.UpdateAfterDelay(gameObject);
                if (!buff.Ended(gameObject)) continue;
                Remove(buff);
                i--;
            }
        }

        public void Clear()
        {
            foreach (var buff in buffs) buff.Destroy(gameObject);
            buffs.Clear();
        }

        private void Add(IBuff buff)
        {
            buff.Appear(gameObject);
            buffs.Add(buff);
        }

        private void Remove(IBuff buff)
        {
            buff.Destroy(gameObject); 
            buffs.Remove(buff);
        }

        public void Apply(IBuff buff)
        {
            foreach (var b in buffs.Where(b => b.GetType() == buff.GetType()))
            {
                if (b.GetLevel() > buff.GetLevel())
                {
                    return;
                }
                if (Math.Abs(b.GetLevel() - buff.GetLevel()) < .0001f && b.GetTotalTime() - b.GetRemainedTime() <= buff.GetTotalTime())
                {
                    b.Append(buff.GetTotalTime());
                    return;
                }
                b.LevelUp(gameObject, buff.GetLevel());
                b.Append(buff.GetTotalTime());
            }
            Add(buff);
        }
    }
}
