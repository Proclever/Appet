using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Appet.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Web.Security;

namespace Appet.Controllers
{
    public class HomeController : Controller
    {
        private PetModelsDBContext db = new PetModelsDBContext();
        private ApplicationDbContext dba = new ApplicationDbContext();

        public int[] Menu()
        {
            var userid = User.Identity.GetUserId();
            var menu = new int[db.UserPets.Where(x => x.user_id == userid && x.isaccepted == true).Count()];
            for(int i=0; i< db.UserPets.Where(x => x.user_id == userid && x.isaccepted == true).Count(); i++)
            {
                menu[i] = db.UserPets.Where(x => x.user_id == userid && x.isaccepted == true).ToList()[i].pet_id;
            }
            return menu;
        }

        public string[] MenuS()
        {
            var userid = User.Identity.GetUserId();
            var menus = new string[db.UserPets.Where(x => x.user_id == userid && x.isaccepted == true).Count()];
            for (int i = 0; i < db.UserPets.Where(x => x.user_id == userid && x.isaccepted == true).Count(); i++)
            {
                var menu = db.UserPets.Where(x => x.user_id == userid && x.isaccepted == true).ToList()[i].pet_id;
                menus[i] = db.Pets.Where(x => x.id == menu).First().name;
            }
            return menus;
        }

        [Authorize]
        public ActionResult Index()
        {
            ViewBag.Userid = User.Identity.GetUserId();
            ViewBag.Menu = Menu();
            ViewBag.MenuS = MenuS();
            return View(db.UserPets.ToList());
        }

