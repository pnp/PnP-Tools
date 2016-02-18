// ------------------------------------------------------------------------------
//The MIT License(MIT)

//Copyright(c) 2015 Office Developer
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
// ------------------------------------------------------------------------------



using System.Web.Mvc;

namespace TIP.Dashboard.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="id"></param>
        /// <param name="angularController"></param>
        /// <returns></returns>
        public ActionResult ExpiredPrincipals(string id, string angularController)
        {
            ViewBag.AngularControllerName = angularController;
            ViewBag.ActionName = id;
            return View();
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="id"></param>
        /// <param name="angularController"></param>
        /// <returns></returns>

        public ActionResult ExpiredPrincipals30(string id, string angularController)
        {
            ViewBag.AngularControllerName = angularController;
            ViewBag.ActionName = id;
            return View();
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="id"></param>
        /// <param name="angularController"></param>
        /// <returns></returns>
        public ActionResult ExpiredPrincipals60(string id, string angularController)
        {
            ViewBag.AngularControllerName = angularController;
            ViewBag.ActionName = id;
            return View();
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="id"></param>
        /// <param name="angularController"></param>
        /// <returns></returns>
        public ActionResult ExpiredPrincipals90(string id, string angularController)
        {
            ViewBag.AngularControllerName = angularController;
            ViewBag.ActionName = id;
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="angularController"></param>
        /// <returns></returns>
        public ActionResult Principals(string id, string angularController)
        {
            ViewBag.AngularControllerName = angularController;
            ViewBag.ActionName = id;
            return View();

        }


    }
}