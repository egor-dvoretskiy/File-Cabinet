using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FileCabinetApp.Interfaces;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable SA1401 // Fields should be private

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Abstract class for CommandHandler.
    /// </summary>
    public abstract class CommandHandlerBase : ICommandHandler
    {
        /// <summary>
        /// Holds the similar commands string.
        /// </summary>
        protected static StringBuilder similarCommandsBuilder = new StringBuilder(InitSimilarCommandsMessage);
        private const string InitSimilarCommandsMessage = "The most similar command(s):\n";
        private static int amountOfSimilarCommands = 0;

        private ICommandHandler nextHandler;
        private int minimalSimilarityCoefficient = 3;

        /// <inheritdoc/>
        public virtual void Handle(AppCommandRequest appCommandRequest)
        {
            if (this.nextHandler != null)
            {
                this.nextHandler.Handle(appCommandRequest);
            }
            else
            {
                Console.WriteLine("There is no such command. See 'help'.");

                this.PrintSimilarCommands();
                this.ResetSimilarCommandsHandler();
            }
        }

        /// <inheritdoc/>
        public void SetNext(ICommandHandler commandHandler)
        {
            this.nextHandler = commandHandler;
        }

        /// <summary>
        /// Resets the similar commands handler.
        /// </summary>
        protected void ResetSimilarCommandsHandler()
        {
            similarCommandsBuilder = new StringBuilder(InitSimilarCommandsMessage);
            amountOfSimilarCommands = 0;
        }

        /// <summary>
        /// Assigns similar commands to builder.
        /// </summary>
        /// <param name="commandName">Name of method.</param>
        /// <param name="appCommandRequest">Current command request.</param>
        protected void AssignToSimilarCommands(string commandName, AppCommandRequest appCommandRequest)
        {
            Dictionary<char, int> letterPresenceInCommandWord = this.AssignCommandWordToDictionary(commandName);

            this.minimalSimilarityCoefficient = (int)Math.Ceiling(commandName.Length * 0.7);

            string requestCommand = appCommandRequest.Command;

            int similarityCoefficient = 0;

            for (int i = 0; i < requestCommand.Length; i++)
            {
                char currentLetter = requestCommand[i];
                int currentLetterOccurencies = this.CountCharInWord(requestCommand, currentLetter);

                if (letterPresenceInCommandWord.ContainsKey(currentLetter))
                {
                    bool isEnoughOccurencies = currentLetterOccurencies >= letterPresenceInCommandWord[currentLetter];

                    if (isEnoughOccurencies)
                    {
                        similarityCoefficient++;
                    }
                }
            }

            if (similarityCoefficient >= this.minimalSimilarityCoefficient)
            {
                similarCommandsBuilder.AppendLine(string.Concat("\t", commandName));
                amountOfSimilarCommands++;
            }
            else
            {
                for (int i = 0; i < requestCommand.Length; i++)
                {
                    int currentLength = requestCommand.Length - i;
                    bool startsWithLetter = commandName.StartsWith(requestCommand[..currentLength]);
                    if (startsWithLetter)
                    {
                        similarCommandsBuilder.AppendLine(string.Concat("\t", commandName));
                        amountOfSimilarCommands++;
                        return;
                    }
                }
            }
        }

        private void PrintSimilarCommands()
        {
            Console.WriteLine();

            if (amountOfSimilarCommands == 0)
            {
                Console.WriteLine("There are no similar commands.");
            }
            else
            {
                Console.WriteLine(similarCommandsBuilder.ToString());
            }
        }

        private Dictionary<char, int> AssignCommandWordToDictionary(string command)
        {
            Dictionary<char, int> letterPresenceInCommandWord = new Dictionary<char, int>();

            for (int i = 0; i < command.Length; i++)
            {
                if (letterPresenceInCommandWord.ContainsKey(command[i]))
                {
                    letterPresenceInCommandWord[command[i]]++;
                }
                else
                {
                    letterPresenceInCommandWord.Add(command[i], 1);
                }
            }

            return letterPresenceInCommandWord;
        }

        private int CountCharInWord(string word, char letter)
        {
            int amount = word.Split(letter).Length - 1;
            return amount;
        }
    }
}
