using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineStore.Models
{
    public class Order
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Delivery { get; set; }
        public string Payment { get; set; }
        public string PIB { get; set; }
        public string DeliveryCity { get; set; }
        public string DeliveryWarehouse { get; set; }
    }
}