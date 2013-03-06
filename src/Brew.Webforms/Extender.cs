﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Brew.Webforms {

	public abstract class Extender : ExtenderControl, IWidget, IPostBackDataHandler, IPostBackEventHandler {

		private Control _targetControl;
		private WidgetState _widgetState;
		private String _widgetName;

		protected Extender(String widgetName) {
			if(string.IsNullOrEmpty(widgetName)) {
				throw new ArgumentException("The parameter must not be empty", "widgetName");
			}

			_widgetName = widgetName;
			_widgetState = new WidgetState(this);

			SetDefaultOptions();
		}

		[Browsable(false)]
		private WidgetState WidgetState { get { return this._widgetState; } }

		private Control TargetControl {
			get {
				if(_targetControl == null) {
					FindTargetControl();
				}

				return _targetControl;
			}
		}

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);

			WidgetState.SetWidgetNameOnTarget(this.TargetControl as IAttributeAccessor);
			WidgetState.AddPagePreRenderCompleteHandler();
		}

		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);

			if(this.TargetControl.Visible) {
				Page.RegisterRequiresPostBack(this);
				WidgetState.ParseEverything(this.TargetControl);
			}
		}

		protected virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection) {
			WidgetState.LoadPostData();
			return false;
		}

		protected virtual void RaisePostDataChangedEvent() {
			if(AutoPostBack && !Page.IsPostBackEventControlRegistered) {
				Page.AutoPostBackControl = this;
			}		
		}

		protected override IEnumerable<ScriptDescriptor> GetScriptDescriptors(Control targetControl) {
			return null;
		}

		protected override IEnumerable<ScriptReference> GetScriptReferences() {
			return BrewApplication.GetReferences();
		}

		protected virtual IDictionary<string, object> SaveOptionsAsDictionary() {
			return WidgetState.ParseOptions();
		}

		protected virtual void SetDefaultOptions() {
			WidgetState.SetDefaultOptions();
		}

		private void FindTargetControl() {
			_targetControl = FindControl(TargetControlID);

			if(_targetControl == null) {
				throw new ArgumentNullException("TargetControl is null");
			}
		}

		#region IWidget Implementation

		/// <summary>
		/// Disables (true) or enables (false) the widget.
		/// </summary>
		[WidgetOption("disabled", false)] // every widget has a disabled option.
		[Browsable(false)]
		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Disables (true) or enables (false) the widget.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool Disabled {
			get {
				return (bool)(ViewState["Disabled"] ?? false);
			}
			set {
				ViewState["Disabled"] = value;
			}
		}

		/// <summary>
		/// True, if the control should automatically postback to the server after the selected value is changed. False, otherwise.
		/// </summary>
		[DefaultValue(false)]
		[Description("True, if the control should automatically postback to the server after the selected value is changed. False, otherwise.")]
		[Category("Behavior")]
		public bool AutoPostBack {
			get {
				return (bool)(ViewState["AutoPostBack"] ?? false);
			}
			set {
				ViewState["AutoPostBack"] = value;
			}
		}
		
		/// <summary>
		/// The jQuery UI name of the widget.
		/// </summary>
		[Browsable(false)]
		public string WidgetName { get { return this._widgetName; } }
		
		Page IWidget.Page { get { return Page; } }

		string IWidget.ClientID { get { return ClientID; } }

		string IWidget.UniqueID { get { return UniqueID; } }

		string IWidget.TargetClientID { get { return this.TargetControl.ClientID; } }

		bool IWidget.Visible { get { return Visible; } }

		void IWidget.SaveWidgetOptions() {
			((IWidget)this).WidgetOptions = SaveOptionsAsDictionary();
		}

		IDictionary<string, object> IWidget.WidgetOptions { get; set; }

		#endregion

		#region IPostBackDataHandler implementation

		bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection) {
			return LoadPostData(postDataKey, postCollection);
		}

		void IPostBackDataHandler.RaisePostDataChangedEvent() {
			RaisePostDataChangedEvent();
		}

		#endregion

		#region IPostBackEventHandler Implementation

		void IPostBackEventHandler.RaisePostBackEvent(string eventArgument) {
			if(AutoPostBack && !Page.IsPostBackEventControlRegistered) {
				Page.AutoPostBackControl = this;
			}
		
			WidgetState.RaisePostBackEvent(eventArgument);
		}

		#endregion

	}
}