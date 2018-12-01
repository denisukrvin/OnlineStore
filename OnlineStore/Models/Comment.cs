using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineStore.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Author { get; set; }
        public string Text { get; set; }
        public string Time { get; set; }

        public int? ProductId { get; set; }
        public Product Product { get; set; }
    }
}