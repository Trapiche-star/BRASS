namespace Team1
{
    [System.Serializable]
    public class InventorySlot
    {
        public IItem Item;
        public int Count;

        public InventorySlot(IItem item)
        {
            Item = item;
            Count = 1;
        }

        public void AddOne() => Count++;
        public void RemoveOne() => Count--;
    }
}
