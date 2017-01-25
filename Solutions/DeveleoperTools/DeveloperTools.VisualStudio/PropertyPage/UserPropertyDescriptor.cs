using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SharePointPnP.DeveloperTools.VisualStudio
{
	public class UserPropertyDescriptor : PropertyDescriptor
	{
		private Type propertyType;

		public int DispId { get; protected set; }
		public Control FormControl { get; protected set; }
		public Action Setter { get; protected set; }
		public Func<object> Getter { get; protected set; }

		private Func<bool> GetDirty;
		public override Type PropertyType
		{
			get { return propertyType; }
		}

		public UserPropertyDescriptor(
			string name,
			Type type,
			int dispId,
			Control formControl,
			Action setter,
			Func<object> getter,
			Func<bool> getDirty = null) : base(name, new Attribute[] { })
		{
			propertyType = type;
			DispId = dispId;
			FormControl = formControl;
			Setter = setter;
			Getter = getter;
			GetDirty = getDirty;
		}

		public bool IsDirty
		{
			get { return GetDirty != null ? GetDirty() : false;	}
		}
		public override bool CanResetValue(object component)
		{
			return false;
		}
		public override Type ComponentType
		{
			get
			{
				return null;
			}
		}

		public override object GetValue(object component)
		{
			throw new NotImplementedException();
		}

		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public override void ResetValue(object component)
		{
		}

		public override void SetValue(object component, object value)
		{
			throw new NotImplementedException();
		}

		public override bool ShouldSerializeValue(object component)
		{
			return true;
		}

	}
}
