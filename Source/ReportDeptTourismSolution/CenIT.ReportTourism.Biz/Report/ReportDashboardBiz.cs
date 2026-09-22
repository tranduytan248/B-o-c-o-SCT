using System;
using System.Collections.Generic;
using CenIT.ReportTourism.Models.Report;
using TSFramework.App.Processors;

namespace CenIT.ReportTourism.Biz.Report
{
    public class ReportDashboardBiz

    {
        private const string DATA_PROVIDER_NAME = "ReportTourismProvider";

        private readonly string _reportDashboardStatisticEnterprise = "Report_Dashboard_StatisticEnterprise";
        private readonly string _reportDashboardStatisticIncome = "Report_Dashboard_StatisticIncome";
        private readonly string _reportDashboardStatisticMapVisitor = "Report_Dashboard_StatisticMapVisitor";
        private readonly string _reportDashboardStatisticTypeBusiness = "Report_Dashboard_StatisticTypeBusiness";
        private readonly string _reportDashboardStatisticVisitor = "Report_Dashboard_StatisticVisitor";

        public ReportStatisticEnterpriseModel GetStatisticEnterprise(DateTime? forMonth)
        {
            forMonth = forMonth ?? DateTime.Now;
            var dataStatisticEnterpriseModel =
                AppProcessor.ProcedureProvider.ExecuteScalarObject<ReportStatisticEnterpriseModel>(
                    _reportDashboardStatisticEnterprise, DATA_PROVIDER_NAME,
                    forMonth);

            return dataStatisticEnterpriseModel;
        }

        public List<ReportStatisticTypeBusinessModel> GetStatisticTypeBusiness(DateTime? forMonth)
        {
            forMonth = forMonth ?? DateTime.Now;
            var dataStatisticTypeBusinessModels =
                AppProcessor.ProcedureProvider.ExecuteTypedList<ReportStatisticTypeBusinessModel>(
                    _reportDashboardStatisticTypeBusiness, DATA_PROVIDER_NAME, forMonth);

            return dataStatisticTypeBusinessModels;
        }

        public List<ReportStatisticVisitorModel> GetStatisticVisitor(DateTime? forMonth)
        {
            forMonth = forMonth ?? DateTime.Now;
            var dataStatisticVisitorModels =
                AppProcessor.ProcedureProvider.ExecuteTypedList<ReportStatisticVisitorModel>(
                    _reportDashboardStatisticVisitor, DATA_PROVIDER_NAME, forMonth);

            return dataStatisticVisitorModels;
        }

        public List<ReportStatisticMapVisitorModel> GetStatisticMapVisitor(DateTime? forMonth)
        {
            forMonth = forMonth ?? DateTime.Now;
            var dataStatisticMapVisitorModels =
                AppProcessor.ProcedureProvider.ExecuteTypedList<ReportStatisticMapVisitorModel>(
                    _reportDashboardStatisticMapVisitor, DATA_PROVIDER_NAME, forMonth);

            return dataStatisticMapVisitorModels;
        }

        public List<ReportStatisticIncomeModel> GetStatisticIncome(DateTime? forMonth, int? typeStatistic)
        {
            forMonth = forMonth ?? DateTime.Now;
            var dataStatisticIncomeModels =
                AppProcessor.ProcedureProvider.ExecuteTypedList<ReportStatisticIncomeModel>(
                    _reportDashboardStatisticIncome, DATA_PROVIDER_NAME, forMonth, typeStatistic);

            return dataStatisticIncomeModels;
        }
    }
}