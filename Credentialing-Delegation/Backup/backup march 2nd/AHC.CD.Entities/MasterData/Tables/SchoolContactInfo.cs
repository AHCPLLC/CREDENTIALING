﻿using AHC.CD.Entities.MasterData.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHC.CD.Entities.MasterData.Tables
{
  public   class SchoolContactInfo
    {
      public SchoolContactInfo()
        {
            LastModifiedDate = DateTime.Now;
        }

        public int SchoolContactInfoID { get; set; }

        public string  Location { get; set; }
        #region Address

        //[Required]
        public string UnitNumber { get; set; }

        //[Required]
        public string Street { get; set; }

        //[Required]
        public string Country { get; set; }

        //[Required]
        public string State { get; set; }

        public string County { get; set; }

        //[Required]
        public string City { get; set; }

        public string ZipCode { get; set; }

        #endregion        

        #region Phone Number

        //[Required]
        public string Phone
        {
            get
            {
                if (String.IsNullOrEmpty(this.PhoneCountryCode))
                    return this.PhoneNumber;
                else if (!String.IsNullOrEmpty(this.PhoneNumber))
                    return this.PhoneCountryCode + "-" + this.PhoneNumber;

                return null;
            }
            set
            {
                if(value != null)
                {
                    var numbers = value.Split(new char[]{'-'}, 2);
                    if (numbers.Length == 1)
                        this.PhoneNumber = numbers[0];
                    else
                    {
                        this.PhoneCountryCode = numbers[0];
                        this.PhoneNumber = numbers[1];
                    }
                }
            }
        }

        [NotMapped]
        public string PhoneNumber { get; set; }

        [NotMapped]
        public string PhoneCountryCode { get; set; }


        #endregion

        #region Fax Number

        public string Fax
        {
            get
            {
                if (String.IsNullOrEmpty(this.FaxCountryCode))
                    return this.FaxNumber;
                else if (!String.IsNullOrEmpty(this.FaxNumber))
                    return this.FaxCountryCode + "-" + this.FaxNumber;

                return null;
            }
            set
            {
                if(value != null)
                {
                    var numbers = value.Split(new char[] { '-' }, 2);
                    if (numbers.Length == 1)
                        this.FaxNumber = numbers[0];
                    else
                    {
                        this.FaxCountryCode = numbers[0];
                        this.FaxNumber = numbers[1];
                    }
                }
            }
        }

        [NotMapped]
        public string FaxNumber { get; set; }

        [NotMapped]
        public string FaxCountryCode { get; set; }

        #endregion
        
        public string Email { get; set; }

        #region Status

        public string Status { get; private set; }

        [NotMapped]
        public StatusType? StatusType
        {
            get
            {
                if (String.IsNullOrEmpty(this.Status))
                    return null;

                return (StatusType)Enum.Parse(typeof(StatusType), this.Status);
            }
            set
            {
                this.Status = value.ToString();
            }
        }

        #endregion

      

        [Column(TypeName = "datetime2")]
        public DateTime LastModifiedDate { get; set; }

    }
}