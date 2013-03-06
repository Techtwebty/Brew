﻿using System.Linq;
using System.Web.UI;

using Brew;

namespace Brew.Webforms.Widgets {

	[ParseChildren(true)]
	[PersistChildren(false)]
	public class AccordionPanel : Control, INamingContainer {

		private Control _templateContainer;

		public string Title { get; set; }

		public Control TemplateContainer {
			get {
				this.EnsureChildControls();

				return _templateContainer;
			}
		}

		[TemplateContainer(typeof(TemplateContainer))]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public ITemplate PanelContent { get; set; }

		protected override void CreateChildControls() {
			if(PanelContent != null) {
				_templateContainer = new TemplateContainer();
				PanelContent.InstantiateIn(_templateContainer);
				Controls.Add(_templateContainer);
			}
		}

		protected override void Render(HtmlTextWriter writer) {

			writer.WriteFullBeginTag("h3");

			writer.WriteBeginTag("a");
			writer.AddAttribute("href", "#");
			writer.Write(HtmlTextWriter.TagRightChar);
			writer.Write(Title);
			writer.WriteEndTag("a");

			writer.WriteEndTag("h3");

			// Panel content wrapper div
			writer.WriteFullBeginTag("div");

			base.Render(writer);

			writer.WriteEndTag("div");
		}

		/// <summary>
		/// Searches the current naming container for a server control with the specified id parameter.
		/// </summary>
		/// <param name="id">The identifier for the control to be found.</param>
		/// <returns>The specified control, or null if the specified control does not exist.</returns>
		public new Control FindControl(string id) {
			return this.TemplateContainer.Controls.All().Where(c => c.ID == id).FirstOrDefault();
		}
	}

}
