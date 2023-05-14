using System.Xml.Serialization;

namespace Footballers.DataProcessor.ExportDto
{
    [XmlType("Coach")]
    public class ExportCoachDbo
    {
        [XmlElement("CoachName")]
        public string CoachName { get; set; } = null!;

        [XmlAttribute("FootballersCount")]
        public int FootballersCount { get; set; }

        [XmlArray("Footballers")]
        public ExportFootballerDbo[] Footballers { get; set; } = null!;
    }
}
