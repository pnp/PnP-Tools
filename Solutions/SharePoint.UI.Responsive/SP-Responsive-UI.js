/**
 * PnP SharePoint - Responsiveness
 * @see {@link https://github.com/SharePoint/PnP-Guidance/blob/master/articles/Embedding-JavaScript-into-SharePoint.md|PnP Guidance}
 * @see {@link http://usejsdoc.org/|JSDoc}
 */

/*
 * PnPResponsiveApp
 * @namespace
 */
if (window.hasOwnProperty('Type')) {
    Type.registerNamespace('PnPResponsiveApp');
} else {
    window.PnPResponsiveApp = window.PnPResponsiveApp || {};
}

/**
 * PnP Responsive
 * @class
 */
PnPResponsiveApp.Main = (function () {
    /**
     * Toggle element class
     * @param {element} el - Element
     * @param {string} cls - CSS Class Name
     * @private
     */
    function toggleClass(el, cls) {
        if (hasClass(el, cls)) {
            var reg = new RegExp('(\\s|^)' + cls + '(\\s|$)');
            el.className = el.className.replace(reg, ' ');
        } else {
            el.className = el.className + ' ' + cls;
        }
    }

    /**
     * Check if className exists
     * @param {element} el - Element
     * @param {string} cls - CSS Class Name
     * @returns {boolean} True if element has css class
     * @private
     */
    function hasClass(el, cls) {
        try {
            return (' ' + el.className + ' ').indexOf(' ' + cls + ' ') === -1 ? false : true;
        } catch (hc) {
            return false;
        }
    }

    /**
     * Dynamic CSS/JS embedding and loading
     * @private
     */
    function loadCSS(url) {
        var head = document.getElementsByTagName('head')[0];
        var style = document.createElement('link');
        style.type = 'text/css';
        style.rel = 'stylesheet';
        style.href = url;
        head.appendChild(style);
    }

    /**
     * Change element ID and all childs to avoid duplicates
     * Add a feature to manage the case of attributes for the suiteBarButtons
     * @param {element} el - Cloned Element
     * @returns {element} Cloned Element with all childs with a unique id
     * @private
     */
    function cloneSPIdNodes(el) {
        el.id = el.getAttribute('id') + '_mobileClone';
        var childs = el.getElementsByTagName('*');
        /* Regex to Get all SiteActionsMenuMain and SiteActionsMenu occurences in childs attributes */
        var regex = new RegExp("(.*?zz.*?(?:_SiteActionsMenuMain|_SiteActionsMenu))", "g");
        for (var n = 0; n < childs.length; n++) {
            if (childs[n].id) {
                /* Update All Attributes that contains Regex ID */
                var allAttributes = childs[n].attributes;
                for (var a = 0; a < allAttributes.length; a++) {
                    if (allAttributes[a].name != 'id') {
                        allAttributes[a].value = allAttributes[a].value.replace(regex, "$1_mobileClone");
                    }
                }
                /* Update Child ID */
                childs[n].id = childs[n].getAttribute('id') + '_mobileClone';
            }
        }
        return el;
    }

    return {
        /**
         * PnP Responsive Initialization
         * @constructor
         * @public
         */
        init: function () {
            var currentScriptUrl;

            var currentScript = document.getElementById('PnPResponsiveUI');
            if (currentScript != undefined) {
                currentScriptUrl = currentScript.src;
            }

            if (currentScriptUrl == undefined) {
                var responsiveScripts = document.querySelectorAll("script[src$='sp-responsive-ui.js']");
                if (responsiveScripts.length > 0) {
                    currentScriptUrl = responsiveScripts[0].src;
                }
            }
            if (currentScriptUrl != undefined) {
                var currentScriptBaseUrl = currentScriptUrl.substring(0, currentScriptUrl.lastIndexOf('/') + 1);
                loadCSS(currentScriptBaseUrl + 'sp-responsive-ui.css');
            }

            /* This is a Modal Dialog, so no navigation */
            if (document.getElementsByTagName('body')[0] && hasClass(document.getElementsByTagName('body')[0], 'ms-dialogBody')) { return; } 

            PnPResponsiveApp.Main.setUpToggling();
            PnPResponsiveApp.Main.responsivizeSettings();
            PnPResponsiveApp.Main.setUpSuiteBarToogling();

            // also listen for dynamic page change to Settings page
            window.onhashchange = function () { PnPResponsiveApp.Main.responsivizeSettings(); };

            // Extend/override some SP native functions to fix resizing quirks

            // First of all save the original function definition
            var originalResizeFunction = FixRibbonAndWorkspaceDimensions;

            // Then define a new one
            FixRibbonAndWorkspaceDimensions = function () {
                // let sharepoint do its thing
                originalResizeFunction();
                // fix the body container width
                document.getElementById('s4-bodyContainer').style.width = document.getElementById('s4-workspace').offsetWidth;
            };
        },
        /**
         * Add viewport and support retina devices
         * @public
         */
        addViewport: function () {
            var head = document.getElementsByTagName('head')[0];
            var viewport = document.createElement('meta');
            viewport.name = 'viewport';
            if (window.devicePixelRatio == 2) {
                viewport.content = 'width=device-width, user-scalable=yes, initial-scale=.5, shrink-to-fit';
            } else {
                viewport.content = 'width=device-width, user-scalable=yes, initial-scale=1.0, shrink-to-fit';
            }
            var appleMeta = document.createElement('meta');
            appleMeta.name = 'apple-mobile-web-app-capable';
            appleMeta.content = 'yes';
            head.appendChild(viewport);
            head.appendChild(appleMeta);
        },
        /**
         * Manage SharePoint Settings Page Responsive
         * @public
         */
        responsivizeSettings: function () {
            /* return if no longer on Settings page */
            if (window.location.href.indexOf('/settings.aspx') < 0) return;

            /* find the Settings root element, or wait if not available yet */
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
            if (settingsRoot.getElementsByTagName('table')[0]) {
                settingsRoot.removeChild(settingsRoot.getElementsByTagName('table')[0]);
            }
        },
        /**
         * Set up Toggle Button to Hide or Show responsive menu
         * @public
         */
        setUpToggling: function () {
            /* if it is already responsivized, return */
            if (document.getElementById('navbar-toggle')) { return; }

            /* Set up sidenav toggling */
            var topNav = document.getElementById('DeltaTopNavigation');
            var topNavClone = topNav.cloneNode(true);
            topNavClone.className = topNavClone.className + ' mobile-only';
            topNavClone = cloneSPIdNodes(topNavClone);
            /* Sub nodes accordion */
            var childs = topNavClone.querySelectorAll('a.dynamic-children');
            for (var c = 0; c < childs.length; c++) {
                /* Add button to preserve html link */
                var expandBtn = document.createElement('button');
                expandBtn.type = 'button';
                var expandSpan = document.createElement('span');
                expandBtn.appendChild(expandSpan);
                childs[c].parentNode.insertBefore(expandBtn, childs[c]);
                expandBtn.addEventListener('click', function () {
                    toggleClass(this.parentNode, 'expand');
                    return false;
                });
            }
            topNav.className = topNav.className + ' no-mobile';
            document.getElementById('sideNavBox').appendChild(topNavClone);

            var sideNavToggle = document.createElement('button');
            sideNavToggle.id = 'navbar-toggle';
            sideNavToggle.className = 'mobile-only burger';
            sideNavToggle.type = 'button';
            sideNavToggle.innerHTML = '<span></span>';
            sideNavToggle.addEventListener('click', function () {
                toggleClass(document.getElementsByTagName('body')[0], 'shownav');
                toggleClass(sideNavToggle, 'selected');
            });
            document.getElementById('pageTitle').parentNode.insertBefore(sideNavToggle, document.getElementById('pageTitle'));
        },
        /**
         * Set Up Toggle and Top SuiteBar Responsive
         * Available only for SharePoint 2013 (SharePoint 2016/Online have already a responsive suiteBar)
         * @public
         */
        setUpSuiteBarToogling: function () {
            if (document.getElementById('suiteBar')) {
                /* Open Responsive Bar Button that replace Suite Bar Links */
                var suiteBarToggle = document.createElement('button');
                suiteBarToggle.id = 'suiteBar-open';
                suiteBarToggle.className = 'mobile-only ms-button-emphasize';
                suiteBarToggle.type = 'button';
                suiteBarToggle.innerHTML = '<span></span>';
                suiteBarToggle.addEventListener('click', function () {
                    if (document.getElementById('suiteBar-rwd')) {
                        toggleClass(document.getElementById('suiteBar-rwd'), 'ms-hide');
                    }
                });
                document.getElementById('DeltaSuiteLinks').appendChild(suiteBarToggle);
            }
            PnPResponsiveApp.Main.setUpSuiteBarRwd();
        },
        /**
         * Build and render Suite Bar Rwd Mode
         * @public
         */
        setUpSuiteBarRwd: function () {
            /* if it is already responsivized, return */
            if (document.getElementById('suiteBar-rwd')) { return; }

            /*
             * Get if it's SharePoint 2013 environment
             * 
             * suiteBar is used by SharePoint 2013
             * suiteBarDelta is used by SharePoint 2016/Online
             */
            if (document.getElementById('suiteBar')) {
                /* Clone Suite Bar Buttons */
                var suiteBarBtn = document.getElementById('suiteBarButtons');
                var suiteBarBtnClone = suiteBarBtn.cloneNode(true);
                suiteBarBtnClone = cloneSPIdNodes(suiteBarBtnClone);
                /* Clone Suite Bar links (Newsfeed, OneDrive, Sites, etc.) */
                var suiteLinksBox = document.getElementById('suiteLinksBox');
                var suiteLinksBoxClone = suiteLinksBox.cloneNode(true);
                suiteLinksBoxClone = cloneSPIdNodes(suiteLinksBoxClone);
                /* Create responsive bar */
                var suiteBarRwd = document.createElement('div');
                suiteBarRwd.id = 'suiteBar-rwd';
                suiteBarRwd.className = 'mobile-only ms-rteThemeBackColor-5-0 ms-hide';
                /* Close Responsive Bar Button */
                var closeSuiteBarRwd = document.createElement('button');
                closeSuiteBarRwd.id = 'suiteBar-close';
                closeSuiteBarRwd.type = 'button';
                closeSuiteBarRwd.className = 'ms-button-emphasize';
                closeSuiteBarRwd.addEventListener('click', function () {
                    toggleClass(document.getElementById('suiteBar-rwd'), 'ms-hide');
                });
                /* Render Responsive Bar */
                suiteBarRwd.appendChild(closeSuiteBarRwd);
                suiteBarRwd.appendChild(suiteBarBtnClone);
                suiteBarRwd.appendChild(suiteLinksBoxClone);
                document.getElementById('aspnetForm').insertBefore(suiteBarRwd, document.getElementById('suiteBar'));
            }
        }
    };
})();

// Register Responsive Behavior after SP page is loaded
_spBodyOnLoadFunctionNames.push("responsiveStartup");

/* exported responsiveStartup */
function responsiveStartup() {
    PnPResponsiveApp.Main.addViewport();
    PnPResponsiveApp.Main.init();
}
