using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileCabinetApp;

namespace FileCabinetGenerator
{
    public static class RecordGenerator
    {        
        private static List<string> lastNames = new List<string>();
        private static List<string> firstNames = new List<string>();

        private static int currentId = 0; 

        private static Tuple<DateTime, DateTime> birthDateLimits = new Tuple<DateTime, DateTime>(new DateTime(1950,1,1), DateTime.Now);

        private static Random random = new Random();

        private static char[] genderLetters = {
                'M',
                'F',
                'T',
                'N',
                'O',
            };

        public static void InitArrays(int startId)
        {
            InitFirstNameArray();
            InitLastNameArray();

            currentId = startId;
        }

        public static FileCabinetRecord GetRecord()
        {
            FileCabinetRecord record = new FileCabinetRecord();

            record.Id = currentId++;
            record.FirstName = GetFirstName();
            record.LastName = GetLastName();
            record.DateOfBirth = GetBirthDate();
            record.PersonalRating = GetPersonalRating();
            record.Salary = GetSalary();
            record.Gender = GetGender();

            return record;
        }

        private static void InitLastNameArray()
        {
            using (FileStream fileStream = new FileStream(@"..\\..\\..\\..\\..\\_file-cabinet-records\\lastNames.txt", FileMode.Open, FileAccess.Read))
            using (StreamReader streamReader = new StreamReader(fileStream))
            {
                var fileContainer = streamReader.ReadToEnd();
                lastNames = fileContainer.Split("\r\n").ToList();
            }
        }

        private static void InitFirstNameArray()
        {
            using (FileStream fileStream = new FileStream(@"..\\..\\..\\..\\..\\_file-cabinet-records\\firstNames.txt", FileMode.Open, FileAccess.Read))
            using (StreamReader streamReader = new StreamReader(fileStream))
            {
                var fileContainer = streamReader.ReadToEnd();
                firstNames = fileContainer.Split("\r\n").ToList();
            }
        }

        private static string GetFirstName()
        {
            int randomIndex = random.Next(firstNames.Count);
            return firstNames[randomIndex];
        }

        private static string GetLastName()
        {
            int randomIndex = random.Next(lastNames.Count);
            return lastNames[randomIndex];
        }

        private static DateTime GetBirthDate()
        {
            int dateoffset = (birthDateLimits.Item2 - birthDateLimits.Item1).Days;
            return birthDateLimits.Item1.AddDays(random.Next(dateoffset));
        }

        private static short GetPersonalRating()
        {
            return (short)random.Next(short.MinValue, short.MaxValue);
        }

        private static decimal GetSalary()
        {
            decimal maxSalary = 1000000; //european tugrics
            return Decimal.Round((decimal)random.NextDouble() * maxSalary, 2);
        }

        private static char GetGender()
        {
            return genderLetters[random.Next(genderLetters.Length)];
        }
    }
}
