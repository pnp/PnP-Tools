// ------------------------------------------------------------------------------
//The MIT License(MIT)

//Copyright(c) 2016 Office Developer
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

using Tip.Mvc.Middleware;

namespace Owin
{
    /// <summary>
    /// Extension Methods for Middleware
    /// </summary>
    public static class BuilderExtensions
    {
        /// <summary>
        /// Extension Method for using the <see cref="Tip.Mvc.Middleware.ResponseMiddleware"/> to add the response header
        /// </summary>
        /// <param name="app"></param>
        /// <returns><see cref="IAppBuilder"/></returns>
        public static IAppBuilder UseResponseMiddleware(this IAppBuilder app)
        {
            return app.Use<ResponseMiddleware>();
        }

        /// <summary>
        /// Extension method for using the <see cref="Tip.Mvc.Middleware.CorrelationMiddleware"/> to add the correlation id to the headers
        /// </summary>
        /// <param name="app"></param>
        /// <returns><see cref="IAppBuilder"/></returns>
        public static IAppBuilder UserCorrelationMiddleware(this IAppBuilder app)
        {
            return app.Use<CorrelationMiddleware>();
        }
    }
}
