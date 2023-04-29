using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CountryhouseService.Models;
using CountryhouseService.ViewModels;
using CountryhouseService.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using X.PagedList;
using System.ComponentModel.DataAnnotations;

namespace CountryhouseService.Controllers
{    
    public class AdController : Controller
    {
        public static readonly List<string> ImageExtensions = new List<string> { ".JPG", ".JPEG", ".JPE", ".BMP", ".GIF", ".PNG" };
        private readonly ApplicationDbContext _dbcontext;
        private readonly SignInManager<User> _signinmanager;
        public AdController(ApplicationDbContext dbcontext, SignInManager<User> signInManager)
        {
            _dbcontext = dbcontext;
            _signinmanager = signInManager;
        }

        public async Task<IActionResult> Index(string sortOrder, string searchString, int? page)
        {

            ViewBag.CurrentFilter = searchString;
            IEnumerable<Ad> adenum = _dbcontext.Ads;
            if (!String.IsNullOrEmpty(searchString))
            {
                adenum = adenum.Where(s => s.Title.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "newest":
                    adenum = adenum.OrderBy(s => s.created_at);
                    break;
                case "oldest":
                    adenum = adenum.OrderByDescending(s => s.created_at);
                    break;
                case "payment_asc":
                    adenum = adenum.OrderBy(s => s.Payment);
                    break;
                case "payment_desc":
                    adenum = adenum.OrderByDescending(s => s.Payment);
                    break;
                case "status":
                    var PublishedAds = adenum.Where(s => s.Status == "Published");
                    var NotPublished = adenum.Where(s => s.Status != "Published");
                    NotPublished.OrderBy(s => s.Status);
                    adenum = PublishedAds;
                    adenum.Union(NotPublished);
                    break;
                default:
                    adenum = adenum.OrderBy(s => s.created_at);
                    break;
            }
            int pageSize = 5;
            int pageNumber = page ?? 1;
            IPagedList<Ad> adList = adenum.ToPagedList(pageNumber, pageSize);
            AdsListPagedResult result = new AdsListPagedResult
            {
                AdList = adList,
                CurrentSortOrder = sortOrder,
                CurrentSearchString = searchString, 
            };
            return View(result);
        }

        [HttpGet("Ad/{id:int}")]
        public async Task<IActionResult> Ad(int? id)
        {
            if (id > 0)
            {
                Ad? ad = _dbcontext.Ads.Find(id);

                if (ad != null)
                {
                    ViewBag.currentuserid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    User? creator = await _dbcontext.Users.FindAsync(ad.CreatorId);
                    ViewBag.creatorid = creator.Id;
                    IQueryable<int> images = from image in _dbcontext.Images where image.Ad == ad select image.ImageId;
                    List<int>? idlist = images.ToList();
                    string? userrole = null;
                    if (_signinmanager.IsSignedIn(User))
                    {
                        userrole = User.IsInRole("Owner") ? "Owner" : "Worker";
                    }
                    AdPagedResult result = new AdPagedResult
                    {
                        Id = id,
                        Title = ad.Title,
                        Status = ad.Status,
                        Description = ad.Description,
                        Adress = ad.Address,
                        ContactNumber = ad.ContactNumber,
                        Creatorname = $"{creator.FirstName} {creator.LastName}",
                        CreatorIsCurrentUser = (User.FindFirstValue(ClaimTypes.NameIdentifier) == ad.CreatorId),
                        UserRole = (userrole == null) ? null : userrole,
                        IdList = idlist
                    };
                    return View(result);
                }
            }
            return Redirect("Index");
        }

        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Complete(int? id)
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Ad? ad = _dbcontext.Ads.Find(id);
            if(ad != null && ad.CreatorId == currentUserId && ad.Status == "Accepted")
            {
                Request request = _dbcontext.Requests.First(s => s.AdId == ad.AdId && s.Status == "Accepted");
                if (request != null)
                {
                    User worker = _dbcontext.Users.Find(request.WorkerId);
                    ad.Status = $"Completed by {worker.FirstName} {worker.LastName}";
                    var requests = _dbcontext.Requests.Where(s => s.AdId == ad.AdId);
                    _dbcontext.Requests.RemoveRange(requests);
                    await _dbcontext.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Owner")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Create(CreateAdModel createAdModel)
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid)
            {

                List<Image> images = new List<Image>();
                if (createAdModel.Images != null)
                {
                    foreach (var formimage in createAdModel.Images)
                    {
                        Stream str = formimage.OpenReadStream();
                        byte[] tmp = new byte[str.Length];
                        str.Read(tmp);
                        images.Add(new Image { file = tmp });
                    }
                    await _dbcontext.Images.AddRangeAsync(images);
                }

                int newAdId = await CreateAsync(createAdModel, images, currentUserId);

                ViewData["AdId"] = newAdId;
                ModelState.Clear();
                return Redirect($"{newAdId}");
            }
            return View(createAdModel);
        }


        [HttpGet]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id > 0)
            {
                Ad? ad = _dbcontext.Ads.Find(id);
                if (ad != null && ad.CreatorId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    EditAdModel result = new EditAdModel
                    {
                        Id = ad.AdId,
                        Title = ad.Title,
                        Description = ad.Description,
                        Address = ad.Address,
                        Payment = ad.Payment,
                        ContactNumber = ad.ContactNumber,
                        FromDate = ad.FromDate,
                        UntilDate = ad.UntilDate,
                    };
                    return View(result);
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Edit(EditAdModel AdModel)
        {
            if (ModelState.IsValid)
            {
                if (AdModel.Id > 0)
                {
                    Ad ad = _dbcontext.Ads.Find(AdModel.Id);
                    if (ad != null && ad.CreatorId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                    {
                        IQueryable<Image> removingimages = from image in _dbcontext.Images where image.Ad == ad select image;
                        _dbcontext.Images.RemoveRange(removingimages);

                        List<Image> images = new List<Image>();
                        if (AdModel.Images != null)
                        {
                            foreach (var formImage in AdModel.Images)
                            {
                                Stream str = formImage.OpenReadStream();
                                byte[] tmp = new byte[str.Length];
                                str.Read(tmp);
                                images.Add(new Image { file = tmp });
                            }
                            await _dbcontext.Images.AddRangeAsync(images);
                        }

                        UpdateAd(ad, AdModel, images);

                        return RedirectToAction(ad.AdId.ToString());
                    }
                }
                return RedirectToAction("Index");
            }
            return View(AdModel);
        }

        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null && id > 0)
            {
                Ad? ad = await _dbcontext.Ads.FindAsync(id);
                if (ad != null && ad.CreatorId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                {
                    if (ad.Images != null)
                    {
                        _dbcontext.RemoveRange(ad.Images);
                    }
                    var requests = _dbcontext.Requests.Where(s => s.AdId == ad.AdId);
                    _dbcontext.Requests.RemoveRange(requests);
                    _dbcontext.Ads.Remove(ad);
                    _dbcontext.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }



        public class AdPagedResult
        {
            public int? Id;
            public string Title;
            public string Description;
            public string Adress;
            public string ContactNumber;
            public List<int>? IdList;
            public string Creatorname;
            public bool CreatorIsCurrentUser;
            public string? UserRole;
            public string? Status;

        }


        public class AdsListPagedResult
        {
            public IPagedList<Ad> AdList;
            public string CurrentSortOrder;
            public string CurrentSearchString;
        }

        public async Task<int> CreateAsync(Ad ad)
        {
            await _dbcontext.Ads.AddAsync(ad);
            await _dbcontext.SaveChangesAsync();
            return ad.AdId;
        }
        public async Task<int> CreateAsync(CreateAdModel CreateAdModel, List<Image>? images, string currentUserId)
        {

            Ad newAd = new Ad
            {
                Title = CreateAdModel.Title,
                Description = CreateAdModel.Description,
                Address = CreateAdModel.Address,
                Payment = CreateAdModel.Payment,
                ContactNumber = CreateAdModel.ContactNumber,
                Images = images,
                UpdatedOn = DateTime.UtcNow,
                CreatorId = currentUserId,
                Status = "Published",
                FromDate = CreateAdModel.FromDate,
                UntilDate = CreateAdModel.UntilDate,
            };
            return await CreateAsync(newAd);
        }
        public void UpdateAd(Ad ad, EditAdModel EditAdModel, List<Image>? images)
        {
            ad.Title = EditAdModel.Title;
            ad.Description = EditAdModel.Description;
            ad.Address = EditAdModel.Address;
            ad.Payment = EditAdModel.Payment;
            ad.ContactNumber = EditAdModel.ContactNumber;
            ad.Images = images;
            ad.UpdatedOn = DateTime.UtcNow;
            ad.FromDate = EditAdModel.FromDate;
            ad.UntilDate = EditAdModel.UntilDate;
            _dbcontext.Ads.Update(ad);
            _dbcontext.SaveChanges();
        }
    }

}