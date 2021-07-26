using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SubjectModel
{
    public interface ItemStack
    {
        public string GetName();
        public void OnMouseClickLeft(GameObject user);
        public void OnMouseClickRight(GameObject user);
        public void Selecting(GameObject user);
        public void OnSelected(GameObject user);
        public void LoseSelected(GameObject user);
        public int GetCount();
        public ItemStack Fetch();
        public Func<ItemStack, bool> SubInventory();
    }

    public abstract class Material : ItemStack
    {
        public abstract string GetName();
        public abstract int GetCount();
        public abstract ItemStack Fetch();

        public void OnMouseClickLeft(GameObject user)
        {
        }

        public void OnMouseClickRight(GameObject user)
        {
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

        public Func<ItemStack, bool> SubInventory()
        {
            return item => false;
        }
    }

    public class Inventory : MonoBehaviour
    {
        public IList<ItemStack> bag;
        public IList<ItemStack> sub;
        public int selecting;
        public int subSelecting;

        private void Start()
        {
            bag = new List<ItemStack>();
            sub = new List<ItemStack>();
            selecting = 0;
            subSelecting = 0;

            // Debug
            var debugMagazine = new MagazineTemple("测试用弹匣", 20);
            bag.Add(new Firearm(new FirearmTemple("模板0", 100f, 0.75f, 0.2f, 1f, 20f, 0.025f, 0.5f, 1f, 20f, 0.5f,
                debugMagazine)));
            bag.Add(new Magazine(debugMagazine) {BulletRemain = 20});
            bag.Add(new Magazine(debugMagazine) {BulletRemain = 20});
            bag.Add(new Magazine(debugMagazine) {BulletRemain = 20});
            bag.Add(new DrugStack
            {
                Tag = "FeCl3",
                Ions = new List<IonStack>
                {
                    new IonStack
                    {
                        Element = Elements.Fe, Index = Elements.Fe.GetIndex(3),
                        Amount = 1f, Concentration = 1f
                    },
                    new IonStack
                    {
                        Element = Elements.Cl, Index = Elements.Cl.GetIndex(-1),
                        Amount = 3f, Concentration = 3f
                    }
                },
                Properties = Element.Acid,
                Count = 10
            });
            bag.Add(new DrugStack
            {
                Tag = "CuSO4",
                Ions = new List<IonStack>
                {
                    new IonStack
                    {
                        Element = Elements.Cu, Index = Elements.Cu.GetIndex(2),
                        Amount = 1f, Concentration = 1f
                    }
                },
                Properties = Element.Acid,
                Count = 10
            });
            bag.Add(new DrugStack
            {
                Tag = "CoSO4",
                Ions = new List<IonStack>
                {
                    new IonStack
                    {
                        Element = Elements.Co, Index = Elements.Co.GetIndex(2),
                        Amount = 1f, Concentration = 1f
                    }
                },
                Properties = Element.Acid,
                Count = 10
            });
            bag.Add(new DrugStack
            {
                Tag = "HCl",
                Ions = new List<IonStack>
                {
                    new IonStack
                    {
                        Element = Elements.H, Index = Elements.H.GetIndex(1),
                        Amount = 1f, Concentration = 1f
                    },
                    new IonStack
                    {
                        Element = Elements.Cl, Index = Elements.Cl.GetIndex(-1),
                        Amount = 1f, Concentration = 1f
                    }
                },
                Properties = Element.Acid,
                Count = 10
            });
            bag.Add(new DrugStack
            {
                Tag = "FeSO4",
                Ions = new List<IonStack>
                {
                    new IonStack
                    {
                        Element = Elements.Fe, Index = Elements.Fe.GetIndex(2),
                        Amount = 1f, Concentration = 1f
                    }
                },
                Properties = Element.Acid,
                Count = 10
            });
            bag.Add(new DrugStack
            {
                Tag = "H2O2",
                Ions = new List<IonStack>
                {
                    new IonStack
                    {
                        Element = Elements.O, Index = Elements.O.GetIndex(-1),
                        Amount = 1f, Concentration = 1f
                    }
                },
                Properties = Element.Acid,
                Count = 10
            });
            bag.Add(new DrugStack
            {
                Tag = "KMnO4",
                Ions = new List<IonStack>
                {
                    new IonStack
                    {
                        Element = Elements.K, Index = Elements.K.GetIndex(1),
                        Amount = 1f, Concentration = 1f
                    },
                    new IonStack
                    {
                        Element = Elements.Mn, Index = Elements.Mn.GetIndex(7),
                        Amount = 1f, Concentration = 1f
                    }
                },
                Properties = Element.Acid,
                Count = 10
            });
            
            if (bag.Count == 0) return;
            bag[selecting].OnSelected(gameObject);
            foreach (var item in bag.Where(bag[selecting].SubInventory())) sub.Add(item);
        }

        private void Update()
        {
            for (var i = 0; i < bag.Count; i++)
                if (bag[i].GetCount() <= 0)
                {
                    Remove(bag[i]);
                    i--;
                }

            bag[selecting].Selecting(gameObject);
            if (Input.GetMouseButtonDown(0)) bag[selecting].OnMouseClickLeft(gameObject);
            if (Input.GetMouseButtonDown(1)) bag[selecting].OnMouseClickRight(gameObject);
            var alpha = GetAlphaDown();
            if (alpha != -1 && sub.Count > alpha) subSelecting = alpha;
            var mouseAxis = (int) (Input.GetAxisRaw("Mouse ScrollWheel") * 10);
            if (mouseAxis == 0 || selecting + mouseAxis < 0 || selecting + mouseAxis >= bag.Count) return;
            bag[selecting].LoseSelected(gameObject);
            sub.Clear();
            selecting += mouseAxis;
            bag[selecting].OnSelected(gameObject);
            foreach (var item in bag.Where(bag[selecting].SubInventory())) sub.Add(item);
        }

        public bool Remove(ItemStack item)
        {
            if (item == null) return true;
            sub.Remove(item);
            if (!bag.Contains(item)) return false;
            var index = bag.IndexOf(item);
            if (!bag.Remove(item)) return false;
            if (index < selecting) selecting--;
            if (index != selecting) return true;
            item.LoseSelected(gameObject);
            if (selecting == bag.Count) selecting--;
            if (selecting != -1) bag[selecting].OnSelected(gameObject);
            return true;
        }

        public void Add(ItemStack item)
        {
            if (item == null) return;
            bag.Add(item);
            if (bag[selecting].SubInventory()(item)) sub.Add(item);
        }

        public bool TryGetSubItem(out ItemStack item)
        {
            item = null;
            if (subSelecting >= sub.Count) return false;
            item = sub[subSelecting];
            return true;
        }

        private static int GetAlphaDown()
        {
            if (Input.GetKey(KeyCode.Alpha1)) return 0;
            if (Input.GetKey(KeyCode.Alpha2)) return 1;
            if (Input.GetKey(KeyCode.Alpha3)) return 2;
            if (Input.GetKey(KeyCode.Alpha4)) return 3;
            if (Input.GetKey(KeyCode.Alpha5)) return 4;
            if (Input.GetKey(KeyCode.Alpha6)) return 5;
            if (Input.GetKey(KeyCode.Alpha7)) return 6;
            if (Input.GetKey(KeyCode.Alpha8)) return 7;
            if (Input.GetKey(KeyCode.Alpha9)) return 8;
            if (Input.GetKey(KeyCode.Alpha0)) return 9;
            return -1;
        }
    }
}