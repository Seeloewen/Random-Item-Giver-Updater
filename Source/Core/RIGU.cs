namespace RandomItemGiverUpdater
{
    public static class RIGU
    {
        public static wndMain wndMain;
        public static ItemAddingCore itemAddingCore;

        public static void Initialize(wndMain wndMain)
        {
            //Initializes the app and makes necessary content available across the code
            RIGU.wndMain = wndMain;
            itemAddingCore = new ItemAddingCore();
        }
    }

    public enum ModificationState
    {
        Unchanged,
        Deleted,
        Edited,
    }
}
