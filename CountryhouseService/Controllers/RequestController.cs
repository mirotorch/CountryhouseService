using CountryhouseService.Data;
using CountryhouseService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using X.PagedList;

namespace CountryhouseService.Controllers
{
    public class RequestController : Controller
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly IEmailSender _emailSender;
        public RequestController(ApplicationDbContext dbcontext, IEmailSender emailSender)
        {
            _dbcontext = dbcontext;
            _emailSender = emailSender;
        }

        [HttpPost]
        [Authorize(Roles = "Worker")]
        public async Task<IActionResult> Create(string requestdescription, int adid)
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Request request = new Request
            {
                AdId = adid,
                Comment = requestdescription,
                WorkerId = currentUserId,
                Status = "Sent"
            };
            await _dbcontext.Requests.AddAsync(request);
            await _dbcontext.SaveChangesAsync();
            return Redirect("Index");
        }

        [Authorize]
        public async Task<IActionResult> Index(int? page)
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<Ad> ads = _dbcontext.Ads.ToList();
            List<User> users = _dbcontext.Users.ToList();
            IEnumerable<Request> requestenum = _dbcontext.Requests;
            if (User.IsInRole("Worker"))
            {
                requestenum = requestenum.Where(x => x.WorkerId.Equals(currentUserId));
            }
            else if (User.IsInRole("Owner"))
            {
                List<Ad> ownerads = ads.Where(s => s.CreatorId == currentUserId).ToList();
                requestenum = requestenum.Where(x => ownerads.Contains(x.Ad));
            }
            int pageSize = 6;
            int pageNumber = page ?? 1;
            IPagedList<Request> requestlist = requestenum.ToPagedList(pageNumber, pageSize);
            RequestPagedResult result = new RequestPagedResult
            {
                RequestList = requestlist,
                Ads = ads,
                Users = users
            };
            return View(result);
        }

        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Accept(int? id)
        {
            Request? request = _dbcontext.Requests.Find(id);
            if (request != null)
            {
                request.Status = "Accepted";
                var declinedrequests = _dbcontext.Requests.Where(s => s.Status == "Sent" && 
                s.AdId == request.AdId && s.RequestId != request.RequestId);
                _dbcontext.Requests.RemoveRange(declinedrequests);
                _dbcontext.Ads.Find(request.AdId).Status = "Accepted";
                _dbcontext.SaveChanges();
            }
            return RedirectToAction("");
        }

        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Decline(int? id)
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Request? request = _dbcontext.Requests.Find(id);
            if (request != null)
            {
                Ad? ad = _dbcontext.Ads.Find(request.AdId);
                if (ad != null && ad.CreatorId == currentUserId)
                {
                    await _emailSender.SendEmailAsync(_dbcontext.Users.Find(currentUserId).Email, "Your request has been declined",
                    $"Your request on ad '{ad.Title}' has been declined by the owner.");
                    _dbcontext.Requests.Remove(request);
                    _dbcontext.SaveChanges();
                }
            }
            return RedirectToAction("");
        }

        [Authorize(Roles = "Worker")]
        public async Task<IActionResult> Delete(int? id)
        {
            Request? request = _dbcontext.Requests.Find(id);
            if (request != null && request.WorkerId == User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                _dbcontext.Requests.Remove(request);
                _dbcontext.SaveChanges();
            }
            return RedirectToAction("Index");
        }


        public class RequestPagedResult
        {
            public IPagedList<Request> RequestList;
            public List<Ad> Ads;
            public List<User> Users;
        }
    }
}
