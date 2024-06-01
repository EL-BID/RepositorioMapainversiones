using Dapper;
using IMRepo.Models.Domain;
using IMRepo.Services.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace IMRepo.Controllers
{
    public partial class PaymentController : Controller
    {


        public AdvanceBasic? paymentsGreaterThanProgrammed(Payment payment)
        {
            AdvanceBasic? advance = null;
            //SqlConnection sqlConnection = new SqlConnection(context.Database.GetConnectionString());
            //JaosDataTools jaosDataTools = new JaosDataTools();
            //try
            //{
            //    sqlConnection.Open();
            //    var parameters = new[]
            //    {
            //        new SqlParameter("@projectId", payment.Project),
            //        new SqlParameter("@paymentId", payment.Id > 0 ? payment.Id : (object)DBNull.Value) // Convert null to DBNull.Value
            //    };

            //    advance = sqlConnection.Query<AdvanceBasic>($"select * from [project_ProgrammedAndPayments]({payment.Project},{payment.Id})").FirstOrDefault();
            //    if (advance == null)
            //        advance=new AdvanceBasic();
            //    advance.actual = jaosDataTools.add(advance.actual, payment.Total);
            //    if (advance.actual.HasValue && advance.actual.Value > 0)
            //    {
            //        if (advance.programmed.HasValue)
            //        {
            //            // payments are greater than programmed
            //            if (advance.programmed.Value < advance.actual.Value)
            //                advance.invalid = true;
            //        }
            //        else // payment has value and programmed has no value.
            //            advance.invalid = true;
            //    }
            //}
            //catch
            //{
            //    throw;
            //}
            //finally
            //{
            //    sqlConnection.Close();
            //}
            return advance;
        }






    }
}
