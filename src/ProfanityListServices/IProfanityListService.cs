﻿using System.Collections.Generic;

namespace ProfanityList
{
    /// <summary>
    /// Service for administrating a profanity word list
    /// </summary>
    public interface IProfanityListService
    {
        /// <summary> Return all registred words </summary>
        /// <returns></returns>
        IReadOnlyCollection<string> GetProfanityWordList();

    }
}