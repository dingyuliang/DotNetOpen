using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetOpen.Common.Data
{/// <summary>
 /// This class is for standardlizing list view parameters
 /// </summary>
    public class ListInput<TDataInfo>
    {
        #region 
        public string IgnoredPrefix { get; set; } = "_";
        #endregion

        #region Pagination 
        /// <summary>
        /// default value: 10
        /// </summary>
        public int PageSize { get; set; } = 10;
        /// <summary>
        /// start with 1
        /// </summary>
        public int PageNumber { get; set; } = 1;
        #endregion

        #region Sort
        public List<SortOption> SortOptions { get; set; } = new List<SortOption>();
        #endregion

        #region Search
        public string SearchText { get; set; }
        public List<string> SearchFields { get; set; } = new List<string>();
        public SearchOperator SearchOperator { get; set; } = SearchOperator.OR;
        #endregion

        #region Filter
        public List<FilterOption> FilterOptions { get; set; } = new List<FilterOption>();
        #endregion

        #region ExtraInfo
        public TDataInfo Data { get; set; }
        #endregion
    }

}
