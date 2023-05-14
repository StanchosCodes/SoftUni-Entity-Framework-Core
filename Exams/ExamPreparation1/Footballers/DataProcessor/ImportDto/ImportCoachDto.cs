using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Footballers.DataProcessor.ImportDto
{
    [XmlType("Coach")]
    public class ImportCoachDto
    {
        [XmlElement("Name")]
        [Required]
        [MaxLength(40)]
        [MinLength(2)]
        public string? Name { get; set; }

        [XmlElement("Nationality")]
        [Required]
        public string? Nationality { get; set; }

        [XmlArray("Footballers")]
        public ImportFootballerDto[] Footballers { get; set; }
    }
}
