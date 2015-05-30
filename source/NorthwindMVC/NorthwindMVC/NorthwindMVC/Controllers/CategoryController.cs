using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fun.BAL;
using Fun.Helper;
using Northwind.Models;

namespace NorthwindMVC.Controllers
{
    public class CategoryController : Controller
    {
        int PageSize { get; set; }
        public CategoryController()
        {
            PageSize = 5;
        }
        //
        // GET: /Category/
        public ActionResult Index(string sortField, string sortOrder, int pageNumber = 1)
        {
            CategoryBAL categoryBAL = new CategoryBAL();
            sortField = (string.IsNullOrEmpty(sortField)) ? "CategoryID" : sortField;
            sortOrder = ((string.IsNullOrEmpty(sortOrder)) || (sortOrder == "asc")) ? "desc" : "asc";
            ViewBag.sortField = sortField;
            ViewBag.sortOrder = sortOrder;            
            int recordCount = 0;
            List<Category> categoryList = categoryBAL.Select(sortField, sortOrder, pageNumber, this.PageSize, ref recordCount);            
            //回傳viewModel
            CategoryViewModel viewModel = new CategoryViewModel();
            viewModel.CategoryList = categoryList;
            viewModel.Pager = new Pager()
            {
                PageSize = this.PageSize,
                PageNumber = pageNumber,
                RecordCount = recordCount,
                SortField = sortField,
                SortOrder = sortOrder
            };
            viewModel.Message = string.Format("{0}",TempData["message"]);
            return View(viewModel);
        }

        // GET: /Category/Detail/{CategoryID}
        //[Route("Category/Detail/{CategoryID}")]
        public ActionResult Detail(int id)
        {
            CategoryBAL categoryBAL = new CategoryBAL();
            Category category = categoryBAL.Select(id);
            CategoryViewModel viewModel = new CategoryViewModel();
            viewModel.Category = category;
            return View(viewModel);
        }


        // GET: /Category/Create
        //[Route("Category/Create")]
        [HttpGet]
        public ActionResult Create()
        {
            CategoryViewModel viewModel = new CategoryViewModel();            
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(Category category)
        {
            CategoryViewModel viewModel = new CategoryViewModel();
            if (ModelState.IsValid)
            {
                CategoryBAL categoryBAL = new CategoryBAL();
                bool result = categoryBAL.Insert(category);
                if (result)
                {
                    TempData["message"] = "新增成功!";
                    viewModel.Category = category;
                    return RedirectToAction("Index");
                }
                else
                {

                    viewModel.Message = string.Format("新增失敗!{0}", categoryBAL.LastError);
                    viewModel.Category = category;
                    return View(viewModel);
                }
            }

            return View(viewModel);

        }


        // GET: /Category/Detail/{CategoryID}
        //[Route("Category/Edit/{CategoryID}")]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            CategoryBAL categoryBAL = new CategoryBAL();
            Category category = categoryBAL.Select(id);
            CategoryViewModel viewModel = new CategoryViewModel();
            viewModel.Category = category;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(Category category)
        {
            CategoryViewModel viewModel = new CategoryViewModel();
            viewModel.Category = category;
            if (ModelState.IsValid)
            {
                CategoryBAL categoryBAL = new CategoryBAL();
                bool result = categoryBAL.Update(category);
                TempData["message"]= result ? string.Format("修改成功!CategoryID:{0}", category.CategoryID) : string.Format("修改失敗!CategoryID:{0},error={1}", category.CategoryID, categoryBAL.LastError);
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }


        // GET: /Category/Delete/{CategoryID}
        //[Route("Category/Delete/{CategoryID}")]
        [HttpGet]
        public ActionResult Delete(int id)
        {
            CategoryBAL categoryBAL = new CategoryBAL();
            bool result = categoryBAL.Delete(id);
            CategoryViewModel viewModel = new CategoryViewModel();
            if (result)
            {
                TempData["message"] = string.Format("刪除成功!CategoryID={0}", id);
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = string.Format("刪除失敗!CategoryID={0},error={1}", id, categoryBAL.LastError);
                return RedirectToAction("Index");
            }
        }
    }
}
