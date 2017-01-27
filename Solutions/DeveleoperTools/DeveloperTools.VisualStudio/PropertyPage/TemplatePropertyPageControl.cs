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
		private TemplateConfiguration config = null;
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
					"PnPTemplateDisplayName", 
					typeof(string), 
					50001, 
					textDisplayName, 
					() => { config.DisplayName = textDisplayName.Text; }, 
					() => { return config.DisplayName; },
					() => { return config.DisplayName != textDisplayName.Text; }),
				new UserPropertyDescriptor(
					"PnPImagePreviewUrl", 
					typeof(string), 
					50002,
					textImagePreviewUrl, 
					() => { config.ImagePreviewUrl = textImagePreviewUrl.Text; }, 
					() => { return config.ImagePreviewUrl; },
					() => { return config.ImagePreviewUrl != textImagePreviewUrl.Text; }),
				new UserPropertyDescriptor(
					"PnPTemplateAuthor",
					typeof(string),
					50004,
					textAuthor,
					() => { config.Author = textAuthor.Text; },
					() => { return config.Author; },
					() => { return config.Author != textAuthor.Text; }),
				new UserPropertyDescriptor(
					"PnPTargetPlatform",
					typeof(TargetPlatform),
					50003,
					chkSupportSPO,
					() => { config.TargetPlatform = GetPnPTargetPlatform(); },
					() => { return config.TargetPlatform; },
					() => { return config.TargetPlatform != GetPnPTargetPlatform(); }),
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
			config = configManager.GetTemplateConfiguration(path);
		}

		private TargetPlatform GetPnPTargetPlatform()
		{
			TargetPlatform res = 0;
			if(chkSupportSPO.Checked)
			{
				res |= TargetPlatform.SPO;
			}
			if (chkSupportSP13.Checked)
			{
				res |= TargetPlatform.SP13;
			}
			if (chkSupportSP16.Checked)
			{
				res |= TargetPlatform.SP16;
			}
			return res;
		}

		private void RestoreTargetPlatfrom()
		{
			chkSupportSPO.Checked = config.TargetPlatform.HasFlag(TargetPlatform.SPO);
			chkSupportSP13.Checked = config.TargetPlatform.HasFlag(TargetPlatform.SP13);
			chkSupportSP16.Checked = config.TargetPlatform.HasFlag(TargetPlatform.SP16);
		}

		protected override void PostInitPage()
		{
			base.PostInitPage();

			RestoreTargetPlatfrom();

			textDisplayName.TextChanged += new EventHandler(ControlStateChanged);
			textImagePreviewUrl.TextChanged += new EventHandler(ControlStateChanged);
			textAuthor.TextChanged += new EventHandler(ControlStateChanged);
			chkSupportSPO.CheckedChanged += new EventHandler(ControlStateChanged);
			chkSupportSP13.CheckedChanged += new EventHandler(ControlStateChanged);
			chkSupportSP16.CheckedChanged += new EventHandler(ControlStateChanged);
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
			configManager.SetTemplateConfiguration(path, config);
		}

		private void ControlStateChanged(object sender, EventArgs e)
		{
			IsDirty = _properties.Values.Any(p => p.IsDirty);
		}
	}
}
