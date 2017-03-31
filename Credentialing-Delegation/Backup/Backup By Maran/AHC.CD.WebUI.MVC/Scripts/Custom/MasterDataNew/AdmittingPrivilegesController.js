﻿//--------------------- Angular Module ----------------------
var masterDataAdmittingPrivileges = angular.module("masterDataAdmittingPrivileges", ['ui.bootstrap']);

//Service for getting master data
masterDataAdmittingPrivileges.service('masterDataService', ['$http', '$q', function ($http, $q) {

    this.getMasterData = function (URL) {
        return $http.get(URL).then(function (value) { return value.data; });
    };

}]);

masterDataAdmittingPrivileges.service('messageAlertEngine', ['$rootScope', '$timeout', function ($rootScope, $timeout) {

    $rootScope.messageDesc = "";
    $rootScope.activeMessageDiv = "";
    $rootScope.messageType = "";

    var animateMessageAlertOff = function () {
        $rootScope.closeAlertMessage();
    };


    this.callAlertMessage = function (calledDiv, msg, msgType, dismissal) { //messageAlertEngine.callAlertMessage('updateHospitalPrivilege' + IndexValue, "Data Updated Successfully !!!!", "success", true);                            
        $rootScope.activeMessageDiv = calledDiv;
        $rootScope.messageDesc = msg;
        $rootScope.messageType = msgType;
        if (dismissal) {
            $timeout(animateMessageAlertOff, 5000);
        }
    }

    $rootScope.closeAlertMessage = function () {
        $rootScope.messageDesc = "";
        $rootScope.activeMessageDiv = "";
        $rootScope.messageType = "";
    }
}])


