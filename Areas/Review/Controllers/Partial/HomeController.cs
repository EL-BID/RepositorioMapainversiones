using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using IMRepo.Models.Reports;
using IMRepo.Services.Utilities;

namespace IMRepo.Areas.Review.Controllers
{
    public partial class HomeController : Controller
    {

        PaymentTotalsModel? getPaymentTotals(int contractId)
        {
            return getTotalsFromView(contractId, "[payment_Totals]");
        }


        PaymentTotalsModel? getAdditionTotals(int contractId)
        {
            return getTotalsFromView(contractId, "[addition_Totals]");
        }

        PaymentTotalsModel? getExtensionTotals(int contractId)
        {
            return getTotalsFromView(contractId, "[extension_Totals]");
        }

        PaymentTotalsModel? getTotalsFromView(int contractId, string viewName)
        {
            PaymentTotalsModel? model = null;
            SqlConnection sqlConnection = new SqlConnection(context.Database.GetConnectionString());
            try
            {
                sqlConnection.Open();
                model = sqlConnection.Query<PaymentTotalsModel>($"select * from {viewName} where id = {contractId}").FirstOrDefault();
            }
            catch
            {
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }
            return model;
        }



    }
}
