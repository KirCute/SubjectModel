using SubjectModel.Scripts.InventorySystem;
using SubjectModel.Scripts.Subject.Firearms;
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
         * </summary>
         * <param name="self">敌人的GameObject</param>
         * <example>
         * 使用如下代码添加枪械
         * <code>inventory.Add(new Firearm(FirearmDictionary.FirearmTemples[枪械模板序号]));</code>
         * 使用如下代码添加弹匣（RF不需要此步骤）
         * <code>inventory.Add(new Magazine(FirearmDictionary.MagazineTemples[弹匣模板序号]));</code>
         * 使用如下代码添加子弹
         * <code>inventory.Add(new Bullet(FirearmDictionary.BulletTemples[子弹模板序号], 数量));</code>
         * </example>
         */
        public static void GenerateShooter(GameObject self)
        {
            if (!self.TryGetComponent<Inventory>(out var inventory)) return; //获取物品栏inventory
            inventory.Add(new Firearm(FirearmDictionary.FirearmTemples[2])); //添加模板0枪支
            inventory.Add(new Bullet(FirearmDictionary.BulletTemples[4], 1000000)); //添加子弹
            inventory.Selecting = 0; //装备位于物品栏0号位的枪支
        }
    }
}