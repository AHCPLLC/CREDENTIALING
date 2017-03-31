﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHC.CD.Entities.MasterProfile.PracticeLocation
{
    public class PrimaryCredentialingContact
    {
        public PrimaryCredentialingContact()
        {
            LastModifiedDate = DateTime.Now;
        }

        public int PrimaryCredentialingContactID { get; set; }

        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string FirstName { get; set; }

        #region Address

        public string Building { get; set; }

        public string Street { get; set; }

        public string Country { get; set; }

        public string State { get; set; }

        public string County { get; set; }

        public string City { get; set; }

        public string ZipCode { get; set; }

        #endregion

        #region Mobile Number

        public string MobileNumber
        {
            get
            {
                if (String.IsNullOrEmpty(this.CountryCodeTelephone))
                    return this.Telephone;
                else if (!String.IsNullOrEmpty(this.Telephone))
                    return this.CountryCodeTelephone + "-" + this.Telephone;

                return null;
            }
            set
            {
                if (value != null)
                {
                    var numbers = value.Split(new char[] { '-' }, 2);
                    if (numbers.Length == 1)
                        this.Telephone = numbers[0];
                    else
                    {
                        this.CountryCodeTelephone = numbers[0];
                        this.Telephone = numbers[1];
                    }

                }
            }
        }

        [NotMapped]
        public string Telephone { get; set; }

        [NotMapped]
        public string CountryCodeTelephone { get; set; }


        #endregion

        #region Fax Number

        public string FaxNumber
        {
            get
            {


                if (String.IsNullOrEmpty(this.CountryCodeFax))
                    return this.Fax;
                else if (!String.IsNullOrEmpty(this.Fax))
                    return this.CountryCodeFax + "-" + this.Fax;

                return null;
            }
            set
            {
                if (value != null)
                {
                    var numbers = value.Split(new char[] { '-' }, 2);
                    if (numbers.Length == 1)
                        this.Fax = numbers[0];
                    else
                    {
                        this.CountryCodeFax = numbers[0];
                        this.Fax = numbers[1];
                    }
                }
            }
        }

        [NotMapped]
        public string Fax { get; set; }

        [NotMapped]
        public string CountryCodeFax { get; set; }

        #endregion

        public string EmailAddress { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime LastModifiedDate { get; set; }
    }
}