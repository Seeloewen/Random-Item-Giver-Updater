using RandomItemGiverUpdater.Core;

namespace RandomItemGiverUpdater
{
    public static class RIGU
    {
        public const string VERSION_NUM = "Public Beta 3";
        public const string VERSION_DATE = "27.08.2024";

        public static Main core;
        public static ItemAdding itemAdding;

        public static void Initialize(Main core)
        {
            //Initializes the app and makes necessary content available across the code
            RIGU.core = core;
            itemAdding = new ItemAdding();
        }
    }

    public enum ModificationState
    {
        Unchanged,
        Deleted,
        Edited,
    }
}
