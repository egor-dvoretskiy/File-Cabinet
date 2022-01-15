using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FileCabinetApp.ConditionWords;
using FileCabinetApp.Interfaces;
using FileCabinetApp.ServiceTools;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for delete command.
    /// </summary>
    public class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        private const string CommandName = "delete";
        private const string KeyWord = "where ";

        private const int KeyWordPosition = 0;
        private const int ParameterIndex = 0;
        private const int ValueIndex = 1;
        private const int AmountOfSplitedParameters = 2;

        private const char ValuesBrackets = '\'';
        private const char ParametersDivider = '=';

        private readonly IRecordInputValidator inputValidator;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommandHandler"/> class.
        /// </summary>
        /// <param name="service">File cabinet service.</param>
        /// <param name="inputValidator">Validator for input args.</param>
        public DeleteCommandHandler(IFileCabinetService service, IRecordInputValidator inputValidator)
            : base(service)
        {
            this.inputValidator = inputValidator;
        }

        /// <inheritdoc/>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            this.AssignToSimilarCommands(CommandName, appCommandRequest);

            string command = appCommandRequest.Command;
            string parameters = appCommandRequest.Parameters;

            if (command.Equals(CommandName, StringComparison.OrdinalIgnoreCase))
            {
                this.Delete(parameters);
            }
            else
            {
                base.Handle(appCommandRequest);
            }
        }

        private void Delete(string parameters)
        {
            try
            {
                string phrase = parameters[KeyWord.Length..];
                ConditionWhere where = new ConditionWhere(this.service, this.inputValidator);
                var records = where.GetFilteredRecords(phrase);

                List<int> listOfIdToDelete = this.GetListOfIds(records);

                this.service.Delete(listOfIdToDelete);
            }
            catch (ArgumentException argumentException)
            {
                Console.WriteLine(argumentException.Message);
            }
        }

        private List<int> GetListOfIds(List<FileCabinetRecord> records)
        {
            List<int> ids = new List<int>();

            for (int i = 0; i < records.Count; i++)
            {
                ids.Add(records[i].Id);
            }

            return ids;
        }
    }
}
