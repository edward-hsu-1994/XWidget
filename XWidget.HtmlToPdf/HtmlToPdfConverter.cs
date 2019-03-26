using PuppeteerSharp;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace XWidget.HtmlToPdf {
    public class HtmlToPdfConverter {
        Browser _browser;
        /// <summary>
        /// 瀏覽器
        /// </summary>
        /// <param name="browser">瀏覽器</param>
        public HtmlToPdfConverter(Browser browser) {
            _browser = browser;
        }

        /// <summary>
        /// 將輸入的HTML
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public async Task<Stream> Convert(string html) {
            var page = await _browser.NewPageAsync();
            await page.SetContentAsync(html);
            return await page.PdfStreamAsync();
        }
    }
}
