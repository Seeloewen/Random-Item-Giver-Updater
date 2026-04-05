using System.Collections.Generic;
using System.IO;

namespace RandomItemGiverUpdater.Core.Data
{
    public class Datapack
    {
        private List<LootTable> lootTables = new List<LootTable>();

        public string rootDirectory;
        public string lootTablesDirectory;
        public bool usesLegacyNBT = false;
        public bool usesOldFolderStructure = false;
        public int packFormat = 0;

        public Datapack(string path)
        {
            rootDirectory = path;
            Load();
            packFormat = GetPackFormat();
            usesLegacyNBT = packFormat < 41;
        }

        public static Datapack Get() => RIGU.core.currentDatapack;

        public bool IsValid() => Directory.Exists(rootDirectory); //TODO: could probably implement a better check if the datapack is actually valid

        public void Save() => lootTables.ForEach(t => t.Save());

        private void Load()
        {
            //Get the rootpath depending on the version
            if (Directory.Exists($"{rootDirectory}\\data\\randomitemgiver\\loot_tables")) //1.20 and before
            {
                lootTablesDirectory = $"{rootDirectory}\\data\\randomitemgiver\\loot_tables";
                usesOldFolderStructure = true;
            }
            else if (Directory.Exists($"{rootDirectory}\\data\\randomitemgiver\\loot_table")) //1.21 and above
            {
                lootTablesDirectory = $"{rootDirectory}\\data\\randomitemgiver\\loot_table";
            }

            lootTables.AddRange(ScanForLootTables(lootTablesDirectory));
        }

        private List<LootTable> ScanForLootTables(string path) //Recursively goes through all folders in this path
        {
            List<LootTable> lootTables = new List<LootTable>();

            //Go through each file in the path and check whether it's a loot table
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                if (file.Contains(".json")) //Assumes every json file in the scanned folder is a loot table (which should be the case)
                {
                    string name = file.Replace($"{path}\\", "").Replace(".json", "");
                    string identifier = file.Replace($"{lootTablesDirectory}\\", "").Replace(".json", "");
                    lootTables.Add(new LootTable(name, identifier, file));
                }
            }

            //Go through the remaining directories and check them too
            string[] directories = Directory.GetDirectories(path);
            foreach (string directory in directories)
            {
                lootTables.AddRange(ScanForLootTables(directory));
            }

            return lootTables;
        }

        public LootTable GetLootTable(string path)
        {
            //Get the corresponding loot table
            foreach (LootTable lootTable in lootTables)
            {
                if (lootTable.path == path) return lootTable;
            }

            return null;
        }

        public string GetVersionString(string path)
        {
            if (File.Exists($"{path}\\UPDATER.txt"))
            {
                //Use default method of detecting version by file
                string datapackVersion = "unknown";
                string mcVersion = "unknown";
                string versionBranch = "unknown";

                //Go through the file to get the variables
                string[] file = File.ReadAllLines(string.Format("{0}\\UPDATER.txt", path));
                for (int i = 0; i < file.Length; i++)
                {
                    //Only read the line if it's not a comment
                    if (!file[i].Contains("#"))
                    {
                        if (file[i].Contains("datapack_version"))
                        {
                            datapackVersion = file[i].Replace("datapack_version=", "");
                        }
                        else if (file[i].Contains("version_branch"))
                        {
                            versionBranch = file[i].Replace("version_branch=", "");
                        }
                        else if (file[i].Contains("mc_version"))
                        {
                            mcVersion = file[i].Replace("mc_version=", "");
                        }
                    }
                }

                return $"Version {datapackVersion} for {mcVersion} ({versionBranch})";
            }
            else
            {
                //Use the legacy version of getting the version if the file doesn't exist (Most likely because the datapack is too old)
                return GetVersionStringLegacy(path);
            }
        }
        public string GetVersionStringLegacy(string path) //Gets datapack version string from pack format
        {
            return packFormat switch
            {
                4 => "Version: 1.13 - 1.14.4 (unsupported) (Pack format: 4)",
                5 => "Version: 1.15 - 1.16.1 (unsupported) (Pack format: 5)",
                6 => "Version: 1.16.2 - 1.16.5 (Pack format: 6)",
                7 => "Version: 1.17 - 1.17.1 (Pack format: 7)",
                8 => "Version: 1.18 - 1.18.1 (Pack format: 8)",
                9 => "Version: 1.18.2 (Pack format: 9)",
                10 => "Version: 1.19 - 1.19.3 (Pack format: 10)",
                11 => "Version: 1.19.4-Snapshot (Pack format: 11)",
                12 => "Version: 1.19.4 (Pack format: 12)",
                13 => "Version: 1.20-Snapshot (Pack format: 13)",
                14 => "Version: 1.20-Snapshot (Pack format: 14)",
                15 => "Version: 1.20 (Pack format: 15)",
                _ => $"Version: Unknown (Pack format: {packFormat})"
            };
        }

        private int GetPackFormat()
        {
            //Read the pack format from the file and remove unnecessary characters
            string versionString = File.ReadAllLines($"{rootDirectory}/pack.mcmeta")[2];
            versionString = versionString.Replace("    \"pack_format\":", "").Replace(",", "");
            return int.Parse(versionString);
        }

        public int GetLootTableAmount() => lootTables.Count;

        public List<LootTable> GetLootTables() => lootTables;
    }
}
