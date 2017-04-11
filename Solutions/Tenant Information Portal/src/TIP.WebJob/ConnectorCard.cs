using Newtonsoft.Json;
using System.Collections.Generic;

namespace TIP.WebJob
{
	internal class ConnectorCard
	{
		/// <summary>
		/// A string used for summarizing card content. This will be shown as the message subject. This is required if the text parameter isn't populated.
		/// </summary>
		public string Summary { get; set; }

		/// <summary>
		/// The main text of the card. This will be rendered below the sender information and optional title, and above any sections or actions present.
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// A title for the Connector message. Shown at the top of the message.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Accent color used for branding or indicating status in the card.
		/// </summary>
		public string ThemeColor { get; set; }

		/// <summary>
		/// Contains a list of sections to display in the card
		/// </summary>
		public List<Section> Sections { get; set; }

		/// <summary>
		/// This array of Schema.org/ViewAction objects will power the action links found at the bottom of the card.
		/// </summary>
		public List<ViewAction> PotentialAction { get; set; }

		public ConnectorCard()
		{
			Sections = new List<Section>();
			PotentialAction = new List<ViewAction>();
		}
	}

	internal class Section
	{
		/// <summary>
		/// Title of the section
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		/// Title of the event or action. Often this will be the name of the "actor".
		/// </summary>
		public string ActivityTitle { get; set; }
		/// <summary>
		/// A subtitle describing the event or action. Often this will be a summary of the action.
		/// </summary>
		public string ActivitySubtitle { get; set; }
		/// <summary>
		/// An image representing the action. Often this is an avatar of the "actor" of the activity.
		/// </summary>
		public string ActivityImage { get; set; }
		/// <summary>
		/// A full description of the action.
		/// </summary>
		public string ActivityText { get; set; }
		/// <summary>
		/// An array of facts, displayed as key-value pairs.
		/// </summary>
		public List<Fact> Facts { get; set; }
		/// <summary>
		/// An array of images that will be displayed at the bottom of the section.
		/// </summary>
		public List<Image> Images { get; set; }
		/// <summary>
		/// Optional text that will appear before the activity.
		/// </summary>
		public string Text { get; set; }
		/// <summary>
		/// Set this to false to disable markdown parsing on this section's content. Markdown parsing is enabled by default.
		/// </summary>
		public bool Markdown { get; set; }
		/// <summary>
		/// This array of Schema.org/ViewAction objects will power the action links found at the bottom of the section
		/// </summary>
		public List<ViewAction> PotentialAction { get; set; }

		public Section()
		{
			Facts           = new List<WebJob.Fact>();
			Images          = new List<Image>();
			PotentialAction = new List<ViewAction>();
		}
	}

	internal class Fact
	{
		/// <summary>
		/// Name of the fact
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Value of the fact
		/// </summary>
		public string Value { get; set; }
	}

	internal class Image
	{
		/// <summary>
		/// Alt-text for the image (optional)
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// A URL to the image file or a data URI with the base64-encoded image inline.
		/// </summary>
		[JsonProperty("image")]
		public string ImageUrl { get; set; }
	}

	internal class ViewAction
	{
		[JsonProperty("@context")]
		public string Context
		{
			get { return "http://schema.org"; }
		}

		[JsonProperty("@type")]
		public string Type
		{
			get { return "ViewAction"; }
		}
		public string Name { get; set; }
		public string[] Target { get; set; }
	}
}