using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF2Clone.Base.Helpers
{
    public static class NamingHelper
    {
        /// <summary>
        /// Gets next valid name if given is already used.
        /// Adds a number at the end if taken.
        /// </summary>
        /// <param name="presentNames"> Names already taken. </param>
        /// <param name="validatedName"> Name to be validated. </param>
        /// <returns></returns>
        public static string GetNextValidName(IEnumerable<string> presentNames, string validatedName)
        {
            int nextNameNumber = 1;
            var namesCount = presentNames.Count();
            if (namesCount > 0)
            {
                var currentVal = validatedName;
                if (presentNames.Any(x => x == validatedName))
                {
                    currentVal = string.Format("{0}{1}", currentVal, nextNameNumber);
                }

                while (presentNames.Any(x => x == string.Format("{0}{1}", currentVal, nextNameNumber)))
                {
                    nextNameNumber++;
                }
                return string.Format("{0}{1}", currentVal, nextNameNumber);
            }

            return validatedName;
        }

        /// <summary>
        /// Gets the next available id > 0.
        /// </summary>
        /// <param name="ids"> Ids that are unavaliable. </param>
        /// <returns></returns>
        public static int GetNextAvailableId(IEnumerable<int> ids)
        {
            return ids.OrderBy(x => x).FirstOrDefault(x => !ids.Contains(x + 1)) + 1;
        }
    }
}
