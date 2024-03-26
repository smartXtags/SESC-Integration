using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Produto
{

    public class ProdutoModel
    {
        public string ID { get; set; }
        public string IDCODE { get; set; }
        public string ACTIVE { get; set; }
        public string SERIAL { get; set; }
        public string QUANTITY { get; set; }
        public string CONDITION_ID { get; set; }
        public string DISPOSITION_ID { get; set; }
        public string LOCATION_ID { get; set; }
        public string USR1 { get; set; }
        public string USR2 { get; set; }
        public string USR3 { get; set; }
        public string USR4 { get; set; }
        public string USR5 { get; set; }
        public string USR6 { get; set; }
        public string USR7 { get; set; }
        public string USR8 { get; set; }
        public string USR9 { get; set; }
        public string DESCRIPTION { get; set; }
        public string PRODUCT_ID { get; set; }
        public string LAST_SEEN { get; set; }
        public string IMAGE { get; set; }
        public string DEPARTMENT_ID { get; set; }
        public string CUSTODIAN_ID { get; set; }
        public string DISPOSAL_ID { get; set; }
        public string GROUP_ID { get; set; }
        public string HOME_LOCATION_ID { get; set; }
        public string LATITUDE { get; set; }
        public string LONGITUDE { get; set; }
        public string CONTAINER_ID { get; set; }
        public string COSTCENTER_ID { get; set; }
        public string LAST_MODIFIED { get; set; }
        public string LAST_LOCATION { get; set; }
    }
}
