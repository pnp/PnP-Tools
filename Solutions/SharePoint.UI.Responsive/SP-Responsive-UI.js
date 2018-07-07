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
     * Current instance class
     */
    var instance;

    /**
     * Current init statement
     */
    var initState = false;

    /**
     * Current viewport statement
     */
    var viewportState = false;
    
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
     * Change element ID and all children to avoid duplicate
     * Add a feature to manage the case of attribute for the suiteBarButtons
     * @param {element} el - Cloned Element
     * @returns {element} Cloned Element with all children with a unique id
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

    /**
     * Manage SharePoint Settings Page Responsive
     * @private
     */
    function responsivizeSettings() {
        /* return if no longer on Settings page */
        if (window.location.href.indexOf('/settings.aspx') < 0) return;

        /* find the Settings root element, or wait if not available yet */
        var settingsRoot = document.getElementsByClassName('ms-siteSettings-root')[0];
        if (!settingsRoot) {
            setTimeout(responsivizeSettings, 100);
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
    }

    /**
     * Retrieve Navigation nodes and adapt them to the custom responsive menu
     * @param {string} Delta Navigation ID
     * @returns {element} Customized Cloned Navigation
     * @private
     */
    function pnpNavGeneration(navId) {
        var nav = document.getElementById(navId);
        var clonedNav = null;
        if (nav) {
            clonedNav = nav.cloneNode(true);
            clonedNav = cloneSPIdNodes(clonedNav);

            /* Sub nodes accordion */
            var navNodes = clonedNav.querySelectorAll('li');
            for (var n = 0; n < navNodes.length; n++) {
                var navItem = navNodes[n].getElementsByClassName('menu-item')[0];
                var navRow = document.createElement('div');
                navRow.className = 'ms-core-menu-item';
                
                var checkChild = navNodes[n].getElementsByTagName('ul');
                if (checkChild.length > 0 && navItem) {
                    if (!hasClass(navNodes[n], 'dynamic-children')) {
                        navNodes[n].className = navNodes[n].className + ' dynamic-children';
                    }
                    /* Add button to preserve html link */
                    var expandBtn = document.createElement('button');
                    expandBtn.type = 'button';
                    navRow.appendChild(expandBtn);
                    expandBtn.addEventListener('click', function () {
                        toggleClass(this.parentNode.parentNode, 'expand');
                        return false;
                    });
                }

                if (navItem) {
                    navNodes[n].insertBefore(navRow, navNodes[n].firstElementChild);
                    navRow.appendChild(navItem);
                }
                
                navNodes[n].insertBefore(navRow, navNodes[n].firstElementChild);
                /* Change Edit Link navigation */
                var l = navNodes[n].querySelector('a.ms-navedit-editLinksText');
                if (l) {
                    l.removeAttribute('onclick');
                    l.href = _spPageContextInfo.webServerRelativeUrl + '/_layouts/15/AreaNavigationSettings.aspx';
                }
            }
        }
        return clonedNav;
    }

    /**
     * Allow to detect when a child node is added to a specific DOM Element (Used essentially because of SharePoint Online)
     * @param {element} p Parent Node that attach the event if child does not exist
     * @param {element} c Child Node that you're looking for
     * @returns {boolean} True if the targeted element exist, else return false
     * @private
     */
    function ensureElemCreation(p, c) {
        var r = false;
        if (!c) {
            /* Custom CSS class is added to add only once the event */
            if (!hasClass(p, 'pnp-nodeListener')) {
                p.className += ' pnp-nodeListener';
                p.addEventListener('DOMNodeInserted', function() {
                    PnPResponsiveApp.Main.setUpToggling();
                });
            }
        } else {
            r = true;
        }
        return r;
    }

    /**
     * Top Bar Responsive UI Burger
     * @returns {element} Element UI Burger
     * @private
     */
    function uiBurger() {
        var newTopItem = document.createElement('div');
        newTopItem.className = 'o365cs-nav-topItem o365cs-rsp-m-hide o365cs-rsp-tn-hideIfAffordanceOn mobile-only';
        var newTopItemChild = document.createElement('div');
        var newTopNavItem = document.createElement('button');
        newTopNavItem.type = 'button';
        newTopNavItem.className = 'o365cs-nav-item o365cs-nav-button ms-bgc-tdr-h o365button o365cs-topnavText o365cs-navMenuButton ms-bgc-tp ms-button-emphasize mobile-only';
        newTopNavItem.id = 'PnP_MainLink_Hamburger';
        newTopNavItem.setAttribute('role', 'menuitem');
        newTopNavItem.setAttribute('aria-disabled', 'false');
        newTopNavItem.setAttribute('aria-selected', 'false');
        newTopNavItem.setAttribute('aria-label', 'Open the menu to access additional app options');

        var newTopNavText = document.createElement('span');
        newTopNavText.className = 'ms-rteThemeBackColor-1-0 ms-Icon--GlobalNavButton';
        newTopNavText.setAttribute('aria-hidden', 'true');

        newTopNavItem.addEventListener('click', function () {
            var panel = document.getElementById('PnPNavPanel');
            toggleClass(panel, 'PnPPanelEnabled');
            return false;
        });
        
        newTopNavItem.appendChild(newTopNavText);
        newTopItemChild.appendChild(newTopNavItem);
        newTopItem.appendChild(newTopItemChild);

        return newTopItem;
    }

    return {
        /**
         * PnP Responsive Initialization
         * @constructor
         * @public
         */
        init: function () {
            if (!initState) {
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
                
                PnPResponsiveApp.Main.setUpToggling();
                PnPResponsiveApp.Main.setUpSuiteBarToogling();

                responsivizeSettings();
                /* also listen for dynamic page change to Settings page */
                window.onhashchange = function() { responsivizeSettings(); };

                /* 
                 * Extend/override some SP native functions to fix resizing quirks
                 * First of all save the original function definition
                 */
                var originalResizeFunction = FixRibbonAndWorkspaceDimensions;

                FixRibbonAndWorkspaceDimensions = function () {
                /* Then define a new one */
                FixRibbonAndWorkspaceDimensions = function() {
                    /* let sharepoint do its thing */
                    originalResizeFunction();
                    /* fix the body container width */
                    document.getElementById('s4-bodyContainer').style.width = document.getElementById('s4-workspace').offsetWidth + 'px';
                };
            }
            /* Init function is done */
            initState = true;
        },
        /**
         * Add viewport and support retina devices
         * @public
         */
        addViewport: function () {
            if (!viewportState) {
                var head = document.getElementsByTagName('head')[0];
                var viewport = document.createElement('meta');
                viewport.name = 'viewport';
                if (window.devicePixelRatio == 2) {
                    viewport.content = 'width=device-width, user-scalable=yes, initial-scale=.5';
                } else {
                    viewport.content = 'width=device-width, user-scalable=yes, initial-scale=1.0';
                }
                var appleMeta = document.createElement('meta');
                appleMeta.name = 'apple-mobile-web-app-capable';
                appleMeta.content = 'yes';
                head.appendChild(viewport);
                head.appendChild(appleMeta);
            }
            viewportState = true;
        },
        /**
         * Set up Toggle Button to Hide or Show responsive menu
         * @public
         */
        setUpToggling: function () {

            /* If current window is a Modal Dialog */
            var mDialog = document.getElementsByClassName('ms-dialogBody');
            if (mDialog != undefined && mDialog.length > 0) { return; }

            /* If it is already responsivized, return */
            if (document.getElementById('PnP_MainLink_Hamburger')) { return; }

            /* Get Top Bar */
            var topBar = document.getElementById('suiteBarTop');
            var topNavLeft = null;
            /* If suiteBarTop of SP2016/online doesn't exists, it is maybe because is a SP2013 ? */
            if (!topBar) {
                topBar = document.getElementById('suiteBar');
                topNavLeft = topBar.querySelector('.ms-verticalAlignMiddle');
                topNavLeft = topNavLeft ? topNavLeft.parentNode : null;
            } else {
                topNavLeft = topBar.getElementsByClassName('o365cs-nav-leftAlign')[0];
                /* Because SharePoint Online is Async and take more time to be defined */
                if (!ensureElemCreation(topBar, topNavLeft)) { return; }
            }

            if (topNavLeft) {
                /* Add burger button */
                topNavLeft.insertBefore(uiBurger(), topNavLeft.firstElementChild);

                /* Check if left panel already exist (To fix MDS feature effect) */
                if (document.getElementById('PnPNavPanel')) { return; }
                /* Left Panel */
                var spSuiteBar = document.getElementById('suiteBarDelta');
                if (!spSuiteBar) {
                    spSuiteBar = document.getElementById('suiteBar');
                }
                if (!spSuiteBar) { return; }

                var pnpNavPanel = document.createElement('div');
                pnpNavPanel.id = 'PnPNavPanel';
                /* Hide with ms-hide to avoid UI load effect */
                pnpNavPanel.className = 'ms-rteThemeBackColor-1-0 ms-dialogHidden mobile-only ms-hide';

                var pnpContentNavPanel = document.createElement('div');
                /* Content Panel */
                pnpContentNavPanel.id = 'PnPContentNavPanel';

                /* MySite Profile */
                var spMySiteProfile = pnpNavGeneration('DeltaPlaceHolderProfileImage');
                if (spMySiteProfile) {
                    pnpContentNavPanel.appendChild(spMySiteProfile);
                }

                /* TOP NAV support */
                var spTopNav = pnpNavGeneration('DeltaTopNavigation');
                if (spTopNav) {
                    spTopNav.className += ' ms-picker-line';
                    pnpContentNavPanel.appendChild(spTopNav);
                }

                /* QUICK LAUNCH nav support */
                var spQLNav = pnpNavGeneration('DeltaPlaceHolderLeftNavBar');
                if (spQLNav) {
                    spQLNav.className += ' ms-picker-line';
                    pnpContentNavPanel.appendChild(spQLNav);
                }

                /* Oslo nav support */
                var spHQLNav = pnpNavGeneration('DeltaHorizontalQuickLaunch');
                if (spHQLNav) {
                    spHQLNav.className += ' ms-picker-line';
                    pnpContentNavPanel.appendChild(spHQLNav);
                }

                /* Add bottom space to compensate for lack of height  */
                var panelBottom = document.createElement('div');
                panelBottom.className = 'PnPPanelBottom';
                pnpContentNavPanel.appendChild(panelBottom);

                /* Add Left Panel to page */
                pnpNavPanel.appendChild(pnpContentNavPanel);
                spSuiteBar.parentElement.insertBefore(pnpNavPanel, spSuiteBar.nextSibling);
            }
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
                suiteBarToggle.id = 'pnp-suiteBar-open';
                suiteBarToggle.className = 'mobile-only ms-button-emphasize';
                suiteBarToggle.type = 'button';
                var bulletStyle = document.createElement('span');
                bulletStyle.className = 'ms-rteThemeBackColor-1-0';
                bulletStyle.setAttribute('aria-hidden', 'true');
                suiteBarToggle.addEventListener('click', function () {
                    if (document.getElementById('pnp-suiteBar')) {
                        toggleClass(document.getElementById('pnp-suiteBar'), 'ms-hide');
                    }
                });
                suiteBarToggle.appendChild(bulletStyle);
                document.getElementById('DeltaSuiteLinks').appendChild(suiteBarToggle);
            }
            PnPResponsiveApp.Main.setUpSuiteBarRwd();
        },
        /**
         * Build and render Suite Bar Rwd Mode
         * @public
         * @see setUpSuiteBarToogling
         */
        setUpSuiteBarRwd: function () {
            /* if it is already responsivized, return */
            if (document.getElementById('pnp-suiteBar')) { return; }

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
                suiteBarRwd.id = 'pnp-suiteBar';
                suiteBarRwd.className = 'mobile-only ms-rteThemeBackColor-5-0 ms-hide';
                /* Close Responsive Bar Button */
                var closeSuiteBarRwd = document.createElement('button');
                closeSuiteBarRwd.id = 'pnp-suiteBar-close';
                closeSuiteBarRwd.type = 'button';
                closeSuiteBarRwd.className = 'ms-button-emphasize';
                closeSuiteBarRwd.addEventListener('click', function () {
                    toggleClass(document.getElementById('pnp-suiteBar'), 'ms-hide');
                });
                /* Render Responsive Bar */
                suiteBarRwd.appendChild(closeSuiteBarRwd);
                suiteBarRwd.appendChild(suiteBarBtnClone);
                suiteBarRwd.appendChild(suiteLinksBoxClone);
                document.getElementById('suiteBar').parentNode.insertBefore(suiteBarRwd, document.getElementById('suiteBar'));
            }
        },
        getInstance: function () {
            if (!instance) {
                instance = this;
            }
            return instance;
        }
    };
})();

/* exported responsiveStartup */
function responsiveStartup() {
    var ui = PnPResponsiveApp.Main.getInstance();
    ui.addViewport();
    ui.init();
}

/* Register Responsive Behavior after SP.js is loaded (used for SharePoint On-Premise) */
SP.SOD.executeFunc('sp.js', 'SP.ClientContext', responsiveStartup);
/* Register Responsive Behavior after SP page is loaded (used for SharePoint Online) */
_spBodyOnLoadFunctionNames.push('responsiveStartup');
