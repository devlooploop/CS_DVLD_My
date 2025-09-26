using System;
using System.Data;
using DataAccess;

namespace Business
{
    public class clsPerson
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int ID { set; get; }
        public string NationalNo { set; get; }
        public string FirstName { set; get; }
        public string SecondName { set; get; }
        public string ThirdName { set; get; }
        public string LastName { set; get; }
        public DateTime DateOfBirth { set; get; }
        public short  Gender { set; get; }
        public string Address { set; get; }
        public string Phone { set; get; } 
        public string Email { set; get; }
        public int NationalityCountryID { set; get; }
        public clsCountry Nationality { set; get; }
        public string ImagePath { set; get; }

        public clsPerson()
        {
            this.ID = -1;
            this.NationalNo = "";
            this.FirstName = "";
            this.SecondName = "";
            this.ThirdName = "";
            this.LastName = "";
            this.DateOfBirth = DateTime.Now;
            this.Gender = -1;
            this.Address = "";
            this.Phone = "";
            this.Email = "";
            this.NationalityCountryID = -1;
            this.Nationality = null;
            this.ImagePath = "";

            Mode = enMode.AddNew;
        }

        private clsPerson(int ID, string NationalNo, string FirstName, string SecondName, string ThirdName, string LastName,
            DateTime DateOfBirth, short Gender, string Address, string Phone, string Email, int NationalityCountryID, string ImagePath)
        {
            this.ID = ID;
            this.NationalNo = NationalNo;
            this.FirstName = FirstName;
            this.SecondName = SecondName;
            this.ThirdName = ThirdName;
            this.LastName = LastName;
            this.DateOfBirth = DateOfBirth;
            this.Gender = Gender;
            this.Address = Address;
            this.Phone = Phone;
            this.Email = Email;
            this.NationalityCountryID = NationalityCountryID;
            this.Nationality = clsCountry.Find(NationalityCountryID);
            this.ImagePath = ImagePath;

            Mode = enMode.Update;
        }

        private bool _AddNewPerson()
        {
            //call DataAccess Layer 

            this.ID = clsPersonDataAccess.AddNewPerson(this.NationalNo, this.FirstName, this.SecondName, this.ThirdName,
            this.LastName, this.Gender, this.Address, this.DateOfBirth, this.Phone, this.Email, this.NationalityCountryID, this.ImagePath);

            if (this.ID != -1)
            {
                this.Nationality = clsCountry.Find(NationalityCountryID);
                return true;
            }
            return false;
        }

        private bool _UpdatePerson()
        {
            //call DataAccess Layer 

            bool Result = clsPersonDataAccess.UpdatePerson(this.ID, this.NationalNo, this.FirstName, this.SecondName, this.ThirdName,
              this.LastName, this.Gender, this.Address, this.DateOfBirth, this.Phone, this.Email, this.NationalityCountryID, this.ImagePath);

            if (Result)
            {
                //this.Nationality = clsCountry.Find(this.NationalityCountryID);
                return Result;
            }
            else
                return Result;
        }

        public static clsPerson Find(int PersonID)
        {
            string NationalNo = "", FirstName = "", SecondName = "", ThirdName = "", LastName = "", Address = "",
              Phone = "", Email = "", ImagePath = ""; DateTime DateOfBirth = DateTime.Now;
            int NationalityCountryID = -1;
            short Gender = -1;

            if (clsPersonDataAccess.GetPersonInfoByID(PersonID, ref NationalNo, ref FirstName, ref SecondName,
                ref ThirdName, ref LastName, ref DateOfBirth, ref Gender, ref Address, ref Phone, ref Email, ref NationalityCountryID, ref ImagePath))
                return new clsPerson(PersonID, NationalNo, FirstName, SecondName, ThirdName, LastName,
                           DateOfBirth, Gender, Address, Phone, Email, NationalityCountryID, ImagePath);
            else
                return null;
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewPerson())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:

                    return _UpdatePerson();
            }

            return false;
        }

        public static DataTable GetAllPeople()
        {
            return clsPersonDataAccess.GetAllPeople();
        }


        // ************** ************* *************//
        //                New Methodes               //
        // ************** ************* *************//

       public static DataTable GetPeopleWithStringValues()
        {
            return clsPersonDataAccess.GetAllPeopleWithJoinedInfo();
        }
        
    }
}
