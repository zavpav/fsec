namespace ProfanityList.Check
{
    /// <summary> Required execute detail </summary>
    public enum EnumExecuteDetail
    {
        /// <summary> Default detalization </summary>
        Default,

        /// <summary> Find first Profanity including. Result only fact of existing Profanity </summary>
        AnyResult,

        /// <summary> Count of blockers </summary>
        CountResult,

        /// <summary> Detailed information </summary>
        Detailed,

        /// <summary> Any founded information </summary>
        Verbosity
    }
}