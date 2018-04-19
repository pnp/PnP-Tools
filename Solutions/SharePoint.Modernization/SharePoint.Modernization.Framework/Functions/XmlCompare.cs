// Eli Algranti Copyright ©  2013
// Code taken from http://xmlspecificationcompare.codeplex.com/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SharePoint.Modernization.Framework.Functions
{
    /// <summary>
    /// Loosely compares XML documents for equality:
    /// <list type="bullet">
    /// <item>Order of siblings in an element is ignored.</item>
    /// <item>Text nodes are the only node in at the bottom of the tree so sibling text nodes are merged for comparison.</item>
    /// <item>The prefix used for a namespace is ignored.</item>
    /// <item>Comments are ignored.</item>
    /// </list>
    /// This type of comparison is useful when comparing the XML documents used as messages, configuration, etc. in various specifications.
    /// </summary>
    public class XmlComparer
    {
        public static XmlEqualityResult AreEqual(string xmlA, string xmlB)
        {
            return AreEqual(ParseXml(xmlA).Root, ParseXml(xmlB).Root);
        }


        public static XmlEqualityResult AreEqual(XElement xmlA, XElement xmlB)
        {
            if (xmlA == null)
                throw new ArgumentNullException("xmlA", "The input Xml cannot be null");

            if (xmlB == null)
                throw new ArgumentNullException("xmlB", "The input Xml cannot be null");

            if (!xmlA.Name.Equals(xmlB.Name))
            {
                return new XmlEqualityResult
                {
                    ErrorMessage = "Root elements do not match.",
                    FailObject = xmlA
                };
            }

            var result = AreObjectsEqual(xmlA, xmlB);
            return new XmlEqualityResult
            {
                ErrorMessage = result.ErrorMessage,
                FailObject = result.FailObject
            };

        }

        private static InternalResult AreObjectsEqual(XElement xmlA, XElement xmlB)
        {
            var result = AreAttributesEqual(xmlA, xmlB);
            if (result.FailObject != null)
                return result;

            result = AreChildrenEqual(xmlA, xmlB);
            if (result.FailObject != null)
                return result;

            return AreLeafEqual(xmlA, xmlB);
        }

        private static InternalResult AreAttributesEqual(XElement xmlA, XElement xmlB)
        {
            var attributesB = xmlB.Attributes().Where(a => !a.IsNamespaceDeclaration).ToDictionary(a => a.Name);

            var attributesA = xmlA.Attributes().Where(a => !a.IsNamespaceDeclaration).ToList();

            if (attributesA.Count != attributesB.Count)
            {
                return new InternalResult
                {
                    FailObject = xmlA,
                    ErrorMessage = "Element has different number of attributes"
                };
            }

            foreach (var attributeA in attributesA)
            {
                XAttribute attributeB;
                if (attributesB.TryGetValue(attributeA.Name, out attributeB) && attributeA.Value == attributeB.Value)
                    continue;

                return new InternalResult
                {
                    FailObject = attributeA,
                    ErrorMessage = "No matching attribute found."
                };
            }

            return new InternalResult();
        }

        private static InternalResult AreLeafEqual(XElement xmlA, XElement xmlB)
        {
            var valueA = GetElementValue(xmlA);
            if (valueA == GetElementValue(xmlB))
                return new InternalResult();

            return new InternalResult
            {
                FailObject = string.IsNullOrEmpty(valueA)
                                ? xmlA
                                : xmlA.Nodes().First(n => n is XText)
            };
        }

        private static string GetElementValue(XElement element)
        {
            return string.Join("",
                               element.Nodes()
                                   .Where(n => n is XText)
                                   .Cast<XText>()
                                   .Select(t => t is XCData ? t.Value : t.Value.Trim()));
        }

        private static InternalResult AreChildrenEqual(XElement xmlA, XElement xmlB)
        {
            var elementsBLookup = xmlB.Elements()
                                      .ToLookup(n => n.Name).ToDictionary(g => g.Key, g => g.ToList());

            foreach (var childA in xmlA.Elements())
            {
                List<XElement> possibleMatches;
                if (!elementsBLookup.TryGetValue(childA.Name, out possibleMatches))
                {
                    return new InternalResult
                    {
                        FailObject = childA
                    };
                }

                InternalResult result;
                var matchIndex = FindChildMatch(childA, possibleMatches, out result);

                if (matchIndex < 0)
                {
                    return result;
                }

                if (possibleMatches.Count > 1)
                {
                    possibleMatches.RemoveAt(matchIndex);
                }
                else
                {
                    elementsBLookup.Remove(childA.Name);
                }
            }

            if (elementsBLookup.Count > 0)
            {
                return new InternalResult
                {
                    FailObject = xmlA,
                    ErrorMessage = "Different number of child elements"
                };
            }

            return new InternalResult();
        }

        private static int FindChildMatch(XElement child, IList<XElement> possibleMatches, out InternalResult result)
        {
            result = new InternalResult();
            for (var i = 0; i < possibleMatches.Count; ++i)
            {
                result = AreObjectsEqual(child, possibleMatches[i]);

                if (result.FailObject == null)
                    return i;
            }

            return -1;
        }

        public static XDocument ParseXml(string xml)
        {
            try
            {
                return XDocument.Parse(xml);
            }
            catch (Exception e)
            {
                throw new ArgumentException("The string provided is not a valid XML Document.", e);
            }
        }

        private struct InternalResult
        {
            public string ErrorMessage;
            public XObject FailObject;
        }

    }
}







