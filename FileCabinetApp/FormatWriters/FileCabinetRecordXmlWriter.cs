using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace FileCabinetApp
{
    /// <summary>
    /// Contains logics to work with xml.
    /// </summary>
    public class FileCabinetRecordXmlWriter
    {
        private readonly TextWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlWriter"/> class.
        /// </summary>
        /// <param name="writer">Writer.</param>
        public FileCabinetRecordXmlWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Writes to xml file.
        /// </summary>
        /// <param name="records">Records to write.</param>
        public void Write(FileCabinetRecord[] records)
        {
            /*XmlDocument xmlDocument = new XmlDocument();
            XmlNode root = xmlDocument.CreateElement(nameof(records));
            xmlDocument.AppendChild(root);

            for (int i = 0; i < records.Length; i++)
            {
                //-------------------------------------------------------- record node
                // add record node
                XmlNode recordNode = xmlDocument.CreateElement("record");

                // add attribute to record node
                XmlAttribute attributeRecord = xmlDocument.CreateAttribute("id");
                attributeRecord.Value = records[i].Id.ToString();
                recordNode.Attributes.Append(attributeRecord);

                root.AppendChild(recordNode);

                //------------------------------------------------------- ^^^

                //------------------------------------------------------- name node
                // add child to record
                XmlNode nameNode = xmlDocument.CreateElement("name");

                // add attribute to firstname
                XmlAttribute attributeNameFirst = xmlDocument.CreateAttribute("first");
                attributeNameFirst.Value = records[i].FirstName;
                nameNode.Attributes.Append(attributeNameFirst);

                // add attribute to lastname
                XmlAttribute attributeNameLast = xmlDocument.CreateAttribute("last");
                attributeNameLast.Value = records[i].LastName;
                nameNode.Attributes.Append(attributeNameLast);

                // add to doc name node
                recordNode.AppendChild(nameNode);

                //------------------------------------------------------ ^^^

                //------------------------------------------------------ birth date node
                // add dateBirth node
                XmlNode dateNode = xmlDocument.CreateElement("dateOfBirth");
                dateNode.InnerText = records[i].DateOfBirth.ToString("yyyy-MMM-dd");

                // add to doc dateNode
                recordNode.AppendChild(dateNode);

                //------------------------------------------------------ ^^^

                //------------------------------------------------------ personal rating node
                // add personal rating node
                XmlNode personalRatingNode = xmlDocument.CreateElement("personalRating");
                personalRatingNode.InnerText = records[i].PersonalRating.ToString();

                // add to doc personalRatingNode
                recordNode.AppendChild(personalRatingNode);

                //------------------------------------------------------ ^^^

                //------------------------------------------------------ debt node
                // add debt node
                XmlNode debtNode = xmlDocument.CreateElement("debt");
                debtNode.InnerText = records[i].Debt.ToString();

                // add to doc debtNode
                recordNode.AppendChild(debtNode);

                //------------------------------------------------------ ^^^

                //------------------------------------------------------ gender node
                // add gender node
                XmlNode genderNode = xmlDocument.CreateElement("gender");
                genderNode.InnerText = records[i].Gender.ToString();

                // add to doc debtNode
                recordNode.AppendChild(genderNode);

                //------------------------------------------------------ ^^^
            }

            xmlDocument.Save(this.writer);*/

            XmlSerializer serializer = new XmlSerializer(typeof(FileCabinetRecord[]), "records");
            serializer.Serialize(this.writer, records);
        }
    }
}
