using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using API.TheChannel.BE.Infrastructure;

namespace API.TheChannel.BE.Services
{
    public class ValidUserService
    {
        ApplicationDbContext Ctx = new ApplicationDbContext();

        public bool userIsActive(string id)
        {
            bool active = false;
            var daUser = Ctx.Users.FirstOrDefault(u => u.Id == id);
            active = daUser.UserActive;
            return active;
        }

        public bool checkEmail(string email)
        {
            bool yes;
            var mm = Ctx.Users.FirstOrDefault(e => e.Email == email)?.Email;
            yes = mm == email;
            return yes;
        }

        public bool checkUsername(string username)
        {
            bool yes;
            var mm = Ctx.Users.FirstOrDefault(e => e.UserName == username)?.UserName;
            yes = mm == username;
            return yes;
        }
    }
}