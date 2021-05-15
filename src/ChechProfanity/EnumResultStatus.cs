namespace CheckProfanity
{
    /// <summary> Processing result </summary>
    public enum EnumResultStatus
    {
        /// <summary> Haven't any information about execute method </summary>
        Undef,

        /// <summary> Text is ok </summary>
        TextIsOk,

        /// <summary> Text has Profanity </summary>
        TextHasProfanity,

        /// <summary> Something wrong </summary>
        ExecuteError
    }
}