namespace CenIT.ReportTourism.Models.Sys
{
    public class SysSearchModel
    {
        public string Search { get; set; }

        /// <summary>
        ///     Sort Column
        /// </summary>
        public string Order { get; set; }

        /// <summary>
        ///     Sort Type : DESC | ASC
        /// </summary>
        public string OrderDir { get; set; }

        /// <summary>
        ///     Current Row Index
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        ///     Page size
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        ///     Output: total rows
        /// </summary>
        public int TotalRecord { get; set; }
    }
}