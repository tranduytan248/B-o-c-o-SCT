using CenIT.ReportTourism.Models.Cate;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Cate
{
    public class CateDocBiz
    {
        private const string DATA_PROVIDER_NAME = "SysProvider";
        private readonly string _cateDocDelete = "Cate_Docs_Delete";
        private readonly string _cateDocGetById = "Cate_Docs_GetByID";

        public CateDocModel GetById(string fileId)
        {
            var cateDoc =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<CateDocModel>(_cateDocGetById, DATA_PROVIDER_NAME,
                    fileId);
            return cateDoc;
        }

        public bool Delete(CateDocModel model)
        {
            var result = AppProcessor.ProcedureProvider.Execute(_cateDocDelete, DATA_PROVIDER_NAME, model.FileId,
                model.Reason, model.SavedBy);
            return result == 1;
        }
    }
}