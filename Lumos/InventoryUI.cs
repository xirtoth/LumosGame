using Lumos;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

public class InventoryUI
{
    private SpriteBatch spriteBatch;
    private Texture2D itemRectangleTexture;
    private SpriteFont itemFont;
    private Vector2 inventoryPosition;
    private int itemSize;
    private int padding;
    private Item draggedItem;
    private Rectangle draggedItemRect;
    private Vector2 draggedItemOffset;

    public InventoryUI(SpriteBatch spriteBatch, Texture2D itemRectangleTexture, SpriteFont itemFont, Vector2 inventoryPosition, int itemSize, int padding)
    {
        this.spriteBatch = spriteBatch;

        this.itemFont = itemFont;
        this.inventoryPosition = inventoryPosition;
        this.itemSize = itemSize;
        this.padding = padding;
        this.itemRectangleTexture = GenerateItemRectangleTexture(16, Color.Black);
    }

    public void UpdateInventory(Inventory inventory)
    {
        MouseState mouseState = Mouse.GetState();

        // Check if an item is being dragged
        if (draggedItem != null)
        {
            // Update the position of the dragged item
            Vector2 newPosition = mouseState.Position.ToVector2() - draggedItemOffset;
            draggedItemRect.Location = newPosition.ToPoint();

            // Check if the dragged item is released
            if (mouseState.LeftButton == ButtonState.Released)
            {
                // Find the slot where the dragged item is released

                List<Item> itemsCopy = new List<Item>(inventory.items);
                foreach (Item slotItem in itemsCopy)
                {
                    Rectangle slotRect = GetItemRectangle(inventoryPosition, itemSize, padding, slotItem, inventory);
                    if (slotRect.Contains(mouseState.Position))
                    {
                        // Swap the positions of the dragged item and the slot item
                        int draggedItemIndex = inventory.items.IndexOf(draggedItem);
                        int slotItemIndex = inventory.items.IndexOf(slotItem);

                        inventory.items[draggedItemIndex] = slotItem;
                        inventory.items[slotItemIndex] = draggedItem;
                        break;
                    }
                    else if (!GetInventoryRectangle(inventoryPosition, (int)inventory.Width, 160).Contains(mouseState.Position))
                    {
                        for (int i = 0; i < draggedItem.Count; i++)
                        {
                            Game1.Instance._items.Add(new Item(draggedItem.Name, draggedItem.Description, draggedItem.Texture, Game1.Instance._player.Pos + new Vector2(60 + i * 2, -10)));
                        }
                        draggedItem.Count = 0;
                        inventory.items.Remove(draggedItem);
                    }
                }

                // Reset the dragged item
                draggedItem = null;
            }
        }
        else
        {
            // Check if an item is clicked and start dragging
            foreach (Item item in inventory.items)
            {
                Rectangle itemRect = GetItemRectangle(inventoryPosition, itemSize, padding, item, inventory);
                if (itemRect.Contains(mouseState.Position) && mouseState.LeftButton == ButtonState.Pressed)
                {
                    draggedItem = item;
                    draggedItemRect = itemRect;
                    draggedItemOffset = mouseState.Position.ToVector2() - itemRect.Location.ToVector2();
                    break;
                }
            }
        }
    }

    private Rectangle GetInventoryRectangle(Vector2 position, int width, int height)
    {
        return new Rectangle((int)position.X, (int)position.Y, width, height);
    }

    private Rectangle GetItemRectangle(Vector2 position, int size, int padding, Item item, Inventory inventory)
    {
        int row = 0;
        int col = 0;
        int itemsPerRow = (int)inventory.Width / itemSize;

        // Find the row and column of the item
        for (int i = 0; i < inventory.items.Count; i++)
        {
            if (inventory.items[i] == item)
            {
                row = i / itemsPerRow;
                col = i % itemsPerRow;
                break;
            }
        }

        int x = (int)position.X + col * (size + padding);
        int y = (int)position.Y + row * (size + padding);

        return new Rectangle(x, y, size, size);
    }

    public void DrawInventory(Inventory inventory)
    {
        Vector2 itemPosition = inventoryPosition;
        MouseState mouseState = Mouse.GetState();

        foreach (Item item in inventory.items)
        {
            Rectangle itemRect = new Rectangle((int)itemPosition.X, (int)itemPosition.Y, itemSize, itemSize);
            bool isMouseOverItem = itemRect.Contains(mouseState.Position);

            spriteBatch.Draw(itemRectangleTexture, itemRect, Color.Black);
            spriteBatch.Draw(item.Texture, itemRect, Color.White);
            if (draggedItem != null)
            {
                spriteBatch.Draw(draggedItem.Texture, new Vector2(mouseState.X, mouseState.Y), Color.Wheat);
            }
            int itemcount = item.Count + 1;
            spriteBatch.DrawString(itemFont, itemcount.ToString(), itemPosition + new Vector2(padding), Color.White);

            if (isMouseOverItem)
            {
                Vector2 namePosition = itemPosition + new Vector2(padding, itemSize + padding);
                spriteBatch.DrawString(itemFont, item.Name, namePosition, Color.Red);
            }

            itemPosition.X += itemSize + padding;

            // Check if the next item will exceed the inventory width
            if (itemPosition.X + itemSize > inventoryPosition.X + inventory.Width)
            {
                itemPosition.X = inventoryPosition.X;
                itemPosition.Y += itemSize + padding;
            }
        }

        // Draw empty inventory spaces for any remaining slots
        int remainingSlots = inventory.Size - inventory.items.Count;
        for (int i = 0; i < remainingSlots; i++)
        {
            Rectangle emptyItemRect = new Rectangle((int)itemPosition.X, (int)itemPosition.Y, itemSize, itemSize);
            bool isMouseOverEmptyItem = emptyItemRect.Contains(mouseState.Position);

            spriteBatch.Draw(itemRectangleTexture, emptyItemRect, Color.Black);

            if (isMouseOverEmptyItem)
            {
                //  Vector2 namePosition = itemPosition + new Vector2(padding, itemSize + padding);
                //  spriteBatch.DrawString(itemFont, "Empty Slot", namePosition, Color.Yellow);
            }

            itemPosition.X += itemSize + padding;

            // Check if the next item will exceed the inventory width
            if (itemPosition.X + itemSize > inventoryPosition.X + inventory.Width)
            {
                itemPosition.X = inventoryPosition.X;
                itemPosition.Y += itemSize + padding;
            }
        }
    }

    private Texture2D GenerateItemRectangleTexture(int size, Color color)
    {
        Texture2D texture = new Texture2D(Game1.Instance.GraphicsDevice, itemSize, itemSize);

        Color[] data = new Color[size * size];
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = new Color(color.R, color.G, color.B, (byte)128); // Set the alpha value to 128 for semi-transparency
        }

        texture.SetData(data);

        return texture;
    }
}