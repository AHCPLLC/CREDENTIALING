﻿using AHC.CD.Entities.MasterData.Enums;
using AHC.CD.Entities.MasterData.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHC.CD.Entities.MasterProfile.HospitalPrivilege
{
    public class HospitalPrivilegeDetailHistory
    {
        public HospitalPrivilegeDetailHistory()
        {
            LastModifiedDate = DateTime.Now;
            this.DeletedDate = DateTime.Now.ToUniversalTime();
        }

        public int HospitalPrivilegeDetailHistoryID { get; set; }

        [Column(TypeName = "datetime2")]
        //[Required]
        public DateTime? IssueDate { get; set; }

        //[Required]
        [Column(TypeName = "datetime2")]
        public DateTime? ExpiryDate { get; set; }

        //[Required]
        public int? HospitalID { get; set; }
        [ForeignKey("HospitalID")]
        public Hospital Hospital { get; set; }

        #region Preference

        //[Required]
        public string Preference { get; set; }

        [NotMapped]
        public virtual PreferenceType? PreferenceType
        {
            get
            {
                if (String.IsNullOrEmpty(this.Preference))
                    return null;

                if (this.Preference.Equals("Not Available"))
                    return null;

                return (PreferenceType)Enum.Parse(typeof(PreferenceType), this.Preference);
            }
            set
            {
                this.Preference = value.ToString();
            }
        }

        #endregion

        #region StaffCategory

        //[Required]
        public int? StaffCategoryID { get; set; }
        [ForeignKey("StaffCategoryID")]
        public StaffCategory StaffCategory { get; set; }

        #endregion

        //public string StaffCategoryExplanation { get; set; }

        public string DepartmentName { get; set; }

        public string DepartmentChief { get; set; }

        #region AdmittingPrivilege

        //[Required] // Do not uncomment
        public int? AdmittingPrivilegeID { get; set; }
        [ForeignKey("AdmittingPrivilegeID")] // Do not uncomment
        public AdmittingPrivilege AdmittingPrivilege { get; set; }

        #endregion

        public double? AnnualAdmisionPercentage { get; set; }

        #region Are Privileges Temporary

        public string ArePrevilegesTemporary { get; private set; }

        [NotMapped]
        public YesNoOption? PrevilegesTemporaryYesNoOption
        {
            get
            {
                if (String.IsNullOrEmpty(this.ArePrevilegesTemporary))
                    return null;

                if (this.ArePrevilegesTemporary.Equals("Not Available"))
                    return null;

                return (YesNoOption)Enum.Parse(typeof(YesNoOption), this.ArePrevilegesTemporary);
            }
            set
            {
                this.ArePrevilegesTemporary = value.ToString();
            }
        }

        #endregion

        #region Full Unrestricted Previleges

        //[Required]
        public string FullUnrestrictedPrevilages { get; set; }

        [NotMapped]
        public YesNoOption? FullUnrestrictedPrevilagesYesNoOption
        {
            get
            {
                if (String.IsNullOrEmpty(this.FullUnrestrictedPrevilages))
                    return null;

                if (this.FullUnrestrictedPrevilages.Equals("Not Available"))
                    return null;

                return (YesNoOption)Enum.Parse(typeof(YesNoOption), this.FullUnrestrictedPrevilages);
            }
            set
            {
                this.FullUnrestrictedPrevilages = value.ToString();
            }
        }

        #endregion

        #region Specialty

        //[Required] // Do not uncomment
        public int? SpecialtyID { get; set; }
        [ForeignKey("SpecialtyID")] // Do not uncomment
        public Specialty Specialty { get; set; }

        #endregion

        [Column(TypeName = "datetime2")]
        //[Required]
        public DateTime? AffilicationStartDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? AffiliationEndDate { get; set; }

        //[Required]
        public string HospitalPrevilegeLetterPath { get; set; }

        #region HospitalContactInfo

        //[Required] // Do not uncomment
        public int? HospitalContactInfoID { get; set; }
        [ForeignKey("HospitalContactInfoID")]
        public HospitalContactInfo HospitalContactInfo { get; set; }

        #endregion

        #region HospitalContactPerson

        //[Required] // Do not uncomment
        public int? HospitalContactPersonID { get; set; }
        [ForeignKey("HospitalContactPersonID")]
        public HospitalContactPerson HospitalContactPerson { get; set; }

        #endregion
        
        #region History Status

        public string HistoryStatus { get; private set; }

        [NotMapped]
        public HistoryStatusType? HistoryStatusType
        {
            get
            {
                if (String.IsNullOrEmpty(this.HistoryStatus))
                    return null;

                if (this.HistoryStatus.Equals("Not Available"))
                    return null;

                return (HistoryStatusType)Enum.Parse(typeof(HistoryStatusType), this.HistoryStatus);
            }
            set
            {
                this.HistoryStatus = value.ToString();
            }
        }

        #endregion

        [Column(TypeName = "datetime2")]
        public DateTime LastModifiedDate { get; set; }

        public int? DeletedById { get; set; }
        [ForeignKey("DeletedById")]
        public CDUser DeletedBy { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? DeletedDate { get; set; }
    }
}