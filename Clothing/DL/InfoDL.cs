using Clothing.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Clothing.DL
{
    public class InfoDL : Controller
    {
        // GET: InfoDL
        DatabaseEntities db;
        #region Info
        public List<Info> GetActiveInfoesList(DatabaseEntities de = null)
        {
            List<Info> Infoes;

            if (de != null)
            {
                Infoes = de.Infoes.Where(x => x.IsActive == 1).ToList();
                //Infoes = de.ShowAllInfo().ToList();
            }
            else
            {
                DatabaseEntities db = new DatabaseEntities();
                Infoes = db.Infoes.Where(x => x.IsActive == 1).ToList();
                //Infoes = db.ShowAllInfo().ToList();
            }
            return Infoes;
        }

        public List<Info> GetInactiveInfoes()
        {
            List<Info> Info;
            DatabaseEntities db = new DatabaseEntities();
            Info = db.Infoes.Where(x => x.IsActive == 0).ToList();
            return Info;
        }

        public Info getInfoById(int _Id, DatabaseEntities de = null)
        {
            DatabaseEntities db = new DatabaseEntities();
            if (de != null)
            {
                Info Infoes = de.Infoes.Where(x => x.IsActive == 1).FirstOrDefault(x => x.Id == _Id);
                //Info Infoes = de.GetInfoById(_Id).FirstOrDefault();
                return Infoes;
            }
            Info Info = db.Infoes.Where(x => x.IsActive == 1).FirstOrDefault(x => x.Id == _Id);
            //Info Info = de.GetInfoById(_Id).FirstOrDefault();

            return Info;
        }

        public bool AddInfo(Info _Info)
        {
            using (db = new DatabaseEntities())
            {
                db.Infoes.Add(_Info);
                //db.AddInfo(_Info.DatePurchased, _Info.PurchasePrice, _Info.NotesII, _Info.QRbarcode, _Info.User_Id, _Info.LocationId, _Info.ItemId);
                db.SaveChanges();
            }
            return true;
        }

        public bool UpdateInfo(Info _Info, DatabaseEntities de = null)
        {
            try
            {
                if (de != null)
                {
                    de.Entry(_Info).State = System.Data.Entity.EntityState.Modified;
                    //de.UpdateInfo(_Info.Id, _Info.DatePurchased, _Info.PurchasePrice, _Info.NotesII, _Info.QRbarcode, _Info.User_Id, _Info.LocationId, _Info.ItemId, _Info.IsActive);
                    de.SaveChanges();
                    return true;
                }
                using (db = new DatabaseEntities())
                {
                    db.Entry(_Info).State = System.Data.Entity.EntityState.Modified;
                    //db.UpdateInfo(_Info.Id, _Info.DatePurchased, _Info.PurchasePrice, _Info.NotesII, _Info.QRbarcode, _Info.User_Id, _Info.LocationId, _Info.ItemId, _Info.IsActive);
                    db.SaveChanges();
                }
                return true;
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }

        }

        #endregion
    }
}