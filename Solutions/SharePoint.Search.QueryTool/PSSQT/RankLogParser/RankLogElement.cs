using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace PSSQT.RankLogParser
{
    public abstract class RankLogElement
    {
        protected RankLogElement()
        {
        }


        // Parse XmlNode
        internal abstract void Parse(XmlNode node);

        // Factory method for child elements

        protected virtual RankLogElement CreateChildElement(XmlNode node)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return GetType().Name;
        }

        // protected helper methods

    }


}