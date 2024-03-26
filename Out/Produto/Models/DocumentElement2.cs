using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Produto.Models
{
    [XmlRoot("DocumentElement")]
    public class DocumentElement2
    {
        [XmlElement("data")]
        public List<LocationModel> LocationModels { get; set; }
    }
}
