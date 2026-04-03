using RandomItemGiverUpdater.Core;

namespace RandomItemGiverUpdater
{
    public static class RIGU
    {
        public const string VERSION_NUM = "1.0.0 Pre-Release";
        public const string VERSION_DATE = "01.04.2026";

        public static Main core;
        public static ItemAdding itemAdding;
        public static DuplicateFinder duplicateFinder;
        public static ItemRemover itemRemover;
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
