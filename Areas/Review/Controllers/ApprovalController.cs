using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IMRepo.Areas.Review.Models.Approval;
using IMRepo.Data;
using IMRepo.Models.Domain;

namespace IMRepo.Areas.Review.Controllers
{
    [Authorize]
    [Area("Review")]
    public class ApprovalController : Controller
    {
        private readonly IMRepoDbContext context;

        public ApprovalController(IMRepoDbContext context
            )
        {
            this.context = context;
        }

        List<int> paymentStages = new List<int> { ProjectGlobals.PaymentStage.Presentado, ProjectGlobals.PaymentStage.Aprobado };
        List<int> taskStages = new List<int> { ProjectGlobals.TaskStage.Presentada };  // 1 a iniciar 2 en proceso 3 aprobada

        // GET: Approval/Payments
        public IActionResult Payments()
        {
            if (!User.IsInRole(ProjectGlobals.RoleDireccion)
                && !User.IsInRole(ProjectGlobals.RoleAdmin))
                return NotFound();

            return View(fillPayments());
        }

        // GET: Approval/Additions
        public IActionResult Additions()
        {
            if (!User.IsInRole(ProjectGlobals.RoleDireccion)
                && !User.IsInRole(ProjectGlobals.RoleAdmin))
                return NotFound();

            return View(fillAdditions());
        }


        // GET: Approval/Extensions
        public IActionResult Extensions()
        {
            if (!User.IsInRole(ProjectGlobals.RoleDireccion)
                && !User.IsInRole(ProjectGlobals.RoleAdmin))
                return NotFound();

            return View(fillExtensions());
        }


        //==================================================================
        //
        //    Fill Data for Views

        List<Payment> fillPayments()
        {
            List<Payment> payments = context.Payment
                .Include(p => p.Product_info)
                .ThenInclude(pr => pr.Project_info)
                .Include(p => p.Stage_info)
                .Where(p => paymentStages.Contains(p.Stage)).ToList();
            return payments;
        }

        List<Addition> fillAdditions()
        {
            List<Addition> additions = context.Addition
                .Include(p => p.Product_info)
                .ThenInclude(p => p.Project_info)
                .Include(p => p.Stage_info)
                .Where(p => taskStages.Contains(p.Stage)).ToList();
            return additions;
        }

        List<Extension> fillExtensions()
        {
            List<Extension> extensions = context.Extension
                .Include(p => p.Project_info)
                .Include(p => p.Stage_info)
                .Where(p => taskStages.Contains(p.Stage)).ToList();
            return extensions;
        }


        List<GeneralApprovalModel> fillPendingSummary()
        {
            List<GeneralApprovalModel> generalApprovalModels = new List<GeneralApprovalModel>();

            return generalApprovalModels;

        }

    }
}
