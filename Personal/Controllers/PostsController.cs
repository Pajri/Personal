using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Personal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Personal.Utils;
using Personal.ViewModels;

namespace Personal.Controllers
{
    public class PostsController : Controller
    {
        #region Properties
        private readonly PersonalContext _context;
        private UserManager<ApplicationUser> _userManager { get; set; }
        private IHostingEnvironment _env { get; set; }

        private const string VDATA_SEARCH = "search_keyword";
        #endregion

        #region Constructors
        public PostsController(PersonalContext context, UserManager<ApplicationUser> userManager, IHostingEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }
        #endregion

        #region Actions
        public async Task<IActionResult> Index(string search="")
        {
            //process search
            var queryPost = _QueryPosts(search).Take(10);            
            var postList = await queryPost.ToListAsync();

            ViewData[VDATA_SEARCH] = search; 
            return View(postList);
        }

        public async Task<IActionResult> MorePosts(int page, int pageSize, string search)
        {
            //process search
            var queryPost = _QueryPosts(search).Skip((page - 1) * pageSize).Take(pageSize);
            var postList = await queryPost.ToListAsync();
            ViewData[VDATA_SEARCH] = search;
            return PartialView("PartialPosts", postList);
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // TODO: Add Authorize
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Content)
        {
            PersonalPost personalPost = new PersonalPost();
            personalPost.Content = Content;
            if (ModelState.IsValid)
            {
                personalPost.Id = Guid.NewGuid();
                List<IFormFile> images = Request.Form.Files.Where(f => f.Name == "images[]").ToList();
                string[] imgNameList = _UploadImages(images, personalPost.Id.ToString());
                personalPost.ImageUrls = (imgNameList != null) ? string.Join(";", imgNameList) : null;
                personalPost.InsertDate = DateTime.Now;
                personalPost.LastUpdated = DateTime.Now;
                personalPost.UserId = _userManager.GetUserId(User);

                _context.Add(personalPost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(personalPost);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personalPost = await _context.PersonalPost.SingleOrDefaultAsync(m => m.Id == id);
            if (personalPost == null)
            {
                return NotFound();
            }
            PersonalPostViewModel personalPostViewModel = new PersonalPostViewModel
            {
                Id = personalPost.Id,
                Content = personalPost.Content,
                InsertDate = personalPost.InsertDate,
                LastUpdated = personalPost.LastUpdated,
                UserId = personalPost.UserId
            };

            if (personalPost.ImageUrls != null && personalPost.ImageUrls != "")
            {
                personalPostViewModel.StoredImageUrls = personalPost.ImageUrls.Split(";");

            }


            return View(personalPostViewModel);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Content,StoredImageUrls,NewImages,InsertDate,LastUpdated,UserId")] PersonalPostViewModel personalPostViewModel)
        {
            if (id != personalPostViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    PersonalPost personalPost = _context.PersonalPost.Where(p => p.Id == id).FirstOrDefault();
                    personalPost.Content = personalPostViewModel.Content;
                    personalPost.InsertDate = personalPostViewModel.InsertDate;
                    personalPost.LastUpdated = DateTime.Now;
                    personalPost.UserId = personalPostViewModel.UserId;

                    string existingImages = _context.PersonalPost.Where(p => p.Id == personalPost.Id).FirstOrDefault().ImageUrls;
                    //delete unselected images
                    if (existingImages != null && existingImages != "")
                    {
                        if (personalPostViewModel.StoredImageUrls == null)
                        {
                            _DeleteImages(personalPost.ImageUrls);
                        }
                        else
                        {
                            foreach (string image in personalPost.ImageUrls.Split(";"))
                            {
                                if (!personalPostViewModel.StoredImageUrls.Contains(image))
                                {
                                    _DeleteOneImage(image);
                                }
                            }
                        }

                    }

                    //upload image new images
                    int startNumber = 0;
                    if (personalPost.ImageUrls != null && personalPost.ImageUrls != "")
                    {
                        startNumber = personalPost.ImageUrls.Split(";").Count();
                    }

                    string[] newImages = _UploadImages(personalPostViewModel.NewImages, personalPost.Id.ToString(), startNumber + 1);
                    personalPost.ImageUrls = _CreateImageUrls(personalPostViewModel.StoredImageUrls, newImages);

                    _context.Update(personalPost);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_PersonalPostExists(personalPostViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(personalPostViewModel);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var personalPost = await _context.PersonalPost.SingleOrDefaultAsync(m => m.Id == id);
            _DeleteImages(personalPost.ImageUrls);
            _context.PersonalPost.Remove(personalPost);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion
       
        #region Private Methods
        private string[] _UploadImages(List<IFormFile> images, string baseFileName, int startNumber = 0)
        {
            if (images == null) return null;
            string username = _userManager.GetUserName(User);
            string imageUserDir = _env.ContentRootPath + PersonalConstants.USERDATA_IMAGE + "\\" + username;
            if (!Directory.Exists(imageUserDir))
            {
                Directory.CreateDirectory(imageUserDir);
            }

            //these codes were taken from https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-2.2
            string[] imgFileNames = new string[images.Count()];
            int i = (startNumber == 0) ? 1 : startNumber;
            int idx = 0;
            foreach (IFormFile formImgs in images)
            {
                if (formImgs.Length > 0)
                {
                    string fileName = baseFileName + "-" + (i++) + Path.GetExtension(formImgs.FileName);
                    string completePath = imageUserDir + "\\" + fileName;
                    using (var stream = new FileStream(completePath, FileMode.Create))
                    {
                        formImgs.CopyTo(stream);

                        string imageUrl = PersonalConstants.POST_IMAGE_URL + "/" + username + "/" + fileName;
                        imgFileNames[idx++] = imageUrl;
                    }
                }
            }
            return imgFileNames;
        }
        private void _DeleteImages(string images)
        {
            string[] arrImgs = images.Split(";");
            string username = _userManager.GetUserName(User);
            foreach (string imgUrl in arrImgs)
            {
                _DeleteOneImage(imgUrl);
            }
        }

        //in db, image stored as image url
        private void _DeleteOneImage(string imgUrl)
        {
            string username = _userManager.GetUserName(User);
            string imgName = imgUrl.Split("/").Last();
            if (imgName != null && imgName != "")
            {
                string imgPath = _env.ContentRootPath + "\\" + PersonalConstants.USERDATA_IMAGE + "\\" + username + "\\" + imgName;
                System.IO.File.Delete(imgPath);
            }
        }

        private string _CreateImageUrls(string[] storedImageUrls, string[] newImageUrls)
        {
            string imageUrls = "";
            if (storedImageUrls != null && storedImageUrls.Length > 0)
            {
                imageUrls = string.Join(";", storedImageUrls);
            }

            if (newImageUrls != null && newImageUrls.Length > 0)
            {
                imageUrls += ";" + string.Join(";", newImageUrls);
                imageUrls = imageUrls.TrimStart(';');
            }

            return imageUrls;
        }

        private IQueryable<PersonalPost> _QueryPosts(string search="")
        {
            if (search == null) search = "";
            return _GetPostForCurrentUser().OrderByDescending(p => p.InsertDate)
                .Where(p => p.Content.ToLower().Contains(search.Trim().ToLower()));
        }
        private IQueryable<PersonalPost> _GetPostForCurrentUser()
        {
            var queryPost = _context.PersonalPost
                .Where(p => (p.UserId == _userManager.GetUserId(User)));
            return queryPost;
        }

        private bool _PersonalPostExists(Guid id)
        {
            return _context.PersonalPost.Any(e => e.Id == id);
        }
        #endregion


    }
}
