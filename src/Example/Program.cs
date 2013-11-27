using System.Linq;

namespace LateNightStupidities.XorPersist.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            RootClass root = new RootClass(true);
            LeafClass leaf1 = new LeafClass(root, "leaf1");
            root.MainLeaf = leaf1;

            LeafClass leaf2 = new LeafClass(root, "leaf2");
            LeafClass leaf11 = new LeafClass(leaf1, "leaf11");
            leaf1.Leaf = leaf11;
            LeafClass leaf111 = new LeafClass(leaf11, "leaf111");
            leaf11.Leaf = leaf111;

            var controller = XorController.Get();
            controller.Save(root, @"C:\Users\Sebastian\Desktop\temp\xor\test.xml");
            
            root = controller.Load<RootClass>(@"C:\Users\Sebastian\Desktop\temp\xor\test.xml");

            var references = root.ReferencedXorObjects.ToList();
            var owned = root.OwnedXorObjects.ToList();

            controller = XorController.Get();
            controller.Save(root, @"C:\Users\Sebastian\Desktop\temp\xor\test2.xml");
        }
    }
}
