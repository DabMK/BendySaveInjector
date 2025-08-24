using System;

namespace BendySaveInjector
{
    internal class DB
    {
        readonly public static Dictionary<string, string> values = new()
        {
            { "Difficulty", "m_Difficulty-m_Difficulty" },
            { "Difficulty Changed", "m_Difficulty-m_DifficultyChanged" },
            { "Butcher Gang Status", "m_Difficulty-m_ButcherGangStatus" },
            { "Socialite Not Invoked", "m_Difficulty-m_IsNotSocialite" },
            { "Health", "m_PlayerData-m_Statistics-m_Health" },
            { "Life", "m_PlayerData-m_Statistics-m_Life" },
            { "ILife", "m_PlayerData-m_Statistics-m_ILife" },
            { "Cooldown", "m_PlayerData-m_Statistics-m_Cooldown" },
            { "Food", "m_PlayerData-m_Statistics-m_Food" },
            { "Slugs", "m_PlayerData-m_Statistics-m_Slugs" },
            { "Slugs Spent", "m_PlayerData-m_Statistics-m_Spent" },
            { "Gent Cards", "m_PlayerData-m_Statistics-m_Cards" },
            { "Gent Toolkits", "m_PlayerData-m_Statistics-m_Toolkits" },
            { "Gent Parts", "m_PlayerData-m_Statistics-m_Parts" },
            { "Gent Battery Casings", "m_PlayerData-m_Statistics-m_BatteryCasings" },
            { "Gent Batteries", "m_PlayerData-m_Statistics-m_Batteries" },
            { "Deposited Batteries", "m_PlayerData-m_Statistics-m_DepositedBatteries" },
            { "Crouched State", "m_PlayerData-m_Statistics-m_IsCrouched" },
            { "Health Level", "m_PlayerData-m_Statistics-m_UpgradeLevelHealth" },
            { "Stamina Level", "m_PlayerData-m_Statistics-m_UpgradeLevelStamina" },
            { "Ability Level", "m_PlayerData-m_Statistics-m_UpgradeLevelAbility" },
            { "Kills", "m_PlayerData-m_Statistics-m_Kills" },
            { "Shock Kills", "m_PlayerData-m_Statistics-m_ShockKills" },
            { "Banish Kills", "m_PlayerData-m_Statistics-m_BanishKill" },
            { "Combat Status", "m_PlayerData-m_Statistics-m_CombatStatus" },
            { "Weapon ID", "m_WeaponData-m_WeaponID" },
            { "Weapon Status", "m_PlayerData-m_Statistics-m_WeaponStatus" },
            { "Power", "m_PlayerData-m_Statistics-m_Power" }
        };

        readonly public static Dictionary<string, string> customValues = new()
        {
            { "Illusion of Living Books", "m_DataDirectories-m_CollectableDirectory-m_IllusionDirectory" },
            { "Audio Logs", "m_DataDirectories-m_CollectableDirectory-m_AudioLogDirectory" },
            { "Memories", "m_DataDirectories-m_CollectableDirectory-m_MemoryDirectory" },
            { "Memos", "m_DataDirectories-m_CollectableDirectory-m_MemoDirectory" },
            // TODO: Meatleys (my save is glitched and I can't get through some walls I should be able to, so I can't get the meatleys)
        };

        // Custom Values IDs
        readonly public static List<List<int>> customValuesIDs =
        [
            [10501, 10601, 10701, 10801, 10901, 11001, 11101, 11201, 11301, 11401, 11501, 11601, 11701, 11801, 11901, 12001, 12101, 12201, 12301, 12401, 12501, 12601, 12701, 12801],
            [10501, 10601, 10701, 10702, 10703, 10801, 10802, 10803, 11001, 11002, 11101, 11102, 11103, 11201, 11301, 11302, 11701, 11702, 11901, 11902, 12001, 12101, 12201, 12202, 12203, 12401, 12601],
            [105101, 107101, 110101, 113101, 115101, 118101, 120101, 122101, 124101],
            [10501, 10502, 10503, 10504, 10505, 10601, 10602, 10603, 10604, 10801, 10802, 10803, 10804, 10805, 10901, 11001, 11002, 11101, 11201, 11301, 11302, 11401, 11402, 11701, 11702, 11703, 11704, 11901, 12201, 12202, 12301, 12501, 12601, 12602],

        ];

        // Custom Values Names
        readonly public static List<string> customValuesNames =
        [
            "m_DataID", "m_AudioLogID", "", ""
        ];
    }
}