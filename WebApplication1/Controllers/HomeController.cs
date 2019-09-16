using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApplication1.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetSavedRepositoryIds()
        {
            var allRepoString = HttpContext.Session.GetString("allRepo");
            var list = new List<int>();
            if (allRepoString != null)
            {
                List<githubRepositoryModel> allRepoSaved = JsonConvert.DeserializeObject<List<githubRepositoryModel>>(allRepoString);
                foreach(githubRepositoryModel repo in allRepoSaved)
                {
                    list.Add(repo.Id);
                }
            }
            return Json(list);
          
        }
        public IActionResult ShowSavedGithubRepository()
        {
            int amountGithubRepositorySaved = 0;
            var allRepoString = HttpContext.Session.GetString("allRepo");
            string strAmountGithubRepositorySaved = HttpContext.Session.GetString("amountSavedRepository");

            if(!(strAmountGithubRepositorySaved == null))
            {
                amountGithubRepositorySaved = int.Parse(strAmountGithubRepositorySaved);
            }

            if (amountGithubRepositorySaved == 0)
            {
                ViewBag.errorMessage = "You don't have any repository saved";
            }
            else
            {
                List<githubRepositoryModel> allRepoSaved = JsonConvert.DeserializeObject<List<githubRepositoryModel>>(allRepoString);
                ViewBag.allRepositoriesSaved = allRepoSaved;
            }

            ViewBag.amountRepositorySaved = amountGithubRepositorySaved;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult SaveFavoriteRepository(string repo)
        {
            var allRepoString = HttpContext.Session.GetString("allRepo");
            var list = new List<githubRepositoryModel>();
            githubRepositoryModel curentRepository = JsonConvert.DeserializeObject<githubRepositoryModel>(repo);
            var checkedIfCurrentRepositoryExist = false; 
            var test = HttpContext.Session.GetString("test");

            if (allRepoString == null)
            {
                HttpContext.Session.SetString("amountSavedRepository", "1");
            }
            else
            {
                List<githubRepositoryModel> allRepositorySaved = JsonConvert.DeserializeObject<List<githubRepositoryModel>>(allRepoString);
                foreach (githubRepositoryModel savedRepo in allRepositorySaved)
                {
                    if(curentRepository.Id == savedRepo.Id)
                    {
                        checkedIfCurrentRepositoryExist = true;
                    }
                    else // if is exist we deleted it
                    {
                        list.Add(savedRepo);

                    }
                }
            }

            if(checkedIfCurrentRepositoryExist == false)
            {
                list.Add(curentRepository);
            }
          
            // update the amount od saved repository and the all the saved repository
            HttpContext.Session.SetString("amountSavedRepository", list.Count.ToString());
            HttpContext.Session.SetString("allRepo", JsonConvert.SerializeObject(list));

            return View();
        }
    }
}