        [Authorize]
        public ActionResult AcceptInv(int id)
        {
            var userid = User.Identity.GetUserId();
            if (db.UserPets.Where(x => x.id == id && x.user_id == userid).Count() == 0) return RedirectToAction("Index", "Home");

            UserPet userpet = db.UserPets.Where(x => x.id == id).First();
            userpet.isaccepted = true;
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult CancelInv(int id, int petid=0)
        {
            var userid = User.Identity.GetUserId();
            if (db.UserPets.Where(x => x.id == id && x.user_id == userid).Count() == 0) return RedirectToAction("Index", "Home");
            if (db.UserPets.Where(x => x.id == id && x.ismain == true).Count() == 1) return RedirectToAction("PetDetails", "Home", new { id = petid, message = "Aby opuścić pupila, musisz najpierw zmienić głównego właściciela." });

            UserPet userpet = db.UserPets.Where(x => x.id == id).First();
            db.UserPets.Remove(userpet);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult AddPet()
        {
            ViewBag.Menu = Menu();
            ViewBag.MenuS = MenuS();
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddPet(Pet pet)
        {
            var userid = User.Identity.GetUserId();
            var p = db.Pets.Add(new Pet { name = pet.name, img = pet.img });
            db.SaveChanges();
            var up = db.UserPets.Add(new UserPet { pet_id = p.id, user_id = userid, user_name = User.Identity.Name, user_email = dba.Users.Where(x => x.Id == userid).First().Email, ismain = true, isaccepted = true });
            db.SaveChanges();
            return RedirectToAction("PetDetails", "Home", new { id = p.id });
        }

        [Authorize]
        public ActionResult PetDetails(int id, string message = "")
        {
            var userid = User.Identity.GetUserId();
            if (db.UserPets.Where(x => x.pet_id == id && x.user_id == userid && x.isaccepted).Count() == 0) return RedirectToAction("Index", "Home");
            ViewBag.Menu = Menu();
            ViewBag.MenuS = MenuS();

            var walks = db.Walks.Where(x => x.pet_id == id).ToList();
            var walkstoremove = new List<Walk>();
            if (walks.Count() > 1)
            {
                for (int i = 1; i < walks.Count(); i++)
                {
                    var prev = walks.ElementAt(i-1).datetime;
                    if (prev.AddMinutes(30) > walks.ElementAt(i).datetime) walkstoremove.Add(walks.ElementAt(i - 1));
                }
                for (int i = 0; i < walkstoremove.Count(); i++)
                {
                    db.Walks.Remove(walkstoremove[i]);
                }
            }
            var eats = db.Eats.Where(x => x.pet_id == id).ToList();
            var eatstoremove = new List<Eat>();
            if (eats.Count() > 1)
            {
                for (int i = 1; i < eats.Count(); i++)
                {
                    var prev = eats.ElementAt(i - 1).datetime;
                    if (prev.AddMinutes(30) > eats.ElementAt(i).datetime) eatstoremove.Add(eats.ElementAt(i - 1));
                }
                for (int i = 0; i < eatstoremove.Count(); i++)
                {
                    db.Eats.Remove(eatstoremove[i]);
                }
            }
            db.SaveChanges();

            ViewBag.Message = message;
            ViewBag.Title = "Pupil " + db.Pets.Where(x => x.id == id).First().name;
            ViewBag.Userid = User.Identity.GetUserId();
            return View(db.Pets.Where(x => x.id == id).First());
        }

        [Authorize]
        public ActionResult RemoveOwner(int id)
        {
            var userid = User.Identity.GetUserId();
            if (db.UserPets.Where(x => x.id == id).First().Pet.UserPet.Where(x => x.user_id == userid && x.ismain == true).Count() == 0) return RedirectToAction("Index", "Home");

            var pet_id = db.UserPets.Where(x => x.id == id).First().pet_id;
            var up = db.UserPets.Where(x => x.id == id).First();
            db.UserPets.Remove(up);
            db.SaveChanges();
            return RedirectToAction("PetDetails", "Home", new { id = pet_id });
        }

        [Authorize]
        public ActionResult RenamePet(int id)
        {
            var userid = User.Identity.GetUserId();
            if (db.UserPets.Where(x => x.pet_id == id && x.user_id == userid && x.ismain).Count() == 0) return RedirectToAction("Index", "Home");
            ViewBag.Menu = Menu();
            ViewBag.MenuS = MenuS();

            return View(db.Pets.Where(x => x.id == id).First());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RenamePet([Bind(Include = "id, name, img")] Pet pet)
        {
            db.Entry(pet).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("PetDetails", "Home", new { id = pet.id });
        }

        [Authorize]
        public ActionResult RemovePet(int id)
        {
            var userid = User.Identity.GetUserId();
            if (db.UserPets.Where(x => x.pet_id == id && x.user_id == userid && x.ismain).Count() == 0) return RedirectToAction("Index", "Home");

            var up = db.UserPets.Where(x => x.pet_id == id);
            foreach(UserPet x in up) db.UserPets.Remove(x);
            db.SaveChanges();
            var p = db.Pets.Where(x => x.id == id).First();
            db.Pets.Remove(p);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult WalkNow(string action2, object route, int id)
        {
            var userid = User.Identity.GetUserId();
            if (db.UserPets.Where(x => x.pet_id == id && x.user_id == userid && x.isaccepted == true).Count() == 0) return RedirectToAction("Index", "Home");

            var w = db.Walks.Add(new Walk { pet_id = id, user_id = userid, user_name = User.Identity.Name, datetime = DateTime.Now });
            db.SaveChanges();
            return RedirectToAction(action2, "Home", new { id = id });
        }

        [Authorize]
        public ActionResult DelWalkNow(int id, int petid)
        {
            var userid = User.Identity.GetUserId();
            if (db.Walks.Where(x => x.id == id && x.user_id == userid).Count() == 0) return RedirectToAction("Index", "Home");
            if(db.Walks.Where(x => x.id == id && x.user_id == userid).First().datetime.AddMinutes(30)<DateTime.Now) return RedirectToAction("Index", "Home");

            var w = db.Walks.Where(x => x.id == id).First();
            db.Walks.Remove(w);
            db.SaveChanges();
            return RedirectToAction("PetDetails", "Home", new { id = petid });
        }

        [Authorize]
        public ActionResult EatNow(string action2, object route, int id)
        {
            var userid = User.Identity.GetUserId();
            if (db.UserPets.Where(x => x.pet_id == id && x.user_id == userid && x.isaccepted == true).Count() == 0) return RedirectToAction("Index", "Home");

            var e = db.Eats.Add(new Eat { pet_id = id, user_id = userid, user_name = User.Identity.Name,datetime = DateTime.Now });
            db.SaveChanges();
            return RedirectToAction(action2, "Home", new { id = id });
        }

        [Authorize]
        public ActionResult DelEatNow(int id, int petid)
        {
            var userid = User.Identity.GetUserId();
            if (db.Eats.Where(x => x.id == id && x.user_id == userid).Count() == 0) return RedirectToAction("Index", "Home");
            if (db.Eats.Where(x => x.id == id && x.user_id == userid).First().datetime.AddMinutes(30) < DateTime.Now) return RedirectToAction("Index", "Home");

            var e = db.Eats.Where(x => x.id == id).First();
            db.Eats.Remove(e);
            db.SaveChanges();
            return RedirectToAction("PetDetails", "Home", new { id = petid });
        }

        [Authorize]
        public ActionResult AddNote(int id)
        {
            var userid = User.Identity.GetUserId();
            if (db.UserPets.Where(x => x.pet_id == id && x.user_id == userid && x.isaccepted).Count() == 0) return RedirectToAction("Index", "Home");
            ViewBag.Menu = Menu();
            ViewBag.MenuS = MenuS();

            ViewBag.Pet_id = id;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddNote(Note note, int id)
        {
            var n = db.Notes.Add(new Note { pet_id = id, user_id = User.Identity.GetUserId(), user_name = User.Identity.Name, nnote = note.nnote, datetime = DateTime.Now });
            db.SaveChanges();
            return RedirectToAction("PetDetails", "Home", new { id = id });
        }

        [Authorize]
        public ActionResult DelNote(int id, int petid)
        {
            var userid = User.Identity.GetUserId();
            if (db.Notes.Where(x => x.id == id && x.user_id == userid).Count() == 0) return RedirectToAction("Index", "Home");
            if (db.Notes.Where(x => x.id == id && x.user_id == userid).First().datetime.AddMinutes(30) < DateTime.Now) return RedirectToAction("Index", "Home");

            var n = db.Notes.Where(x => x.id == id).First();
            db.Notes.Remove(n);
            db.SaveChanges();
            return RedirectToAction("PetDetails", "Home", new { id = petid });
        }

        [Authorize]
        public ActionResult GiveMain(int id, int owner_id)
        {
            var userid = User.Identity.GetUserId();
            if (db.UserPets.Where(x => x.pet_id == id && x.user_id == userid && x.ismain).Count() == 0) return RedirectToAction("Index", "Home");

            var up1 = db.UserPets.Where(x => x.pet_id == id && x.user_id == userid).First();
            up1.ismain = false;
            db.Entry(up1).State = EntityState.Modified;
            db.SaveChanges();
            var up2 = db.UserPets.Where(x => x.id == owner_id).First();
            up2.ismain = true;
            db.Entry(up2).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("PetDetails", "Home", new { id = id });
        }

        [Authorize]
        public ActionResult Invite(int id, string message = "Zaproś właściciela")
        {
            var userid = User.Identity.GetUserId();
            if (db.UserPets.Where(x => x.pet_id == id && x.user_id == userid && x.ismain == true).Count() == 0) return RedirectToAction("Index", "Home");
            ViewBag.Menu = Menu();
            ViewBag.MenuS = MenuS();

            ViewBag.Pet_id = id;
            ViewBag.Message = message;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Invite(UserPet userpet, int id)
        {
            var user_inv = dba.Users.Where(x => x.UserName == userpet.user_name || x.UserName == userpet.user_email);
            if (user_inv.Count()==1)
            {
                if(db.UserPets.Where(x => x.pet_id == id && x.isaccepted == true && (x.user_name == userpet.user_name || x.user_email == userpet.user_name)).Count() == 1)
                {
                    return RedirectToAction("Invite", "Home", new { id = id, message = "Użytkownik (" + userpet.user_name + ") jest już właścicielem Twojego pupila." });
                }
                else
                {
                    if(db.UserPets.Where(x => x.pet_id == id && x.isaccepted == false && (x.user_name == userpet.user_name || x.user_email == userpet.user_name)).Count() == 1)
                    {
                        return RedirectToAction("Invite", "Home", new { id = id, message = "Użytkownik (" + userpet.user_name + ") został już zaproszony." });
                    }
                    else
                    {
                        var up = db.UserPets.Add(new UserPet { pet_id = id, user_id = user_inv.First().Id, user_name = user_inv.First().UserName, user_email = user_inv.First().Email, ismain = false, isaccepted = false });
                        db.SaveChanges();
                        return RedirectToAction("PetDetails", "Home", new { id = id });
                    }
                }
            }
            else
            {
                return RedirectToAction("Invite", "Home", new { id = id, message = "Podany użytkownik (" + userpet.user_name + ") nie istnieje." });
            }
        }
    }
}