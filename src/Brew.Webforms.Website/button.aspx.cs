﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class button : System.Web.UI.Page {
	
	protected void Page_Load(object sender, EventArgs e) {

		butan.ServerClick += delegate(object s, EventArgs ea) {
			var o = brewbutton.Disabled;
		};

	}
}