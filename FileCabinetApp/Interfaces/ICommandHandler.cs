using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp.CommandHandlers;

namespace FileCabinetApp.Interfaces
{
    /// <summary>
    /// Holds "Chain of Responsibility" methods.
    /// </summary>
    public interface ICommandHandler
    {
        /// <summary>
        /// Sets the next command.
        /// </summary>
        /// <param name="commandHandler">Command handler.</param>
        void SetNext(ICommandHandler commandHandler);

        /// <summary>
        /// Hold the current command.
        /// </summary>
        /// <param name="appCommandRequest">Command Request.</param>
        void Handle(AppCommandRequest appCommandRequest);
    }
}
