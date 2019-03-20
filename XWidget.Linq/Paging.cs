using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XWidget.Linq {
    /// <summary>
    /// 分頁
    /// </summary>
    /// <typeparam name="TSource">列舉成員類型</typeparam>
    public class Paging<TSource> {
        /// <summary>
        /// 起始索引
        /// </summary>
        public virtual int Skip { get; private set; }

        /// <summary>
        /// 取得筆數
        /// </summary>
        public virtual int Take { get; private set; }

        /// <summary>
        /// 資料總筆數
        /// </summary>
        public virtual int TotalCount => Source.Count();

        /// <summary>
        /// 目前所在分頁索引
        /// </summary>
        public virtual int CurrentPageIndex {
            get {
                if (Take == -1) return 0;
                return (int)Math.Floor(Skip / (double)Take);
            }
        }

        /// <summary>
        /// 總頁數
        /// </summary>
        public virtual int TotalPageCount {
            get {
                if (Take == -1) return 1;
                return (int)Math.Ceiling(TotalCount / (double)Take);
            }
        }

        /// <summary>
        /// 是否有上一頁
        /// </summary>
        public virtual bool HasPreviousPage => CurrentPageIndex > 0;

        /// <summary>
        /// 是否有下一頁
        /// </summary>
        public virtual bool HasNextPage => CurrentPageIndex < TotalPageCount - 1;

        /// <summary>
        /// 對應方法
        /// </summary>
        [JsonIgnore]
        public Func<TSource, TSource> Selector { get; set; }

        /// <summary>
        /// 分頁結果
        /// </summary>
        public virtual IEnumerable<TSource> Result {
            get {
                IEnumerable<TSource> result = null;
                if (Take == -1) {
                    result = Source.Skip(Skip);
                } else {
                    result = Source.Skip(Skip).Take(Take).ToArray();
                }

                if (Selector != null) {
                    return result.Select(x => Selector(x));
                }

                return result;
            }
        }

        /// <summary>
        /// 分頁資料來源
        /// </summary>
        [JsonIgnore]
        public virtual IEnumerable<TSource> Source { get; private set; }

        /// <summary>
        /// 預設建構子
        /// </summary>
        [Obsolete("此建構子僅供Proxy使用", true)]
        public Paging() { }

        /// <summary>
        /// 分頁建構子
        /// </summary>
        /// <param name="source">分頁資料來源</param>
        /// <param name="skip">起始索引</param>
        /// <param name="take">取得筆數，如果為-1則表示取得所有資訊不分頁</param>
        public Paging(IEnumerable<TSource> source, int skip, int take) {
            this.Source = source;
            this.Skip = skip;
            this.Take = take;
        }

        /// <summary>
        /// 移動目前所在分頁索引至指定索引
        /// </summary>
        /// <param name="pageIndex">頁數索引</param>
        /// <returns></returns>
        public bool MoveToPage(int pageIndex) {
            if (Take == -1 && pageIndex != 0) {
                return false;
            }

            var newSkip = Take * pageIndex;

            if (newSkip < 0 || newSkip >= TotalCount) return false;

            Skip = newSkip;

            return true;
        }

        /// <summary>
        /// 取得指定分頁索引的分頁物件，如無下個分頁則返回<see cref="null"/>
        /// </summary>
        /// <param name="pageIndex">頁數索引</param>
        /// <returns>分頁物件</returns>
        public Paging<TSource> GetMoveToPage(int pageIndex) {
            if (Take == -1 && pageIndex != 0) {
                pageIndex = 0;
            }

            var newSkip = Take * pageIndex;

            if (newSkip < 0 || newSkip >= TotalCount) return null;

            var result = new Paging<TSource>(Source, newSkip, Take);
            result.Selector = Selector;

            return result;
        }

        /// <summary>
        /// 前後頁移動目前所在分頁索引
        /// </summary>
        /// <param name="deltaPageCount">分頁索引變動量</param>
        /// <returns>是否移動成功</returns>
        public bool MovePage(int deltaPageCount) {
            if (Take == -1 && deltaPageCount != 0) {
                return false;
            }

            var newSkip = Skip + Take * deltaPageCount;

            if (newSkip < 0 || newSkip >= TotalCount) return false;

            Skip = newSkip;

            return true;
        }

        /// <summary>
        /// 取得前後頁移動目前所在分頁索引後的新的分頁物件，如無下個分頁則返回<see cref="null"/>
        /// </summary>
        /// <param name="deltaPageCount">分頁索引變動量</param>
        /// <returns>分頁物件</returns>
        public Paging<TSource> GetMovePage(int deltaPageCount) {
            if (Take == -1 && deltaPageCount != 0) {
                return null;
            }

            var newSkip = Skip + Take * deltaPageCount;

            if (newSkip < 0 || newSkip >= TotalCount) return null;

            var result = new Paging<TSource>(Source, newSkip, Take);
            result.Selector = Selector;

            return result;
        }

        /// <summary>
        /// 重設目前分頁索引為0
        /// </summary>
        public void Reset() {
            MoveToPage(0);
        }
    }
}
