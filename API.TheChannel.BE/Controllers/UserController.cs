using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;

using System.Web.Http;

using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using API.TheChannel.BE.Infrastructure;
using API.TheChannel.BE.Models;
using API.TheChannel.BE.Services;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace API.TheChannel.BE.Controllers
{
    [RoutePrefix("api/accounts")]
    public class UserController : BaseApiController
    {
        private ValidUserService _checkUser = new ValidUserService();

        [AllowAnonymous]
        [Route("Register")]
        // POST api/Account/Register
        /// <summary>
        /// JB. Asynhronous task to Register a user by providing Emaiil address ad password. (User must enter password twice to confirm)
        /// </summary>
        /// <param name="model">RegisterBindingModel</param>
        /// <returns></returns>
        public async Task<IHttpActionResult> Register(CreateUserBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //JB. if user has not send username, then set the Email as username.
            var user = new ApplicationUser() { UserName = model.Username = String.IsNullOrEmpty(model.Username) ? model.Email : model.Username, Email = model.Email };
            IdentityResult result = await AppUserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }



            string code = await this.AppUserManager.GenerateEmailConfirmationTokenAsync(user.Id);

                     //JB.Build Email callback where user will Confirm Email (When the user is interacting without a Wrapper).
            var callbackUrl = new Uri(Url.Link("ConfirmEmailRoute", new { userId = user.Id, code = code, email = user.Email }));

            await this.AppUserManager.SendEmailAsync(user.Id,
                                                    "Please Confirm your Email",
                                                   "<img src=\"" + System.Configuration.ConfigurationManager.AppSettings["BaseUrlAddress"] + "Content/Images/header.png" + "\"><br/> Welcome to The Channel, in order to continue with your registration Please confirm your email address by clicking <a href=\"" + callbackUrl + "\">here</a>");

            //JB. Once confirmed, tell our app and update AspNet users table accordingly ;)
            Uri locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));

            AppUserManager.AddClaim(user.Id, new Claim(ClaimTypes.Uri, user.Email));
            //AppUserManager.AddToRole(user.Id, "User");
            var dis = new Dictionary<string,Uri>();
            dis.Add("userUrl", locationHeader);

            var daReturnedUSer = JsonConvert.SerializeObject(dis);
            //JB. Return generated UserId to client
            return Ok(daReturnedUSer);
        }


        [AllowAnonymous]
        [HttpGet]
        /// <summary>
        /// End point returning the UserCode after user activaytes user email address.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [Route("ConfirmEmail", Name = "ConfirmEmailRoute")]
        public async Task<HttpResponseMessage> ConfirmEmail(string userId = "", string code = "")
        {
            var response = Request.CreateResponse(HttpStatusCode.Moved);
            var badUriResponse = ConfigurationManager.AppSettings["BaseUrlAddress"]+ "Home/BadRequest";
            var goodUriResponse = ConfigurationManager.AppSettings["BaseUrlAddress"] + "Home/SuccessRegistration";

            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError("", "User Id and Code are required");
                response.Headers.Location = new Uri(badUriResponse);
                return response;
            }

            IdentityResult result = await this.AppUserManager.ConfirmEmailAsync(userId, code);

            if (result.Succeeded)
            {
                response.Headers.Location = new Uri(goodUriResponse);

                return response;
            }
            else
            {
                response.Headers.Location = new Uri(badUriResponse);
                return response;
            }
        }



        [Route("users")]
        public IHttpActionResult GetUsers()
        {
            return Ok(this.AppUserManager.Users.ToList().Select(u => this.TheModelFactory.Create(u)));
        }

        [Route("user/{id:guid}", Name = "GetUserById")]
        public async Task<IHttpActionResult> GetUser(string Id)
        {
            var user = await this.AppUserManager.FindByIdAsync(Id);

            if (user != null)
            {
                return Ok(this.TheModelFactory.Create(user));
            }

            return NotFound();

        }

        [Route("user/{username}")]
        public async Task<IHttpActionResult> GetUserByName(string username)
        {
            var user = await this.AppUserManager.FindByNameAsync(username);

            if (user != null)
            {
                return Ok(this.TheModelFactory.Create(user));
            }

            return NotFound();

        }
        [HttpGet]
        [Route("checkEmail")]
        public  bool EmailExist(string email)
        {
            //bool fHasSpace = email.Contains(" ");
            var damail = Regex.Replace(email, @"\s+", "+");
            return _checkUser.checkEmail(damail);

        }

        [HttpGet]
        [Route("checkUsername")]
        public bool UsernameExist(string username)
        {
            
            return _checkUser.checkUsername(username);

        }
    }
}
