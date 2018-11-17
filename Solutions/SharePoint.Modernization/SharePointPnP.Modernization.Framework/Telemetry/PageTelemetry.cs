using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Generic;

namespace SharePointPnP.Modernization.Framework.Telemetry
{
    public class PageTelemetry
    {
        private readonly TelemetryClient telemetryClient;

        #region Construction
        /// <summary>
        /// Instantiates the telemetry client
        /// </summary>
        public PageTelemetry(string version)
        {
            try
            {
                this.telemetryClient = new TelemetryClient
                {
                    // TEST key
                    InstrumentationKey = "373400f5-a9cc-48f3-8298-3fd7f4c063d6"
                };

                // Setting this is needed to make metric tracking work
                TelemetryConfiguration.Active.InstrumentationKey = this.telemetryClient.InstrumentationKey;

                this.telemetryClient.Context.Session.Id = Guid.NewGuid().ToString();
                this.telemetryClient.Context.Cloud.RoleInstance = "SharePointPnPPageTransformation";
                this.telemetryClient.Context.Device.OperatingSystem = Environment.OSVersion.ToString();
                this.telemetryClient.Context.GlobalProperties.Add("Version", version);
            }
            catch (Exception ex)
            {
                this.telemetryClient = null;
            }
        }
        #endregion

        public void LogTransformationDone(TimeSpan duration)
        {
            if (this.telemetryClient == null)
            {
                return;
            }

            try
            {
                // Prepare event data
                Dictionary<string, string> properties = new Dictionary<string, string>();
                Dictionary<string, double> metrics = new Dictionary<string, double>();

                if (duration != null)
                {
                    properties.Add("Duration", duration.Seconds.ToString());
                }

                this.telemetryClient.TrackEvent("TransformationEngine.PageDone", properties, metrics);

                // Also add to the metric of transformed pages via the service endpoint
                this.telemetryClient.GetMetric($"TransformationEngine.PagesTransformed").TrackValue(1);
                this.telemetryClient.GetMetric($"TransformationEngine.PageDuration").TrackValue(duration.TotalSeconds);
            }
            catch
            {
                // Eat all exceptions 
            }
        }

        public void LogError(Exception ex, string location)
        {
            if (this.telemetryClient == null || ex == null)
            {
                return;
            }

            try
            {
                // Prepare event data
                Dictionary<string, string> properties = new Dictionary<string, string>();
                Dictionary<string, double> metrics = new Dictionary<string, double>();

                if (!string.IsNullOrEmpty(location))
                {
                    properties.Add("Location", location);
                }

                this.telemetryClient.TrackException(ex, properties, metrics);
            }
            catch (Exception ex2)
            {
                // Eat all exceptions 
            }
        }

        /// <summary>
        /// Ensure telemetry data is send to server
        /// </summary>
        public void Flush()
        {
            try
            {
                // before exit, flush the remaining data
                this.telemetryClient.Flush();
            }
            catch
            {
                // Eat all exceptions
            }
        }
    }
}
