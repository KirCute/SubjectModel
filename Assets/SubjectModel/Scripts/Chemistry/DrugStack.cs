using System;
using System.Collections.Generic;
using System.Linq;
using SubjectModel.Scripts.Firearms;
using SubjectModel.Scripts.InventorySystem;
using UnityEngine;

namespace SubjectModel.Scripts.Chemistry
{
    public class IonStack
    {
        private const int MaxPartner = 10;

        public Element Element;
        public int Index;
        public float Amount;
        public float Concentration;
        public float DropTime;

        public string GetSymbol(int properties)
        {
            if (Element.valences[Index] == 0)
                return $"{Element.symbol}{Utils.ToInteger(Element.combination[properties][Index])}";
            var fCombination = Element.combination[properties][Index];
            var iPartner = FindPartner(fCombination);
            var iCombination = Utils.ToInteger(fCombination * iPartner);
            var partner = iPartner == 1 ? "" : iPartner.ToString();
            var combination = iCombination == 1 ? "" : iCombination.ToString();
            var charge = Element.GetCharge(Index, properties) * iPartner;
            var chargeStr = charge == 0
                ? ""
                : Math.Abs(charge) == 1
                    ? $"{(charge >= 0 ? "+" : "-")}"
                    : $"{Math.Abs(charge)}{(charge >= 0 ? "+" : "-")}";
            return iCombination == 0
                ? $"{Element.symbol} {chargeStr}"
                : fCombination > .0f
                    ? $"{Element.symbol}{partner}O{combination} {chargeStr}"
                    : $"{Element.symbol}{partner}H{combination} {chargeStr}";
        }

        private static int FindPartner(float num)
        {
            for (var i = 1; i < MaxPartner; i++)
                if (Utils.IsInteger(num * i))
                    return i;
            return 1;
        }
    }

    public class DrugStack : IItemStack, IFiller
    {
        public readonly string Tag;
        public readonly IList<IonStack> Ions;
        public readonly int Properties;
        private int count;

        public DrugStack(string tag, IList<IonStack> ions, int properties, int count)
        {
            Tag = tag;
            Ions = ions;
            Properties = properties;
            this.count = count;
        }

        public void Merge(IItemStack item)
        {
            count += ((DrugStack) item).count;
        }

        public IItemStack Fetch(int c)
        {
            if (c > count) c = count;
            var ions = Ions.Select(ion => new IonStack
                    {Element = ion.Element, Index = ion.Index, Amount = ion.Amount, Concentration = ion.Concentration})
                .ToList();
            count -= c;
            return new DrugStack(Tag, ions, Properties, c);
        }

        public int GetCount()
        {
            return count;
        }

        public void CountAppend(int c)
        {
            count += c;
        }

        public bool CanMerge(IItemStack item)
        {
            if (item.GetType() != typeof(DrugStack)) return false;
            var drug = (DrugStack) item;
            if (drug.Ions.Count != Ions.Count || drug.Tag != Tag || drug.Properties != Properties) return false;
            return Ions.All(ion => drug.Ions.Any(i =>
                ion.Element == i.Element && ion.Index == i.Index && Math.Abs(ion.Amount - i.Amount) < 0.000001f &&
                Math.Abs(ion.Concentration - i.Concentration) < 0.000001f));
        }

        public string GetName()
        {
            return $"{Tag}({count})";
        }

        public void OnMasterUseKeep(GameObject user, Vector2 pos)
        {
        }

        public void OnMasterUseOnce(GameObject user, Vector2 pos)
        {
            if (Camera.main == null) return;
            BuffInvoker.InvokeByThrower(this, pos, user.GetComponent<Rigidbody2D>().position);
        }

        public void OnSlaveUseKeep(GameObject user)
        {
        }

        public void OnSlaveUseOnce(GameObject user)
        {
            if (Camera.main == null) return;
            BuffInvoker.InvokeByThrower(this, user.GetComponent<Rigidbody2D>().position,
                user.GetComponent<Rigidbody2D>().position);
        }

        public void Selecting(GameObject user)
        {
        }

        public void OnSelected(GameObject user)
        {
        }

        public void LoseSelected(GameObject user)
        {
        }

        public Func<IItemStack, bool> SubInventory()
        {
            return item => false;
        }

        public void OnBulletHit(GameObject target)
        {
            if (!target.TryGetComponent<BuffRenderer>(out var br)) return;
            br.Register((DrugStack) Fetch(1));
        }

        public string GetFillerName()
        {
            return Tag;
        }

        public bool Equals(IFiller other)
        {
            return other != null && other.GetType() == typeof(DrugStack) && CanMerge((DrugStack) other);
        }
    }
}