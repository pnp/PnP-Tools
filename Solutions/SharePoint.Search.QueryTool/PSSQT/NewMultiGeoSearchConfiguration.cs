using System;
using System.Management.Automation;
using System.Text;

//APC Southeast or East Asia datacenters
//AUS Southeast or East Asia datacenters
//CAN US datacenters
//EUR Europe datacenters
//FRA Europe datacenters
//GBR Europe datacenters
//KOR Southeast or East Asia datacenters
//JPN Southeast or East Asia datacenters
//NAM US datacenters


namespace PSSQT
{
    public enum MultiGeoLocation
    {
        APC,
        AUS, // Southeast or East Asia datacenters
        CAN, // US datacenters
        EUR, // Europe datacenters
        FRA, // Europe datacenters
        GBR, // Europe datacenters
        IND, // India
        KOR, // Southeast or East Asia datacenters
        JPN, // Southeast or East Asia datacenters
        NAM  // US datacenters
    }

    public class MultiGeoSearchConfiguration
    {
        public string DataLocation { get; set; }
        public Uri Endpoint { get; set; }
        public Guid SourceId { get; set; }

        public string Formatted
        {
            get {
                string formatted;

                var EndpointEscaped = Endpoint.ToString().Replace(":", "\\:");

                // [{DataLocation\:"NAM"\, Endpoint\:"https\://contosoNAM.sharepoint.com"\, SourceId\:"B81EAB55-3140-4312-B0F4-9459D1B4FFEE"}\,{DataLocation\:"CAN"\,Endpoint\:"https\://contosoCAN.sharepoint-df.com"}]'

                if (SourceId == Guid.Empty)
                {
                    formatted = $"{{DataLocation\\:\"{DataLocation}\"\\,Endpoint\\:\"{EndpointEscaped}\"}}";
                }
                else
                {
                    formatted = $"{{DataLocation\\:\"{DataLocation}\"\\,Endpoint\\:\"{EndpointEscaped}\"\\,SourceId\\:\"{SourceId}\"}}";
                }

                return formatted;
            }
        }

        // Format for POST request
        public string FormattedPost
        {
            get
            {
                string formatted;

                var EndpointEscaped = Endpoint.ToString();

                // "[{\"DataLocation\":\"NAM\",\"Endpoint\":\"https://contoso.sharepoint.com\",\"SourceId\":\"B81EAB55-3140-4312-B0F4-9459D1B4FFEE\"},{\"DataLocation\":\"CAN\",\"Endpoint\":\"https://contosoCAN.sharepoint.com\"}]"

                if (SourceId == Guid.Empty)
                {
                    formatted = $"{{\\\"DataLocation\\\":\\\"{DataLocation}\\\",\\\"Endpoint\\\":\\\"{EndpointEscaped}\\\"}}";
                }
                else
                {
                    formatted = $"{{\\\"DataLocation\\\":\\\"{DataLocation}\\\",\\\"Endpoint\\\":\\\"{EndpointEscaped}\\\",\\\"SourceId\\\":\\\"{SourceId}\\\"}}";
                }

                return formatted;
            }
        }

        public static string Format(MultiGeoSearchConfiguration[] configurations, bool isPost = false)
        {
            var result = new StringBuilder();

            result.Append("[");

            result.Append(string.Join((isPost ? "," : "\\,"), Array.ConvertAll(configurations, c => (isPost ? c.FormattedPost : c.Formatted))));

            result.Append("]");

            return result.ToString();
        }

    }

    [Cmdlet(VerbsCommon.New, "MultiGeoSearchConfiguration", DefaultParameterSetName = "DataLocationEnum")]
    [OutputType(typeof(MultiGeoSearchConfiguration))]
    public class NewMultiGeoSearchConfiguration 
        : PSCmdlet
    {

        #region ScriptParameters
        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "Data Location (String).",
            ParameterSetName = "DataLocationString"
         )]

        public String DataLocationString { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 0,
            HelpMessage = "Data Location (Enum).",
            ParameterSetName = "DataLocationEnum"
         )]

        public MultiGeoLocation DataLocation { get; set; }

        [Parameter(
            Mandatory = true,
            Position = 1,
            HelpMessage = "Endpoint Uri."
         )]

        public Uri Endpoint { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 2,
            HelpMessage = "Result Source Id."
         )]

        public Guid SourceId { get; set; }

        #endregion

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            var multiGeoSearchConfiguration = new MultiGeoSearchConfiguration();

            if (ParameterSetName == "DataLocationEnum")
            {
                DataLocationString = Enum.GetName(typeof(MultiGeoLocation),DataLocation);
            }

            multiGeoSearchConfiguration.DataLocation = DataLocationString;
            multiGeoSearchConfiguration.Endpoint = Endpoint;
            multiGeoSearchConfiguration.SourceId = SourceId;

            WriteObject(multiGeoSearchConfiguration);
        }
    }
}
