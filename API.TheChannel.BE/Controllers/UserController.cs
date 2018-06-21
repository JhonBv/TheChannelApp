using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using API.TheChannel.BE.Infrastructure;
using API.TheChannel.BE.Models;
using Microsoft.AspNet.Identity;

namespace API.TheChannel.BE.Controllers
{
    [RoutePrefix("api/accounts")]
    public class UserController : BaseApiController
    {
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
                                                   "<img src=\"" + System.Configuration.ConfigurationManager.AppSettings["BaseUrlAddress"] + "Content/images/header.png" + "\"><br/> Welcome to The Channel, in order to continue with your registration Please confirm your email address by clicking <a href=\"" + callbackUrl + "\">here</a>");

            //JB. Once confirmed, tell our app and update AspNet users table accordingly ;)
            Uri locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));

            AppUserManager.AddClaim(user.Id, new Claim(ClaimTypes.Uri, user.Email));
            //AppUserManager.AddToRole(user.Id, "User");


            //JB. Return generated UserId to client
            return Ok(locationHeader);
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
        public async Task<IHttpActionResult> ConfirmEmail(string userId = "", string code = "")
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError("", "User Id and Code are required");
                return BadRequest(ModelState);
            }

            IdentityResult result = await this.AppUserManager.ConfirmEmailAsync(userId, code);

            if (result.Succeeded)
            {
                //JB. Add to Users and Claims               

                //JB. return response;
                string mess = "email account have been successfully activated";

                return Ok(mess);
            }
            else
            {
                return GetErrorResult(result);
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
    }
}
