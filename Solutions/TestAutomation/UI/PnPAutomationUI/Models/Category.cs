using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PnPAutomationUI.Models
{
    public class Category
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
    }
    public class CategoryList
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}