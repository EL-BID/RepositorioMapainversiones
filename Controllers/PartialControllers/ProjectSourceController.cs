using Dapper;
using IMRepo.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace IMRepo.Controllers
{
    public partial class ProjectFundingController : Controller
    {


        public double projectTotalFunding(ProjectFunding projectFunding)
        {
            double total = 0;
            SqlConnection sqlConnection = new SqlConnection(context.Database.GetConnectionString());
            try
            {
                sqlConnection.Open();
                var result = sqlConnection.Query<AdvanceBasic>($"select * from project_fundingTotal({projectFunding.Project},{projectFunding.Id})").FirstOrDefault();
                if (result != null && result.programmed.HasValue)
                    total = result.programmed.Value;
                if (projectFunding.Value.HasValue)
                    total = total + projectFunding.Value.Value;
            }
            catch
            {
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }
            return total;
        }






    }
}
