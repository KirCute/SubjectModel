using System;
using System.Collections.Generic;
using System.Linq;
using SubjectModel.Scripts.Firearms;
using SubjectModel.Scripts.InventorySystem;
using UnityEngine;

namespace SubjectModel.Scripts.Chemistry
{
    /**
     * <summary>粒子堆，用于描述一个DrugStack内全部同种粒子（同元素的同价态）的信息</summary>
     */
    public class IonStack
    {
        private const int MaxPartner = 10; //中心原子的最大氧（氢）结合量

        public Element Element; //中心原子的元素
        public int Index; //中心原子化合价序号
        public float Amount; //物质的量
        public float Concentration; //浓度
        public float DropTime; //剩余沾染时间

        /**
         * <summary>得到粒子的化学符号</summary>
         */
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

        /**
         * <summary>
         * 计算中心原子数量的方法
         * <param name="num">中心原子氧（氢）结合数</param>
         * <returns>中心原子数</returns>
         * </summary>
         */
        private static int FindPartner(float num)
        {
            for (var i = 1; i < MaxPartner; i++) //其实就是寻找最小公倍数
                if (Utils.IsInteger(num * i))
                    return i;
            return 1;
        }
    }

    /**
     * <summary>试剂堆，用于描述玻封药品，子弹或其它炼金术容器内的全部内容物</summary>
     */
    public class DrugStack : IThrowable, IFiller
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
            if (!(item is DrugStack)) return false;
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

        public void OnMasterThrow(GameObject user, Vector2 pos)
        {
            if (Camera.main == null) return;
            BuffInvoker.InvokeByThrower(this, pos, user.GetComponent<Rigidbody2D>().position);
        }

        public void OnSlaveThrow(GameObject user)
        {
            if (Camera.main == null) return;
            BuffInvoker.InvokeByThrower(this, user.GetComponent<Rigidbody2D>().position,
                user.GetComponent<Rigidbody2D>().position);
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
            return other is DrugStack o && CanMerge(o);
        }
    }
}