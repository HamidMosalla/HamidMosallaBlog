using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.Net.Http.Headers;
using VS2017Test.Models;

namespace VS2017Test.Controllers
{

    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Person()
        {
            var home = new HomeController();
            var type = home.JsonActionResult();
        }
    }

    [Produces("application/json")]
    public class HomeController : Controller
    {


        public IActionResult IndexWithId(int id)
        {
            return View();
        }

        public ActionResult IndexActionResult()
        {
            return View("Index");
        }

        public ViewResult Index()
        {
            return View();
        }

        public IActionResult JsonActionResult()
        {
            return NotFound();

            var data = new { Name = "Alex", LastName = "DeLarge" };

            return Json(data);
            //or new JsonResult(data);
        }

        public PartialViewResult PartialViewActionResult()
        {
            var model = new List<int> { 2, 3 };

            return PartialView("_PartialViewActionResult", model);
        }

        //writes the result of component to response, notice that you can directly call it in a controller
        //IViewComponentResult should be returned from a Class that inherent form ViewComponent
        public ViewComponentResult HomeSliderComponent()
        {
            return ViewComponent("HomeSlider", new { id = 4 });
        }

        //returns 200 with the content and specified media type for the content
        public ContentResult ContentActionResult()
        {
            return Content("{Name: 'Hamid'}, {Name: 'Stanley'}", new MediaTypeHeaderValue("application/json"));
        }

        //returns 200 OK which is empty
        public EmptyResult EmptyActionResult()
        {
            return new EmptyResult();
        }

        public XmlResult XmlActionResult()
        {
            var data = new List<Person> { new Person { FirstName = "Alex", LastName = "DeLarge" }, new Person { FirstName = "Major", LastName = "Boobage" } };

            return new XmlResult(data);
        }

        public Person PocoResult()
        {
            return new Person { FirstName = "Major", LastName = "Boobage" };
        }

        public List<Person> GetAllPersons()
        {
            return new List<Person> { new Person { FirstName = "Alex", LastName = "DeLarge" }, new Person { FirstName = "Major", LastName = "Boobage" } };
        }

        public int IntResult()
        {
            return 2;
        }

        public string StringResult()
        {
            return "Major Boobage?";
        }

        [NonAction]
        public Person YouShallNotPass()
        {
            return new Person { FirstName = "James", LastName = "Gandolfini" };
        }











        //sign in the user with its claim through returning SignInResult
        public SignInResult SignInActionResult()
        {
            const string Issuer = "https://gov.uk";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Andrew", ClaimValueTypes.String, Issuer),
                new Claim(ClaimTypes.Surname, "Lock", ClaimValueTypes.String, Issuer),
                new Claim(ClaimTypes.Country, "UK", ClaimValueTypes.String, Issuer),
                new Claim("ChildhoodHero", "Ronnie James Dio", ClaimValueTypes.String)
            };

            var userIdentity = new ClaimsIdentity(claims, "Passport");

            var userPrincipal = new ClaimsPrincipal(userIdentity);

            var authenticationProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                IsPersistent = false,
                AllowRefresh = false,
                RedirectUri = "/Home/Index"
            };

            return SignIn(userPrincipal, authenticationProperties, "Cookie");
        }

        //sign in the user with its claim through authentication manager
        public async Task SignInResultAsync()
        {
            const string Issuer = "https://gov.uk";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Andrew", ClaimValueTypes.String, Issuer),
                new Claim(ClaimTypes.Surname, "Lock", ClaimValueTypes.String, Issuer),
                new Claim(ClaimTypes.Country, "UK", ClaimValueTypes.String, Issuer),
                new Claim("ChildhoodHero", "Ronnie James Dio", ClaimValueTypes.String)
            };

            var userIdentity = new ClaimsIdentity(claims, "Passport");

            var userPrincipal = new ClaimsPrincipal(userIdentity);

            var authenticationProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                IsPersistent = false,
                AllowRefresh = false,
                RedirectUri = "/Home/Index"
            };

            await HttpContext.Authentication.SignInAsync("Cookie", userPrincipal, authenticationProperties);
        }

        //sign out the user with its claim through returning SignOutResult
        public SignOutResult SignOutActionResult()
        {
            var authenticationProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                IsPersistent = false,
                AllowRefresh = false,
                RedirectUri = "/Index"
            };

            return SignOut(authenticationProperties, "Cookie");
        }

        //sign out the user with its claim through returning authentication manager
        public async Task SignOutResultAsync()
        {
            var authenticationProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                IsPersistent = false,
                AllowRefresh = false,
                RedirectUri = "/Index"
            };

            await HttpContext.Authentication.SignOutAsync("Cookie", authenticationProperties);
        }

        //returns 403 Forbidden status code and redirect the user to the path specified when we setup AccessDeniedPath in cookie authentication
        //but do it through the AuthenticationManager Class
        //https://httpstatuses.com/403
        public async Task ForbidAsyncResult()
        {
            //var props = new AuthenticationProperties
            //{
            //    RedirectUri = "/Home/About"
            //};

            await HttpContext.Authentication.ForbidAsync();
        }

        //returns 403 Forbidden status code and redirect the user to the path specified when we setup AccessDeniedPath in cookie authentication
        public ForbidResult ForbidActionResult()
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = "/Home/About"
            };
            var something = new ForbidResult();
            //return Forbid();
            return Forbid(props);
        }

        //returns the 401 Unauthorized response and redirect the user to the path specified when we setup AccessDeniedPath in cookie authentication
        //but do it through the AuthenticationManager Class
        //https://httpstatuses.com/401
        public async Task ChallengeAsyncResult()
        {
            //var props = new AuthenticationProperties
            //{
            //    RedirectUri = "/Home/About"
            //};

            await HttpContext.Authentication.ChallengeAsync();
        }

        //returns the 401 Unauthorized response and redirect the user to the path specified when we setup AccessDeniedPath in cookie authentication
        public ChallengeResult ChallengeActionResult()
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = "/Home/About"
            };

            //return Challenge(props);
            return Challenge(props);
        }

        //returns a response with 401 response code
        public UnauthorizedResult UnauthorizedActionResult()
        {
            return Unauthorized();
        }











        //redirect to specified string URL with permanent 301 property set to false
        public RedirectResult RedirectActionResult()
        {
            //return Redirect("/");
            //return Redirect("http://localhost:12060/Home/Index");
            return new RedirectResult("/") {Permanent = true};
        }

        //redirect to specified string URL with permanent 301 property set to true
        public RedirectResult RedirectPermanentActionResult()
        {
            return RedirectPermanent("/");
        }

        //redirect to specified action with permanent 301 property set to false
        public RedirectToActionResult RedirectToActionActionResult()
        {
            return RedirectToAction("Index");
        }

        //redirect to specified action with permanent 301 property set to true
        public RedirectToActionResult RedirectToActionPermanentActionResult()
        {
            return RedirectToActionPermanent("Index");
        }

        //redirect to specified route by taking a route dictionary either as a type or as an anonymous type  with permanent 301 property set to false
        public RedirectToRouteResult RedirectToRouteActionResult()
        {
            var routeValue = new RouteValueDictionary(new { action = "Index", controller = "Home", area = "" });
            var routeValue2 = new { action = "Index", controller = "Home", area = "" };

            return RedirectToRoute(routeValue);
        }

        //redirect to specified route by taking a route dictionary either as a type or as an anonymous type with permanent 301 property set to true
        public RedirectToRouteResult RedirectToRoutePermanentActionResult()
        {
            var routeValue = new RouteValueDictionary(new { action = "Index", controller = "Home", area = "" });
            var routeValue2 = new { action = "Index", controller = "Home", area = "" };

            return RedirectToRoutePermanent(routeValue2);
        }

        //redirect to specified URL is it's local URL (also relative), if not it will throws an exception, permanent 301 property set to false
        public LocalRedirectResult LocalRedirectActionResult()
        {
            var IsHomeIndexLocal = Url.IsLocalUrl("/Home/Index");
            var isRootLocal = Url.IsLocalUrl("/");

            //throws InvalidOperationException: The supplied URL is not local. Url must be relative
            var isAbsoluteUrlLocal = Url.IsLocalUrl("http://localhost:12059/Home/Index");

            return LocalRedirect("/Home/Index");
        }

        //redirect to specified URL is it's local URL (also relative), if not it will throws an exception, permanent 301 property set to true
        public LocalRedirectResult LocalRedirectPermanentActionResult()
        {
            var IsHomeIndexLocal = Url.IsLocalUrl("/Home/Index");
            var isRootLocal = Url.IsLocalUrl("/");

            //throws InvalidOperationException: The supplied URL is not local. Url must be relative
            var isAbsoluteUrlLocal = Url.IsLocalUrl("http://localhost:12059/Home/Index");

            return LocalRedirectPermanent("/Home/Index");
        }










        //returns and empty 400 response
        public BadRequestResult BadRequestActionResult()
        {
            return BadRequest();
        }

        //returns 400 with an object containing error detail as object or as Model State Dictionary
        public BadRequestObjectResult BadRequestObjectActionResult()
        {
            var modelState = new ModelStateDictionary();
            modelState.AddModelError("Name", "Name is required.");
            return BadRequest(modelState);
        }

        //returns and empty 404 response
        public NotFoundResult NotFoundActionResult()
        {
            return NotFound();
        }

        //returns 404 with an object containing pertinent info
        public NotFoundObjectResult NotFoundObjectActionResult()
        {
            return NotFound(new { Id = 2, error = "There was no customer with an id of 2." });
        }

        //a response with an object but a null status code
        public ObjectResult ObjectActionResult()
        {
            return new ObjectResult(new { Name = "TomDickHarry" });
        }

        //200 with an object if formatting succed
        public OkObjectResult OkObjectActionResult()
        {
            return new OkObjectResult(new { Name = "TomDickHarry" });
        }

        //200 with an object if formatting succed
        public OkObjectResult OkWithObjectActionResult()
        {
            return Ok(new { Name = "TomDickHarry" });
        }

        // empty 200 without object and formatting
        public OkResult OkEmptyWithoutObject()
        {
            return Ok();
        }

        //returns 204 no content status code response
        //https://httpstatuses.com/204
        public NoContentResult NoContentActionResult()
        {
            return NoContent();
        }

        //returns a response with specified status code
        public StatusCodeResult StatusCodeActionResult()
        {
            return StatusCode(404);
        }

        //returns a response with specified status code along with an object
        public ObjectResult StatusCodeWithObject()
        {
            return StatusCode(404, new { Name = "TomDickHarry" });
        }

        //return 201 created status code along with the path of the created resource and the actual object
        public CreatedResult CreatedActionResult()
        {
            return Created(new Uri("/Home/Index", UriKind.Relative), new { Name = "Hamid" });
        }

        //return 201 created status code along with the controller, action, route values and the actual object that is created
        public CreatedAtActionResult CreatedAtActionActionResult()
        {
            return CreatedAtAction("IndexWithId", "Home", new { id = 2, area = "" }, new { Name = "Hamid" });
        }

        //return 201 created status code along with the route name, route value, and the actual object that is created
        public CreatedAtRouteResult CreatedAtRouteActionResult()
        {
            return CreatedAtRoute("default", new { Id = 2, area = "" }, new { Name = "Hamid" });
        }

        //return 202 accepted which means info in accepted for processing, and you can return a Uri for more info about processing and an object containing pertinent data
        public AcceptedResult AcceptedActionResult()
        {
            return Accepted(new Uri("/Home/Index", UriKind.Relative), new { Name = "Hamid" });
        }

        //return 202 accepted which means info in accepted for processing, and you can return controller and action name along with route values
        //for more info about processing and an object containing pertinent data
        public AcceptedAtActionResult AcceptedAtActionActionResult()
        {
            return AcceptedAtAction("IndexWithId", "Home", new { Id = 2, area = "" }, new { Name = "Hamid" });
        }

        //return 202 accepted which means info in accepted for processing, and you can return route name along with route values
        //for more info about processing and an object containing pertinent data
        public AcceptedAtRouteResult AcceptedAtRouteActionResult()
        {
            return AcceptedAtRoute("default", new { Id = 2, area = "" }, new { Name = "Hamid" });
        }

        //returns UnsupportedMediaType (415) response
        public UnsupportedMediaTypeResult UnsupportedMediaTypeActionResult()
        {
            return new UnsupportedMediaTypeResult();
        }









        //parent of the file related results, you can return any of the FileContentResult, FileStreamResult, VirtualFileResult, PhysicalFileResult to it
        public FileResult FileActionResult()
        {
            var file = System.IO.File.ReadAllBytes(@"C:\Users\User\Documents\Visual Studio 2017\Projects\VS2017Test\VS2017Test\Controllers\HomeController.cs");

            return File(file, "text/plain", "HomeController.cs");
        }

        //returns the file content as an array of bytes
        public FileContentResult FileContentActionResult()
        {
            var file = System.IO.File.ReadAllBytes(@"C:\Users\User\Documents\Visual Studio 2017\Projects\VS2017Test\VS2017Test\Controllers\HomeController.cs");

            return File(file, "text/plain", "HomeController.cs");
        }

        //return the file as a stream
        public FileStreamResult FileStreamActionResult()
        {
            //var file = System.IO.File.ReadAllBytes(@"C:\Users\User\Documents\Visual Studio 2017\Projects\VS2017Test\VS2017Test\Controllers\HomeController.cs");
            //var stream = new MemoryStream(file, writable:true);

            var fileStream = new FileStream(@"C:\Users\User\Documents\Visual Studio 2017\Projects\VS2017Test\VS2017Test\Controllers\HomeController.cs", FileMode.Open, FileAccess.Read);

            return File(fileStream, "text/plain", "HomeController.cs");
        }

        //returns a file specified with a virtual path
        public VirtualFileResult VirtualFileActionResult()
        {
            return File("/css/site.css", "text/plain", "site.css");
        }

        //returns the specified file on disk, that is it's physical address
        public PhysicalFileResult PhysicalFileActionResult()
        {
            return PhysicalFile(@"C:\Users\User\Documents\Visual Studio 2017\Projects\VS2017Test\VS2017Test\Controllers\HomeController.cs", "text/plain", "HomeController.cs");
        }









        //Rips
        //public JavaScriptResult JavaScriptResult() => null;
        //public FilePathResult FilePathResult() => null;
        //public HttpNotFoundResult HttpNotFoundResult() => null; => NotFoundResult
        //public HttpStatusCodeResult HttpStatusCodeResult() => null; => StatusCodeResult
        //public HttpUnauthorizedResult HttpUnauthorizedResult() => null; => UnauthorizedResult




        //epilogue research streams and FileStream MemoryStream StreamReader and StreamWriter and the like

        //not related to mvc
        //IAsyncResult
        //ViewEngineResult
        //ModelBindingResult
        //ValueProviderResult
    }
}
