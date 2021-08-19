using SubjectModel.Scripts.Firearms;
using SubjectModel.Scripts.InventorySystem;
using UnityEngine;

namespace SubjectModel.Scripts.AI
{
    /**
     * <summary>用于构建持枪敌人AI的工具类，其中的方法多为Bolt调用</summary>
     */
    public static class ShooterAi
    {
        /**
         * <summary>
         * 构建持枪敌人的物品栏
         * <param name="self">敌人的GameObject</param>
         * <example>
         * 使用如下代码添加枪械
         * <code>inventory.Add(new Firearm(FirearmDictionary.FirearmTemples[枪械模板序号]));</code>
         * 使用如下代码添加弹匣（RF不需要此步骤）
         * <code>inventory.Add(new Magazine(FirearmDictionary.MagazineTemples[弹匣模板序号]));</code>
         * 使用如下代码添加子弹
         * <code>inventory.Add(new Bullet(FirearmDictionary.BulletTemples[子弹模板序号], 数量));</code>
         * </example>
         * </summary>
         */
        public static void GenerateShooter(GameObject self)
        {
            if (!self.TryGetComponent<Inventory>(out var inventory)) return; //获取物品栏inventory
            var firearm = inventory.Add(new Firearm(FirearmDictionary.FirearmTemples[0])); //添加模板0枪支
            inventory.Add(new Bullet(FirearmDictionary.BulletTemples[0], 1000000)); //添加子弹
            inventory.Add(new Magazine(FirearmDictionary.MagazineTemples[0])); //添加弹匣
            inventory.SwitchTo(0); //装备位于物品栏0号位的枪支
            firearm.OnSlaveUse(self); //装备
        }

        /**
         * <summary>
         * 敌人换弹时调用的方法
         * <param name="inventory">敌人的物品栏</param>
         * <param name="bulletIndex">子弹在敌人物品栏中所处的位置</param>
         * </summary>
         */
        public static void Reload(Inventory inventory, int bulletIndex)
        {
            if (!(inventory.Contains[bulletIndex] is Bullet bullet)) return; //若给定位置不是子弹则返回
            ((Firearm) inventory.Contains[0]).Magazine.Load(inventory, bullet); //给位于0号位置的枪支装弹，注意存在失败的可能
        }
    }
}