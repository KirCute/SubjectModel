using System;
using System.Collections.Generic;
using System.Linq;
using Bolt;
using SubjectModel.Scripts.InventorySystem;
using UnityEngine;

namespace SubjectModel.Scripts.Subject.Firearms
{
    /**
     * <summary>
     * 枪支系统工具类
     * 枪支的每个部件（枪支本身，弹匣，子弹）均有模板类和物品类，模板类的实例在游戏加载时初始化，规定了各种部件的规格数据，而部件的物品类均实现IItemStack，其实例均为该模板下的具体物品。部件的模板可在此类的Temples容器中获取
     * 同时，不同的枪有不同的使用方式（如AR支持连射而RF不支持，RF无弹匣等），在枪支初始化时，会自动调用此类FirearmBuilder中的方法，来对其使用方法进行初始化。
     * 不同枪的使用方法，详见FirearmBuilder
     * </summary>
     */
    public static class FirearmDictionary
    {
        public static List<FirearmTemple> FirearmTemples; // = new List<FirearmTemple>();

        public static List<MagazineTemple> MagazineTemples; /* = new List<MagazineTemple>
        {
            new MagazineTemple("弹匣0", 20, 1f, 1f, 1f),
            new MagazineTemple("弹匣1", 100, 3f, 1f, 1f)
        };
        //*/

        public static List<BulletTemple> BulletTemples; /* = new List<BulletTemple>
        {
            new BulletTemple("子弹0", 37.5f, 0f, 50f, 0f, 1f, 1f),
            new BulletTemple("子弹1", 6.25f, 87.5f, 200f, 120f, 1f, 1f),
            new BulletTemple("子弹2", 10f, 0f, 100f, 0f, 1f, 1f)
        };
        //*/

        public static readonly Func<Firearm, string> DefaultName = firearm => //默认的名称显示方式
        {
            var fn = firearm.Temple.Name; //名称
            var l = firearm.Loading > .0f; //是否正在装填
            var m = firearm.Magazine; //正在使用的弹匣名称
            var b = m?.Containing; //子弹
            var bn = b?.Temple.Name; //子弹名称
            var bf = b?.Filler; //子弹内容物
            var bc = b?.Count; //剩余弹药数
            var mc = m?.Temple.BulletContains; //可装填总弹药数
            return m == null
                ? $"{fn}{(l ? "(装填中)" : "")}"
                : b == null
                    ? $"{fn}({m.Temple.Name})"
                    : bf == null
                        ? $"{fn}({bn} {bc}/{mc}{(l ? " 装填中" : "")})"
                        : $"{fn}({bn} {bf.FillerName} {bc}/{mc}{(l ? " 装填中" : "")})";
        };

        private static readonly Action<Firearm, GameObject, Vector2> DefaultShoot = (firearm, user, aim) => //默认射击方式，以下为设计射击模式时的一般步骤
        {
            //生成误差
            if (firearm.Loading > .0f || firearm.Magazine?.Containing == null || Camera.main == null) return;
            var shooterPosition = user.GetComponent<Rigidbody2D>().position; //射击起点
            if (shooterPosition == aim) return;
            aim = Utils.GetRotatedVector(aim - shooterPosition, //旋转
                Utils.GenerateGaussian(.0f, firearm.Deviation, //随机角度
                    (float) (Math.PI / firearm.Temple.MaxRange))) + shooterPosition; //生成误差
            //子弹消耗，计算装填时间，计算后坐力
            firearm.Magazine.Containing.Count--; //子弹消耗
            if (firearm.Magazine.Containing.Count != 0) firearm.Loading = firearm.Temple.Loading; //开始计算装填时间
            firearm.InvokeKick(user); //开始计算后坐力
            //利用GunFlash射击
            var collider = user.GetComponent<GunFlash>().Shoot(shooterPosition, aim); //显示闪光效果，同时得到命中情况
            //扣血，附加填充物效果
            if (collider != null) //若命中了作战单位
            {
                var declarations = collider.GetComponent<Variables>().declarations;
                var defence = declarations.IsDefined("Defence") ? declarations.Get<float>("Defence") : .0f; //得到防御
                var health = declarations.Get<float>("Health"); //得到血量
                var depth = firearm.Magazine.Containing.Filler == null //得到子弹穿深
                    ? firearm.Temple.Depth + firearm.Magazine.Containing.Temple.Depth //无填充物
                    : 0f; //有填充物时，子弹的穿深作废，以0计
                var minDefence = firearm.Magazine.Containing.Temple.MinDefence; //得到过穿防御
                var damage = firearm.Temple.Damage * firearm.Magazine.Containing.Temple.BreakDamage; //得到基础伤害
                var explode = firearm.Magazine.Containing.Temple.Explode; //得到击穿伤害
                declarations.Set("Health", //扣血
                    health - (defence > depth
                            ? Utils.Map(.0f, defence, .0f, damage, depth) // 未击穿
                            : depth - defence <= minDefence || minDefence < 0.001f // 未过穿 或 不存在过穿可能
                                ? damage + (firearm.Magazine.Containing.Filler == null ? explode : .0f) // 刚好击穿
                                : damage // 过穿
                    )
                );
                /*if (defence <= depth) */ //在早期版本中，只有击穿时填充物才能对作战单位附加效果
                firearm.Magazine.Containing.Filler?.OnBulletHit(collider.gameObject); //为作战单位附加效果（若子弹有填充物）
            }
            //若子弹打光，清除弹匣中的子弹信息
            if (firearm.Magazine.Containing.Count == 0) firearm.Magazine.Containing = null;
        };

