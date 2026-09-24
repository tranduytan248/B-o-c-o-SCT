namespace CenIT.ReportTourism.Models.Cate
{
    /// <summary>Chỉ tiêu/sản phẩm trong danh mục Cate_BusinessProduct.</summary>
    public class CateBusinessProductModel
    {
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Unit { get; set; }
        public int? DisplayOrder { get; set; }
    }
}