//=========================== Controller declaration ==========================
masterDataAdmittingPrivileges.controller('masterDataAdmittingPrivilegesController', ['$scope', '$http', '$filter', '$rootScope', 'masterDataService', 'messageAlertEngine', function ($scope, $http, $filter, $rootScope, masterDataService, messageAlertEngine) {

    $scope.data = [];
    $scope.AdmittingPrivileges = [];

    $http.get(rootDir + "/MasterDataNew/GetAllAdmittingPrivileges").success(function (value) {

        for (var i = 0; i < value.length ; i++) {
            if (value[i] != null) {
                value[i].LastModifiedDate = ($scope.ConvertDateFormat(value[i].LastModifiedDate)).toString();
            }

        }

        $scope.AdmittingPrivileges = angular.copy(value);
    });


    //Convert the date from database to normal
    $scope.ConvertDateFormat = function (value) {
        var returnValue = value;
        var dateData = "";
        try {
            if (value.indexOf("/Date(") == 0) {
                returnValue = new Date(parseInt(value.replace("/Date(", "").replace(")/", ""), 10));
                var day = returnValue.getDate() < 10 ? '0' + returnValue.getDate() : returnValue.getDate();
                var tempo = returnValue.getMonth() + 1;
                var month = tempo < 10 ? '0' + tempo : tempo;
                var year = returnValue.getFullYear();
                dateData = month + "-" + day + "-" + year;
            }
            return dateData;
        } catch (e) {
            return dateData;
        }
        return dateData;
    };



    $scope.tempAP = {};

    //------------ gets the template to ng-include for a table row / item -------------------
    $scope.getTemplate = function (admittingPrivilege) {
        if (admittingPrivilege.AdmittingPrivilegeID === $scope.tempAP.AdmittingPrivilegeID) return 'editAdmittingPrivilege';
        else return 'displayAdmittingPrivilege';
    };

    //-------------------- Edit AdmittingPrivilege ----------
    $scope.editAdmittingPrivilege = function (admittingPrivilege) {
        $scope.tempAP = angular.copy(admittingPrivilege);
        $scope.disableAdd = true;
        $scope.lengthExceedsError = "";
    };

    //------------------- Add AdmittingPrivilege ---------------------
    $scope.addAdmittingPrivilege = function (admittingPrivilege) {
        $scope.disableEdit = true;
        var Month = new Date().getMonth() + 1;
        var _month = Month < 10 ? '0' + Month : Month;
        var _date = new Date().getDate() < 10 ? '0' + new Date().getDate() : new Date().getDate();
        var _year = new Date().getFullYear();
        $scope.disableAdd = true;
        var temp = {
            AdmittingPrivilegeID: 0,
            Title: "",
            Status: "Active",
            LastModifiedDate: _month + "-" + _date + "-" + _year
        }
        $scope.AdmittingPrivileges.splice(0, 0, angular.copy(temp));
        $scope.tempAP = angular.copy(temp);
        $scope.lengthExceedsError = "";
    };

    //------------------- Update AdmittingPrivilege ---------------------
    $scope.updateAdmittingPrivilege = function (idx) {
        //$scope.tempAP.LastModifiedDate = "02/03/2015"
        var updateData = {
            AdmittingPrivilegeID: $scope.tempAP.AdmittingPrivilegeID,
            Title: $scope.tempAP.Title,
            StatusType: 1,
        };

        var isExist = true;

        for (var i = 0; i < $scope.AdmittingPrivileges.length; i++) {

            if (updateData.Title && $scope.AdmittingPrivileges[i].AdmittingPrivilegeID != updateData.AdmittingPrivilegeID && $scope.AdmittingPrivileges[i].Title.replace(" ", "").toLowerCase() == updateData.Title.replace(" ", "").toLowerCase()) {

                isExist = false;
                $scope.existErr = "The Title is Exist";
                break;
            }
        }
        if (!updateData.Title) { $scope.error = "Please Enter the Title"; }

        if (updateData.Title > 100) {
            isExist = false;
            $scope.lengthExceedsError = "The Title should be less than or equal to 100 characters.";
        }

        if (updateData && isExist) {
            $http.post(rootDir + '/MasterDataNew/updateAdmittingPrivileges', updateData).
            success(function (data, status, headers, config) {
                try {
                    //----------- success message -----------
                    if (data.status == "true") {
                        messageAlertEngine.callAlertMessage("addedNewPrivilegeDetails", "Admitting Privilege Details Updated Successfully !!!!", "success", true);
                        data.admitingPrivilege.LastModifiedDate = $scope.ConvertDateFormat(data.admitingPrivilege.LastModifiedDate);
                        for (var i = 0; i < $scope.AdmittingPrivileges.length; i++) {

                            if ($scope.AdmittingPrivileges[i].AdmittingPrivilegeID == data.admitingPrivilege.AdmittingPrivilegeID) {
                                $scope.AdmittingPrivileges[i] = data.admitingPrivilege;
                                break;
                            }
                        }

                        $scope.reset();
                        $scope.error = "";
                        $scope.existErr = "";
                    }
                } catch (e) {

                }
            }).
            error(function (data, status, headers, config) {
                //----------- error message -----------
                messageAlertEngine.callAlertMessage("addedNewPrivilegeDetailsError", "Sorry Unable To Update Admitting Privilege !!!!", "danger", true);
            });
        }


    };

    $scope.lengthValidation = function (title) {
        if (title.length > 100) {
            $scope.lengthExceedsError = "The Title should be less than or equal to 100 characters.";
        }
        else {
            $scope.lengthExceedsError = "";
        }
    }

    //------------------- Save AdmittingPrivilege ---------------------
    $scope.saveAdmittingPrivilege = function (idx) {
        //$scope.tempAP.LastModifiedDate = "02/03/2015"
        var admittingdata = {
            AdmittingPrivilegeID: 0,
            Title: $scope.tempAP.Title,
            StatusType: 1,
        };

        var isExist = true;

        for (var i = 0; i < $scope.AdmittingPrivileges.length; i++) {

            if (admittingdata.Title && $scope.AdmittingPrivileges[i].Title.replace(" ", "").toLowerCase() == admittingdata.Title.replace(" ", "").toLowerCase()) {

                isExist = false;
                $scope.existErr = "The Title is Exist";
                break;
            }
        }
        if (!admittingdata.Title) { $scope.error = "Please Enter the Title *"; }

        if (admittingdata.Title.length > 100) {
            isExist = false;
            $scope.lengthExceedsError = "The Title should be less than or equal to 100 characters.";
        }

        if (admittingdata.Title && isExist) {
            $http.post(rootDir + '/MasterDataNew/AddAdmittingPrivileges', admittingdata).
            success(function (data, status, headers, config) {
                try {
                    //----------- success message -----------
                    if (data.status == "true") {
                        messageAlertEngine.callAlertMessage("addedNewPrivilegeDetails", "New Admitting Privilege Details Added Successfully !!!!", "success", true);
                        data.admitingPrivilege.LastModifiedDate = $scope.ConvertDateFormat(data.admitingPrivilege.LastModifiedDate);
                        for (var i = 0; i < $scope.AdmittingPrivileges.length; i++) {

                            if ($scope.AdmittingPrivileges[i].AdmittingPrivilegeID == 0) {
                                $scope.AdmittingPrivileges[i] = angular.copy(data.admitingPrivilege);
                            }
                        }

                        $scope.reset();
                        $scope.error = "";
                        $scope.existErr = "";
                    }
                    else {
                        messageAlertEngine.callAlertMessage("addedNewPrivilegeDetails", "Sorry Unable To Add Admitting Privilege !!!!", "danger", true);
                        $scope.AdmittingPrivileges.slpice(0, 1);
                    }
                } catch (e) {

                }
            }).
            error(function (data, status, headers, config) {
                //----------- error message -----------
                messageAlertEngine.callAlertMessage("addedNewPrivilegeDetails", "Sorry Unable To Add Admitting Privilege !!!!", "danger", true);
                $scope.AdmittingPrivileges.slpice(0, 1);
            });
        }

    };


    //----------------- AdmittingPrivilege new add cancel ---------------
    $scope.cancelAdd = function () {
        $scope.AdmittingPrivileges.splice(0, 1);
        $scope.tempAP = {};
        $scope.disableAdd = false;
        $scope.disableEdit = false;
        $scope.error = "";
        $scope.existErr = "";
    };

    //-------------------- Reset AdmittingPrivilege ----------------------
    $scope.reset = function () {
        $scope.tempAP = {};
        $scope.disableAdd = false;
        $scope.disableEdit = false;
        $scope.error = "";
        $scope.existErr = "";
    };

    $scope.CurrentPage = [];

    //-------------------------- angular bootstrap pagger with custom-----------------
    $scope.maxSize = 5;
    $scope.bigTotalItems = 0;
    $scope.bigCurrentPage = 1;

    //-------------------- page change action ---------------------
    $scope.pageChanged = function (pagnumber) {
        $scope.bigCurrentPage = pagnumber;
    };

    //-------------- current page change Scope Watch ---------------------
    $scope.$watch('bigCurrentPage', function (newValue, oldValue) {
        $scope.CurrentPage = [];
        var startIndex = (newValue - 1) * 10;
        var endIndex = startIndex + 9;
        if ($scope.AdmittingPrivileges) {
            for (startIndex; startIndex <= endIndex ; startIndex++) {
                if ($scope.AdmittingPrivileges[startIndex]) {
                    $scope.CurrentPage.push($scope.AdmittingPrivileges[startIndex]);
                } else {
                    break;
                }
            }
        }
    });

    //-------------- License Scope Watch ---------------------
    $scope.$watchCollection('AdmittingPrivileges', function (newValue, oldValue) {
        if (newValue) {
            $scope.bigTotalItems = newValue.length;

            $scope.CurrentPage = [];
            $scope.bigCurrentPage = 1;

            var startIndex = ($scope.bigCurrentPage - 1) * 10;
            var endIndex = startIndex + 9;

            for (startIndex; startIndex <= endIndex ; startIndex++) {
                if ($scope.AdmittingPrivileges[startIndex]) {
                    $scope.CurrentPage.push($scope.AdmittingPrivileges[startIndex]);
                } else {
                    break;
                }
            }
        }
    });
    //------------------- end ------------------
    $scope.filterData = function () {
        $scope.pageChanged(1);
    }

    $scope.IsValidIndex = function (index) {

        var startIndex = ($scope.bigCurrentPage - 1) * 10;
        var endIndex = startIndex + 9;

        if (index >= startIndex && index <= endIndex)
            return true;
        else
            return false;

    }

}]);