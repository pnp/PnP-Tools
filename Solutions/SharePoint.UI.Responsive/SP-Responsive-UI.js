/* PnP SharePoint - Responsiveness */

var PnPResponsiveApp = PnPResponsiveApp || {};

PnPResponsiveApp.responsivizeSettings = function () {
	// return if no longer on Settings page
	if (window.location.href.indexOf('/settings.aspx') < 0) return;
	
	// find the Settings root element, or wait if not available yet
	var settingsRoot = $(".ms-siteSettings-root");
	if (!settingsRoot.length) {
		setTimeout(PnPResponsiveApp.responsivizeSettings, 100);
        return;
	}
	
	$(".ms-siteSettings-root .ms-linksection-level1").each(function () {
		var self = $(this);
		var settingsDiv = $('<div>');
		settingsDiv.addClass("pnp-settingsdiv");
		self.find(".ms-linksection-iconCell img").appendTo(settingsDiv);
		self.find(".ms-linksection-textCell").children().appendTo(settingsDiv);
		settingsDiv.appendTo(settingsRoot);
	});
	settingsRoot.find("table").remove();
}


PnPResponsiveApp.setUpToggling = function () {
	// if it is already responsivized, return
    if ($("#navbar-toggle").length)
        return;

    // Set up sidenav toggling
    var topNav = $('#DeltaTopNavigation');
    var topNavClone = topNav.clone()
    topNavClone.addClass('mobile-only');
    topNavClone.attr('id', topNavClone.attr('id') + "_mobileClone");
    topNav.addClass('no-mobile');
    $('#sideNavBox').append(topNavClone);
    var sideNavToggle = $('<button>');
    sideNavToggle.attr('id', 'navbar-toggle')
    sideNavToggle.addClass('mobile-only');
	sideNavToggle.addClass('burger');
    sideNavToggle.attr('type', 'button');
	sideNavToggle.html("<span></span>");
    sideNavToggle.click(function() { 
        $("body").toggleClass('shownav');
		sideNavToggle.toggleClass('selected');
    });
    $("#pageTitle").before(sideNavToggle);
}

PnPResponsiveApp.init = function () {
    if (!window.jQuery) {
        // jQuery is needed for PnP Responsive UI to run, and is not fully loaded yet, try later
        setTimeout(PnPResponsiveApp.init, 100);
    } else {
        $(function() { // only execute when DOM is fully loaded

            // embedding and loading of all necessary CSS files and JS libraries
            var currentScriptUrl = $('#PnPResponsiveUI').attr('src');
            if (currentScriptUrl != undefined) {
                var currentScriptBaseUrl = currentScriptUrl.substring(0, currentScriptUrl.lastIndexOf("/") + 1);

                addViewport();
                loadCSS(currentScriptBaseUrl + 'SP-Responsive-UI.css');
            }

            PnPResponsiveApp.setUpToggling();
			PnPResponsiveApp.responsivizeSettings();
			
			// also listen for dynamic page change to Settings page
			window.onhashchange = function () { PnPResponsiveApp.responsivizeSettings(); };
			
			// extend/override some SP native functions to fix resizing quirks
			var originalResizeFunction = FixRibbonAndWorkspaceDimensions;
			FixRibbonAndWorkspaceDimensions = function() {
				// let sharepoint do its thing
				originalResizeFunction();
				// fix the body container width
				$("#s4-bodyContainer").width($("#s4-workspace").width() );
			}
        });
    }
}

/* Dynamic CSS/JS embedding and loading */
function loadCSS(url) {
    var head = document.getElementsByTagName('head')[0];
    var style = document.createElement('link');
	style.type = 'text/css';
	style.rel = 'stylesheet';
    style.href = url;
    head.appendChild(style);
}
function loadScript(url, callback) {
    var head = document.getElementsByTagName('head')[0];
    var script = document.createElement('script');
    script.type = 'text/javascript';
    script.src = url;
    script.onreadystatechange = callback;
    script.onload = callback;
    head.appendChild(script);
}
function addViewport() {
    var head = document.getElementsByTagName('head')[0];
    var viewport = document.createElement('meta');
    viewport.name= "viewport";
    viewport.content= "width=device-width, initial-scale=1"; 
    head.appendChild(viewport);
}


// embedding of jQuery, and initialization of responsiveness when ready
loadScript("//code.jquery.com/jquery-1.12.0.min.js", function() {
    PnPResponsiveApp.init();
});
