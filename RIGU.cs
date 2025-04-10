namespace Random_Item_Giver_Updater
{
    public static class RIGU
    {
        public static wndMain wndMain;

        public static void Initialize(wndMain wndMain)
        {
            //Initializes the app and makes necessary content available across the code
            RIGU.wndMain = wndMain;
        }
    }

    public enum ModificationState
    {
        Unchanged,
        Deleted,
        Edited,
    }
}
