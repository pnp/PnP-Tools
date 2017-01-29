using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualStudio.Editors.PropertyPages;
using System.Runtime.InteropServices;
using SharePointPnP.DeveloperTools.Common.Configuration;
using SharePointPnP.DeveloperTools.Common.Helpers;
using System.IO;

namespace SharePointPnP.DeveloperTools.VisualStudio
{
	public partial class TemplatePropertyPageControl : PropPageUserControlBase
	{
		private ConfigurationManager configManager = null;
		private ProjectConfiguration config = null;
		private PropertyControlData[] _controlData = null;
		private Dictionary<string, UserPropertyDescriptor> _properties = new Dictionary<string, UserPropertyDescriptor>();

		protected override PropertyControlData[] ControlData
		{
			get
			{
				return _controlData;
			}
		}

		public TemplatePropertyPageControl() 
		{
			InitializeComponent();

			var props = new UserPropertyDescriptor[]
			{
				new UserPropertyDescriptor(
					"PnPProvisionSiteUrl", 
					typeof(string), 
					50001,
					textProvisionSiteUrl, 
					() => { config.ProvisionSiteUrl = textProvisionSiteUrl.Text; }, 
					() => { return config.ProvisionSiteUrl; },
					() => { return config.ProvisionSiteUrl != textProvisionSiteUrl.Text; }),
				new UserPropertyDescriptor(
					"PnPTemplateAuthor",
					typeof(string),
					50002,
					textAuthor,
					() => { config.Author = textAuthor.Text; },
					() => { return config.Author; },
					() => { return config.Author != textAuthor.Text; }),
			};

			var controls = new List<PropertyControlData>();
			foreach (var prop in props)
			{
				_properties[prop.Name] = prop;
				controls.Add(new PropertyControlData(prop.DispId, prop.Name, prop.FormControl, ControlDataFlags.UserPersisted));
			}
			_controlData = controls.ToArray();

			configManager = new ConfigurationManager();
			var path = VsHelper.GetActiveProjectDirectory();
			config = configManager.GetProjectConfiguration(path);
		}

		protected override void PostInitPage()
		{
			base.PostInitPage();

			textProvisionSiteUrl.TextChanged += new EventHandler(ControlStateChanged);
			textAuthor.TextChanged += new EventHandler(ControlStateChanged);
		}

		public override PropertyDescriptor GetUserDefinedPropertyDescriptor(string propertyName)
		{
			var prop = _properties.ContainsKey(propertyName) ? _properties[propertyName] : null;
			return prop;
		}

		public override bool ReadUserDefinedProperty(string propertyName, ref object value)
		{
			var res = false;
			var prop = _properties.ContainsKey(propertyName) ? _properties[propertyName] : null;
			if(prop != null)
			{
				value = prop.Getter();
				res = true;
			}
			return res;
		}

		public override void Apply()
		{
			base.Apply();

			foreach(var prop in _properties.Values)
			{
				prop.Setter();
			}
			var path = VsHelper.GetActiveProjectDirectory();
			configManager.SetProjectConfiguration(path, config);
		}

		private void ControlStateChanged(object sender, EventArgs e)
		{
			IsDirty = _properties.Values.Any(p => p.IsDirty);
		}

		private void lblImagePreviewUrl_Click(object sender, EventArgs e)
		{

		}

		private void textImagePreviewUrl_TextChanged(object sender, EventArgs e)
		{

		}
	}
}
