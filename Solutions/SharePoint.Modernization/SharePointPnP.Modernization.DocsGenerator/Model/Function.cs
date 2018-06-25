using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointPnP.Modernization.DocsGenerator.Model
{
    public class Function
    {

        public Function()
        {
            this.Inputs = new List<FunctionParameter>();
            this.Outputs = new List<FunctionParameter>();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Example { get; set; }
        public List<FunctionParameter> Inputs { get; set; }
        public List<FunctionParameter> Outputs { get; set; }
    }
}
