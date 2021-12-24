using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Holds current command request.
    /// </summary>
    public class AppCommandRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppCommandRequest"/> class.
        /// </summary>
        /// <param name="command">Input command.</param>
        /// <param name="parameters">Input parameters.</param>
        public AppCommandRequest(string command, string parameters)
        {
            this.Command = command;
            this.Parameters = parameters;
        }

        /// <summary>
        /// Gets user input command.
        /// </summary>
        /// <value>
        /// User input command.
        /// </value>
        public string Command { get; private set; } = string.Empty;

        /// <summary>
        /// Gets user input parameters.
        /// </summary>
        /// <value>
        /// User input parameters.
        /// </value>
        public string Parameters { get; private set; } = string.Empty;
    }
}