        public static readonly Action<Firearm, GameObject> DefaultReload = (firearm, user) => //默认重装填方式，以下为设计射击模式时的一般步骤，注意此方法并未完成整个装填过程，在重装填时间以后的步骤见CompleteReload
        {
            //判断是否允许重装填
            if (firearm.SwitchingMagazine) return; 
            //得到装填物（AR弹匣，RF子弹），预备装填物，设置装填时间，作战单位移动减速
            if (user.GetComponent<Inventory>().TryGetSubItem(out var ready)) //得到子物品
            {
                firearm.Ready = ready; //预备装填物
                firearm.SwitchingMagazine = true;
                firearm.Loading = firearm.Temple.Reload; //设置装填时间
                var declarations = user.GetComponent<Variables>().declarations;
                declarations.Set("Speed", declarations.Get<float>("Speed") * firearm.Temple.ReloadSpeed); //作战单位装填时移动减速
            }
            //退出原有弹匣，重置重量
            if (firearm.Magazine != null) firearm.AddWeight(user, -firearm.Magazine.Temple.Weight);
            user.GetComponent<Inventory>().Add(firearm.Magazine);
            firearm.Magazine = null;
        };

        public static readonly Func<Firearm, Func<IItemStack, bool>> DefaultSub = firearm => item =>
            item is Magazine m && firearm.Temple.Magazine.Contains(m.Temple.Name);

        public static readonly Action<Firearm, GameObject> DefaultCompleteReload = (firearm, user) => //装填完成方法
        {
            var ready = (Magazine) firearm.Ready;
            firearm.Magazine = ready; //装填预备弹匣
            firearm.AddWeight(user, firearm.Magazine.Temple.Weight); //重置重量
            user.GetComponent<Inventory>().Remove(ready); //从物品栏中删除装填物
            firearm.Ready = null;
            var declarations = user.GetComponent<Variables>().declarations;
            declarations.Set("Speed", declarations.Get<float>("Speed") / firearm.Temple.ReloadSpeed); //重置移动速度（消除装填时移动减速）
        };

        public static readonly Action<Firearm, GameObject, Vector2> NoShoot = (firearm, user, aim) => { };

        public static readonly Action<Firearm, GameObject> NoReload = (firearm, user) => { };

        private static readonly string[] Type = {"Unknown", "AR", "RF"};

        public static readonly Action<Firearm>[] FirearmBuilder =
        {
            firearm => { }, //Unknown
            firearm => //AR
            {
                firearm.ShootKeep = DefaultShoot;
                firearm.Reload = DefaultReload;
            },
            firearm => //RF
            {
                firearm.Magazine = new Magazine(MagazineTemples
                    .Where(item => item.Name == firearm.Temple.Magazine[0]).FirstOrDefault()); //自动获得桥架
                firearm.NameGetter = gun =>
                {
                    var fn = gun.Temple.Name;
                    var l = gun.Loading > .0f;
                    var b = gun.Magazine.Containing;
                    var bn = b?.Temple.Name;
                    var bf = b?.Filler;
                    var bc = b?.Count;
                    var mc = gun.Magazine.Temple.BulletContains;
                    return b == null
                        ? $"{fn}{(l ? "(装填中)" : "")}"
                        : bf == null
                            ? $"{fn}({bn} {bc}/{mc}{(l ? " 装填中" : "")})"
                            : $"{fn}({bn} {bf.FillerName} {bc}/{mc}{(l ? " 装填中" : "")})";
                };
                firearm.ShootOnce = DefaultShoot;
                firearm.Sub = gun => gun.Magazine.AppropriateBullet();
                firearm.Reload = (gun, user) =>
                {
                    if (gun.SwitchingMagazine) return;
                    if (user.GetComponent<Inventory>().TryGetSubItem(out var ready))
                    {
                        var bullet = (Bullet) ready;
                        if (gun.Magazine.Containing == null || gun.Magazine.Containing.Is(bullet)) return;
                    }

                    user.GetComponent<Inventory>().Add(gun.Magazine.Containing);
                    gun.Magazine.Containing = null;
                };
                firearm.ReloadKeep = (gun, user) =>
                {
                    if (gun.SwitchingMagazine || !user.GetComponent<Inventory>().TryGetSubItem(out var ready)) return;
                    var bullet = (Bullet) ready;
                    if (gun.Magazine.Containing != null && (!gun.Magazine.Containing.Is(bullet) || 
                                                            gun.Magazine.Containing.Count >= 
                                                            gun.Magazine.Temple.BulletContains)) return;
                    gun.Ready = bullet;
                    gun.SwitchingMagazine = true;
                    gun.Loading = gun.Temple.Reload;
                    var declarations = user.GetComponent<Variables>().declarations;
                    declarations.Set("Speed", declarations.Get<float>("Speed") * gun.Temple.ReloadSpeed);
                };
                firearm.CompleteReload = (gun, user) =>
                {
                    var bullet = (Bullet) gun.Ready.Fetch(1);
                    if (gun.Magazine.Containing == null) gun.Magazine.Containing = bullet;
                    else gun.Magazine.Containing.Count++;
                    gun.Ready = null;
                    var declarations = user.GetComponent<Variables>().declarations;
                    declarations.Set("Speed", declarations.Get<float>("Speed") / gun.Temple.ReloadSpeed);
                };
            }
        };

        public static int TypeIndexOf(string type)
        {
            for (var i = 0; i < Type.Length; i++)
                if (Type[i].Equals(type))
                    return i;
            return 0;
        }

        public static string GetFirearmType(int type)
        {
            return Type[type];
        }
    }

    [Serializable]
    public class GunData
    {
        public List<FirearmTemple> firearmTemples;
        public List<MagazineTemple> magazineTemples;
        public List<BulletTemple> bulletTemples;
    }
}