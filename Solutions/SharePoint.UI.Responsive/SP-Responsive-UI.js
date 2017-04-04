/*
 * PnP SharePoint - Responsiveness
 *
 * @See : https://github.com/SharePoint/PnP-Guidance/blob/master/articles/Embedding-JavaScript-into-SharePoint.md
 */

/*
 * Load Namespace and managing MDS
 */
if (window.hasOwnProperty('Type')) {
    Type.registerNamespace('PnPResponsiveApp');
} else {
    window.PnPResponsiveApp = window.PnPResponsiveApp || {};
}

PnPResponsiveApp.Main = (function() {
    /*
     * Toggle element class
     *
     */
    function toggleClass(el, cls) {
        if (hasClass(el, cls)) {
            var reg = new RegExp('(\\s|^)' + cls + '(\\s|$)');
            el.className = el.className.replace(reg, ' ');
        } else {
            el.className = el.className + ' ' + cls;
        }
    }

    /*
     * Check if className exists
     *
     * Return true if exists
     */
    function hasClass(el, cls) {
        return (' ' + el.className + ' ').indexOf(' ' + cls + ' ') === -1 ? false : true;
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
    /*
     * (Not need anymore)
     */
    function loadScript(url, callback) {
        var head = document.getElementsByTagName('head')[0];
        var script = document.createElement('script');
        script.type = 'text/javascript';
        script.src = url;
        script.onreadystatechange = callback;
        script.onload = callback;
        head.appendChild(script);
    }

    return {
        init: function() {
            var currentScriptUrl = document.getElementById('PnPResponsiveUI').src;
            if (currentScriptUrl != undefined) {
                var currentScriptBaseUrl = currentScriptUrl.substring(0, currentScriptUrl.lastIndexOf('/') + 1);
                loadCSS(currentScriptBaseUrl + 'sp-responsive-ui.css');
            }

            PnPResponsiveApp.Main.setUpToggling();
            PnPResponsiveApp.Main.responsivizeSettings();

            // also listen for dynamic page change to Settings page
            window.onhashchange = function() { PnPResponsiveApp.Main.responsivizeSettings(); };

            // Extend/override some SP native functions to fix resizing quirks

            // First of all save the original function definition
            var originalResizeFunction = FixRibbonAndWorkspaceDimensions;

            // Then define a new one
            FixRibbonAndWorkspaceDimensions = function() {
                // let sharepoint do its thing
                originalResizeFunction();
                // fix the body container width
                document.getElementById('s4-bodyContainer').style.width = document.getElementById('s4-workspace').offsetWidth;
            };
        },
        /*
         * Add viewport and support retina devices
         */
        addViewport: function() {
            var head = document.getElementsByTagName('head')[0];
            var viewport = document.createElement('meta');
            viewport.name = 'viewport';
            if (window.devicePixelRatio == 2) {
                viewport.content = 'width=device-width, user-scalable=no, initial-scale=.5, maximum-scale=.5, minimum-scale=.5';
            } else {
                viewport.content = 'width=device-width, user-scalable=yes, initial-scale=1.0, maximum-scale=1.0, minimum-scale=1.0';
            }
            var appleMeta = document.createElement('meta');
            appleMeta.name = 'apple-mobile-web-app-capable';
            appleMeta.content = 'yes';
            head.appendChild(viewport);
            head.appendChild(appleMeta);
        },
        responsivizeSettings: function() {
            // return if no longer on Settings page
            if (window.location.href.indexOf('/settings.aspx') < 0) return;

            // find the Settings root element, or wait if not available yet
            var settingsRoot = document.getElementsByClassName('ms-siteSettings-root')[0];
            if (!settingsRoot) {
                setTimeout(PnPResponsiveApp.Main.responsivizeSettings, 100);
                return;
            }

            var linkSettingsSectionsLvl = settingsRoot.getElementsByClassName('ms-linksection-level1');
            for (var i = 0; i < linkSettingsSectionsLvl.length; i++) {
                var self = linkSettingsSectionsLvl[i];
                var settingsDiv = document.createElement('div');
                settingsDiv.className = 'pnp-settingsdiv';
                settingsDiv.appendChild(self.getElementsByTagName('img')[0]);
                settingsDiv.appendChild(self.getElementsByClassName('ms-linksection-textCell')[0]);
                settingsRoot.appendChild(settingsDiv);
            }
            settingsRoot.removeChild(settingsRoot[0].getElementsByTagName('table')[0]);
        },
        setUpToggling: function() {
            // if it is already responsivized, return
            if (document.getElementById('navbar-toggle'))
                return;

            // Set up sidenav toggling
            var topNav = document.getElementById('DeltaTopNavigation');
            var topNavClone = topNav.cloneNode(true);
            topNavClone.className = topNavClone.className + ' mobile-only';
            topNavClone.id = topNavClone.getAttribute('id') + '_mobileClone';
            topNav.className = topNav.className + ' no-mobile';
            document.getElementById('sideNavBox').appendChild(topNavClone);

            var sideNavToggle = document.createElement('button');
            sideNavToggle.id = 'navbar-toggle';
            sideNavToggle.className = 'mobile-only burger';
            sideNavToggle.type = 'button';
            sideNavToggle.innerHTML = '<span></span>';
            sideNavToggle.addEventListener('click', function() {
                toggleClass(document.getElementsByTagName('body')[0], 'shownav');
                toggleClass(sideNavToggle, 'selected');
            });
            document.getElementById('pageTitle').parentNode.insertBefore(sideNavToggle, document.getElementById('pageTitle'));
        }
    };
})();

PnPResponsiveApp.Main.addViewport();
PnPResponsiveApp.Main.init();