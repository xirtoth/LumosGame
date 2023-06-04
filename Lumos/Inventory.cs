using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos
{
    public class Inventory
    {
        public int Size { get; set; }
        public int Count { get; set; }
        public float Width { get; internal set; }

        public InventoryUI UI { get; set; }

        public List<Item> items;

        public Inventory()
        {
            UI = new InventoryUI(Game1.Instance._spriteBatch, TileTextures.Apple, TileTextures.MyFont, new Microsoft.Xna.Framework.Vector2(10, 100), 16, 0);
            items = new List<Item>();
            Width = 500;
            Size = 16 * 20 - 10;
        }

        public void AddItem(Item item)
        {
            // Check if the item already exists in the inventory
            Item existingItem = items.FirstOrDefault(i => i.Name == item.Name);
            if (existingItem != null)
            {
                // Item already exists, increment its count
                existingItem.Count++;
            }
            else
            {
                // Item does not exist, add it to the inventory
                items.Add(item);
            }
        }
    }
}