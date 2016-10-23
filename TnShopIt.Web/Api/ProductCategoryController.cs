using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using AutoMapper;
using TnShopIt.Model.Models;
using TnShopIt.Service;
using TnShopIt.Web.Infrastructure.Core;
using TnShopIt.Web.Infrastructure.Extensions;
using TnShopIt.Web.Models;

namespace TnShopIt.Web.Api
{
    [RoutePrefix("api/productcategory")]
    public class ProductCategoryController : ApiControllerBase
    {
        private IProductCategoryService _productCategoryService;

        public ProductCategoryController(IErrorService errorService, IProductCategoryService productCategoryService)
            : base(errorService)
        {
            this._productCategoryService = productCategoryService;
        }

        [Route("create")]
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage Create(HttpRequestMessage request, ProductCategoryViewModel productCategoryVm)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage respone = null;
                if(!ModelState.IsValid)
                {
                    respone = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    var newProductCategory = new ProductCategory();
                    newProductCategory.UpdateProductCategory(productCategoryVm);
                    newProductCategory.CreatedDate = DateTime.Now;
                    _productCategoryService.Add(newProductCategory);
                    _productCategoryService.Save();

                    var responeData = Mapper.Map<ProductCategory, ProductCategoryViewModel>(newProductCategory);
                    respone = request.CreateResponse(HttpStatusCode.Created, responeData);
                }
                return respone;
            });
        }

        [Route("update")]
        [HttpPut]
        [AllowAnonymous]
        public HttpResponseMessage Update(HttpRequestMessage request, ProductCategoryViewModel productCategoryVm)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage respone = null;
                if (!ModelState.IsValid)
                {
                    respone = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    var dbProductCategory = _productCategoryService.GetById(productCategoryVm.ID);
                    dbProductCategory.UpdateProductCategory(productCategoryVm);
                    dbProductCategory.UpdatedDate = DateTime.Now;
                    _productCategoryService.Update(dbProductCategory);
                    _productCategoryService.Save();

                    var responeData = Mapper.Map<ProductCategory, ProductCategoryViewModel>(dbProductCategory);
                    respone = request.CreateResponse(HttpStatusCode.Created, responeData);
                }
                return respone;
            });
        }

        [Route("delete")]
        [HttpDelete]
        [AllowAnonymous]
        public HttpResponseMessage Delete(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage respone = null;
                if (!ModelState.IsValid)
                {
                    respone = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    var oldProductCategory = _productCategoryService.Delete(id);
                    _productCategoryService.Save();

                    var responeData = Mapper.Map<ProductCategory, ProductCategoryViewModel>(oldProductCategory);
                    respone = request.CreateResponse(HttpStatusCode.Created, responeData);
                }
                return respone;
            });
        }

        [Route("deletemulti")]
        [HttpDelete]
        [AllowAnonymous]
        public HttpResponseMessage DeleteMulti(HttpRequestMessage request, string  strId)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage respone = null;
                if (!ModelState.IsValid)
                {
                    respone = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    var listProductCategory = new JavaScriptSerializer().Deserialize<List<int>>(strId);
                    foreach(var item in listProductCategory)
                    {
                        _productCategoryService.Delete(item);
                    }
                    _productCategoryService.Save();
                    respone = request.CreateResponse(HttpStatusCode.OK, listProductCategory.Count);
                }
                return respone;
            });
        }

        [HttpGet]
        [Route("getbyid/{id:int}")]
        public HttpResponseMessage GetById(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                var model = _productCategoryService.GetById(id);

                var responeData = Mapper.Map<ProductCategory, ProductCategoryViewModel>(model);

                var respone = request.CreateResponse(HttpStatusCode.OK, responeData);
                return respone;
            });
        }

        [HttpGet]
        [Route("getallparents")]
        public HttpResponseMessage GetAll(HttpRequestMessage request)
        {
            return CreateHttpResponse(request, () =>
            {
                var model = _productCategoryService.GetAll();

                var responeData = Mapper.Map<IEnumerable<ProductCategory>, IEnumerable<ProductCategoryViewModel>>(model);

                var respone = request.CreateResponse(HttpStatusCode.OK, responeData);
                return respone;
            });
        }

        [HttpGet]
        [Route("getall")]
        public HttpResponseMessage GetAll(HttpRequestMessage request,string keyword, int page, int pageSize = 20)
        {
            return CreateHttpResponse(request, () =>
            {
                int totalRow = 0;
                var model = _productCategoryService.GetAll(keyword);
                totalRow = model.Count();
                var query = model.OrderByDescending(x => x.CreatedDate).Skip(page * pageSize).Take(pageSize);

                var responeData = Mapper.Map<IEnumerable<ProductCategory>, IEnumerable<ProductCategoryViewModel>>(query);

                var paginationSet = new PaginationSet<ProductCategoryViewModel>()
                {
                    Items = responeData,
                    Pages = page,
                    TotalCount = totalRow,
                    TotalPages = (int)Math.Ceiling((decimal)totalRow / pageSize)
                };

                var respone = request.CreateResponse(HttpStatusCode.OK, paginationSet);
                return respone;
            });
        }
    }
}