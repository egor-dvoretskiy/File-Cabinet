using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.ServiceTools
{
    /// <summary>
    /// Provides the communication with buffer (read, write).
    /// </summary>
    internal static class ServiceBufferCommunicator
    {
        /// <summary>
        /// Reads fixed size buffer.
        /// </summary>
        /// <param name="bytesRecord">Acquired from file buffer.</param>
        /// <param name="maxNameLength">Maximum length of name in binary file.</param>
        /// <returns>Returns read record and isDeleted flag.</returns>
        public static Tuple<FileCabinetRecord, short> ReadRecordFromBuffer(byte[] bytesRecord, byte maxNameLength)
        {
            short isRecordDeleted = 0;
            FileCabinetRecord record = new FileCabinetRecord();

            using (MemoryStream memoryStream = new MemoryStream(bytesRecord))
            using (BinaryReader reader = new BinaryReader(memoryStream))
            {
                isRecordDeleted = reader.ReadInt16();

                record.Id = reader.ReadInt32();

                byte[] firstNameBytes = reader.ReadBytes(maxNameLength);
                string firstName = Encoding.ASCII.GetString(firstNameBytes, 0, maxNameLength).TrimEnd('\0');
                record.FirstName = firstName;

                byte[] lastNameBytes = reader.ReadBytes(maxNameLength);
                string lastName = Encoding.ASCII.GetString(lastNameBytes, 0, maxNameLength).TrimEnd('\0');
                record.LastName = lastName;

                int year = reader.ReadInt32();
                int month = reader.ReadInt32();
                int day = reader.ReadInt32();
                record.DateOfBirth = new DateTime(year, month, day);

                record.PersonalRating = reader.ReadInt16();
                record.Salary = reader.ReadDecimal();
                record.Gender = reader.ReadChar();
            }

            return new Tuple<FileCabinetRecord, short>(record, isRecordDeleted);
        }

        /// <summary>
        /// Writes record in temporary buffer.
        /// </summary>
        /// <param name="record">Record to write in buffer.</param>
        /// <param name="recordSize">Max record size in binary representation.</param>
        /// <param name="maxNameLength">Maximum length of name in binary file.</param>
        /// <returns>Returns buffer.</returns>
        internal static byte[] WriteRecordToBuffer(FileCabinetRecord record, int recordSize, int maxNameLength)
        {
            byte[] buffer = new byte[recordSize];
            using (MemoryStream memoryStream = new MemoryStream(buffer))
            using (BinaryWriter writer = new BinaryWriter(memoryStream))
            {
                short isRecordDeleted = 0;
                writer.Write(isRecordDeleted);

                writer.Write(record.Id);

                var firstNameBytes = ServiceBufferCommunicator.PrepareStringToWrite(record.FirstName, maxNameLength);
                writer.Write(firstNameBytes);
                memoryStream.Seek(maxNameLength - firstNameBytes.Length, SeekOrigin.Current);

                var lastNameBytes = ServiceBufferCommunicator.PrepareStringToWrite(record.LastName, maxNameLength);
                writer.Write(lastNameBytes);
                memoryStream.Seek(maxNameLength - lastNameBytes.Length, SeekOrigin.Current);

                int year = record.DateOfBirth.Year;
                writer.Write(year);

                int month = record.DateOfBirth.Month;
                writer.Write(month);

                int day = record.DateOfBirth.Day;
                writer.Write(day);

                writer.Write(record.PersonalRating);
                writer.Write(record.Salary);
                writer.Write(record.Gender);
            }

            return buffer;
        }

        private static byte[] PrepareStringToWrite(string stringToWrite, int maxNameLength)
        {
            var nameBytes = Encoding.ASCII.GetBytes(stringToWrite);
            var nameBuffer = new byte[maxNameLength];
            int nameLength = nameBytes.Length;
            if (nameLength > maxNameLength)
            {
                nameLength = maxNameLength;
            }

            Array.Copy(nameBytes, 0, nameBuffer, 0, nameLength);

            return nameBytes;
        }
    }
}
