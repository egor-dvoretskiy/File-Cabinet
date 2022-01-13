using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.ConditionWords
{
    /// <summary>
    /// Consdition processor.
    /// </summary>
    internal class ConditionWhere
    {
        private readonly string initPhrase = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionWhere"/> class.
        /// </summary>
        /// <param name="phrase">Initial phrase.</param>
        public ConditionWhere(string phrase)
        {
            this.initPhrase = phrase;
        }
    }
}
