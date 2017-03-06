﻿var CCMActionApp = angular.module("CCMActionApp", ['mgcrea.ngStrap', 'ngSignaturePad']);

//CCMActionApp.service("GetCDUsers", function ($http,$q) {
//    return {
//        CDUsers: function () {
//            var deferred1 = $q.defer();
//            $http.get(rootDir + '/TaskTracker/GetAllCDUsers').success(function (data) {
//                deferred1.resolve();
//            }).error(function (msg, code) {
//                deferred1.reject(msg);
//            });
//            return deferred1.promise;
//        },
//    };
//})

//CCMActionApp.service("Getloginusers", function ($http,$q) {
//    return {
//        LoginUsers: function () {
//            var deferred = $q.defer();
//            $http.get(rootDir + '/TaskTracker/GetAllUsers').success(function (data) {
//                deferred.resolve();
//            }).error(function (msg, code) {
//                deferred.reject(msg);
//            });
//            return deferred.promise;
//        },
//    };
//})

//CCMActionApp.run(function ($rootScope,$q, GetCDUsers, Getloginusers) {
//    //var promise = $scope.Getlist().then(function ()
//        GetCDUsers.CDUsers().then(function (response) {
//        $rootScope.CDuserslist = response.data;
//    });

//        Getloginusers.LoginUsers().then(function (response) {
//        $rootScope.LoginUserslist = response.data;
//    })
//})

CCMActionApp.service('messageAlertEngine', ['$rootScope', '$timeout', function ($rootScope, $timeout) {

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
//var welcomeletterdata = [];
CCMActionApp.controller("CCMActionController", function ($scope,$rootScope,$http, $window, messageAlertEngine) {
    $scope.previoussignature = true;
    $scope.signatureloaded = true;
    $scope.showLoading = true;
    $scope.users = [];
    $scope.CDUsers = [];
    $scope.LoginUsers = [];
    $scope.LoadCredInfoData = function (data) {
        $scope.CDUsers = JSON.parse(CDUsers);
        $scope.LoginUsers = JSON.parse(loginusers);
        for (var i = 0; i < $scope.LoginUsers.length; i++) {

            for (var j = 0; j < $scope.CDUsers.length; j++) {

                if ($scope.LoginUsers[i].Id == $scope.CDUsers[j].AuthenicateUserId) {

                    $scope.users.push({ CDUserID: $scope.CDUsers[j].CDUserID, AuthenicateUserId: $scope.CDUsers[j].AuthenicateUserId, FullName: $scope.LoginUsers[i].FullName, Email: $scope.LoginUsers[i].Email, UserName: $scope.LoginUsers[i].UserName })
                    }
                }
            }

        
        $scope.previoussignaturepath = "";
        $scope.errormessageforsignature = false;
                $scope.signatureloaded = false;
                if (sigpath != null && sigpath != "")
                {
                    $scope.previoussignature = true;
                    $scope.previoussignaturepath = sigpath;
                }
                else
                {
                    $scope.signatureloaded = false;
                    $scope.previoussignaturepath = "";
                    $scope.previoussignature = false;
                }
    
                $scope.temp = data;
                $scope.currentdate = $scope.currentDate();
                $scope.joiningdate = $scope.currentDate();
                $scope.tempObject = angular.copy($scope.temp.CredentialingLogs[0].CredentialingAppointmentDetail);
                $scope.tempObject.CredentialingAppointmentSchedule.AppointmentDate = $scope.datesplitter($scope.tempObject.CredentialingAppointmentSchedule.AppointmentDate);
        $scope.tempObject.CredentialingAppointmentResult = {};
        $scope.tempObject.CredentialingAppointmentResult.SignedDate = $scope.currentDate();
        $scope.tempObject.CredentialingAppointmentResult.CredentialingLevel = $scope.tempObject.RecommendedCredentialingLevel;
        $scope.tempObject.CredentialingAppointmentResult.SignedByID = $scope.temp.InitiatedByID;
        $scope.tempObject.CredentialingAppointmentResult.WelcomeLetterPath = $scope.tempObject.WelcomeLetterPath;
        $scope.tempObject.CredentialingAppointmentResult.SignatureFile = null;
        $scope.tempObject.CredentialingAppointmentResult.FileUploadPath = $scope.tempObject.FileUploadPath;

        for (var j = 0; j < $scope.users.length; j++) {
            if ($scope.tempObject.AssignedToCCOID == $scope.users[j].CDUserID) {
                $scope.tempObject.CCOName = $scope.users[j].UserName
            }
        }

    }

    //to hide the error message after uploading a signature in CCM Action
    $("input:file").change(function () {
        var fileName = $(this).val();
        if (fileName == "" || fileName == null) {
            $('#errorid').show();
        }
        else {
            $('#errorid').hide();
        }
    });
   
    $scope.datesplitter = function (date) {
        returndate = "";
        if (date != null) {
            var newdate = date.split('T')[0].split('-');
            returndate = newdate[1] + "/" + newdate[2] + "/" + newdate[0];
        }
        return returndate;
    }

    $scope.editletter = false;
    $scope.showeditview = function () {
        $scope.editletter = true;
    }

    $scope.canceleditview = function () {
        $scope.editletter = false;
        $scope.currentdate = currentdateforwelcomeletter;
        $scope.joiningdate = joiningdateforwelcomeletter;
    }

    //$scope.Savewelcomeletter = function () {
    //     currentdateforwelcomeletter = angular.copy($scope.currentdate);
    //     joiningdateforwelcomeletter = angular.copy($scope.joiningdate);
    //    $scope.editletter = false;
    //}

    $scope.$watch('tempObject.RecommendedCredentialingLevel', function () {
        $scope.tempObject.CredentialingAppointmentResult.CredentialingLevel = $scope.tempObject.RecommendedCredentialingLevel;
    })

    $scope.currentDate = function () {
        var today = new Date();
        var dd = today.getDate();
        var mm = today.getMonth() + 1; //January is 0!

        var yyyy = today.getFullYear();
        if (dd < 10) {
            dd = '0' + dd
        }
        if (mm < 10) {
            mm = '0' + mm
        }
        var today = mm + '/' + dd + '/' + yyyy;
        return today;
    }
    $scope.validatesignature = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAASwAAACMCAYAAADGFpQvAAAEGUlEQVR4Xu3UAQkAAAwCwdm/9HI83BLIOdw5AgQIRAQWySkmAQIEzmB5AgIEMgIGK1OVoAQIGCw/QIBARsBgZaoSlAABg+UHCBDICBisTFWCEiBgsPwAAQIZAYOVqUpQAgQMlh8gQCAjYLAyVQlKgIDB8gMECGQEDFamKkEJEDBYfoAAgYyAwcpUJSgBAgbLDxAgkBEwWJmqBCVAwGD5AQIEMgIGK1OVoAQIGCw/QIBARsBgZaoSlAABg+UHCBDICBisTFWCEiBgsPwAAQIZAYOVqUpQAgQMlh8gQCAjYLAyVQlKgIDB8gMECGQEDFamKkEJEDBYfoAAgYyAwcpUJSgBAgbLDxAgkBEwWJmqBCVAwGD5AQIEMgIGK1OVoAQIGCw/QIBARsBgZaoSlAABg+UHCBDICBisTFWCEiBgsPwAAQIZAYOVqUpQAgQMlh8gQCAjYLAyVQlKgIDB8gMECGQEDFamKkEJEDBYfoAAgYyAwcpUJSgBAgbLDxAgkBEwWJmqBCVAwGD5AQIEMgIGK1OVoAQIGCw/QIBARsBgZaoSlAABg+UHCBDICBisTFWCEiBgsPwAAQIZAYOVqUpQAgQMlh8gQCAjYLAyVQlKgIDB8gMECGQEDFamKkEJEDBYfoAAgYyAwcpUJSgBAgbLDxAgkBEwWJmqBCVAwGD5AQIEMgIGK1OVoAQIGCw/QIBARsBgZaoSlAABg+UHCBDICBisTFWCEiBgsPwAAQIZAYOVqUpQAgQMlh8gQCAjYLAyVQlKgIDB8gMECGQEDFamKkEJEDBYfoAAgYyAwcpUJSgBAgbLDxAgkBEwWJmqBCVAwGD5AQIEMgIGK1OVoAQIGCw/QIBARsBgZaoSlAABg+UHCBDICBisTFWCEiBgsPwAAQIZAYOVqUpQAgQMlh8gQCAjYLAyVQlKgIDB8gMECGQEDFamKkEJEDBYfoAAgYyAwcpUJSgBAgbLDxAgkBEwWJmqBCVAwGD5AQIEMgIGK1OVoAQIGCw/QIBARsBgZaoSlAABg+UHCBDICBisTFWCEiBgsPwAAQIZAYOVqUpQAgQMlh8gQCAjYLAyVQlKgIDB8gMECGQEDFamKkEJEDBYfoAAgYyAwcpUJSgBAgbLDxAgkBEwWJmqBCVAwGD5AQIEMgIGK1OVoAQIGCw/QIBARsBgZaoSlAABg+UHCBDICBisTFWCEiBgsPwAAQIZAYOVqUpQAgQMlh8gQCAjYLAyVQlKgIDB8gMECGQEDFamKkEJEDBYfoAAgYyAwcpUJSgBAgbLDxAgkBEwWJmqBCVAwGD5AQIEMgIGK1OVoAQIGCw/QIBARsBgZaoSlAABg+UHCBDICBisTFWCEiBgsPwAAQIZAYOVqUpQAgQMlh8gQCAjYLAyVQlKgMADe7MAjcEgBREAAAAASUVORK5CYII="
    $scope.confirmationAddCCMAction = function () {

        ResetFormForValidation($("#CCMAction"));
        if ($('#digitalsignature')[0].control.checked == true) {
            $scope.image = canvas.toDataURL("image/png");
            if ($scope.image == document.getElementById('blank').toDataURL() || $scope.image == $scope.validatesignature)
                $scope.errormessageforsignature = true;
            else
            {
                $scope.errormessageforsignature = false;
                if ($("#CCMAction").valid()) {
                    $scope.ConfirmTitle = 'Confirm Submission ';
                    if ($scope.approvalStatusType == 1) {
                        $scope.ConfirmMessage = 'Do you want to Approve?';
                    }
                    else {
                        $scope.ConfirmMessage = 'Do you want to Reject?';
                    }
                    $('#myModal').modal({
                        backdrop: 'static'
                    });
                }
            }
        }
        if ($('#reusedigitalsignature')[0].control.checked == true) {
            $scope.image = $scope.previoussignaturepath;
        }
        if (!$('#digitalsignature')[0].control.checked == true) {
            if ($("#CCMAction").valid()) {
                $scope.ConfirmTitle = 'Confirm Submission ';
                if ($scope.approvalStatusType == 1) {
                    $scope.ConfirmMessage = 'Do you want to Approve?';
                }
                else {
                    $scope.ConfirmMessage = 'Do you want to Reject?';
                }
                $('#myModal').modal({
                    backdrop: 'static'
                });
            }
        }
    }
    $('#myModal').modal('hide');
    $scope.ccmAction = function (Form_Id) {
        //$scope.errormessageforsignature = false;
        //if (sigApi.validateForm()) {
        //    alert();
        //}
        //if ($('#digitalsignature')[0].control.checked == true) {
        //    $scope.image = canvas.toDataURL("image/png");
        //    if ($scope.image == document.getElementById('blank').toDataURL())
        //        $scope.errormessageforsignature = true;
        //    else
        //    { $scope.errormessageforsignature = false; }
        //}
        //if ($('#reusedigitalsignature')[0].control.checked == true)
        //{
        //    $scope.image = $scope.previoussignaturepath;
        //}
        //}
        if ($scope.errormessageforsignature == false) {
            var credAppointmentId = $scope.tempObject.CredentialingAppointmentDetailID;
            ResetFormForValidation($("#" + Form_Id));
            if ($("#" + Form_Id).valid()) {
                var $form = $("#" + Form_Id)[0];
                $.ajax({
                    url: rootDir + '/Credentialing/CCM/CCMActionUploadAsync?profileId=' + $scope.temp.ProfileID,
                    type: 'POST',
                    data: new FormData($form),
                    async: false,
                    cache: false,
                    contentType: false,
                    processData: false,
                    success: function (data) {
                        try {
                            $('#myModal').modal('hide');
                            if (data.status == "true") {

                                if ($("#approvestat option:selected").text() == 'Approved') {
                                    messageAlertEngine.callAlertMessage('ccmAccepted', 'Application is approved', "success", true);
                                } else {
                                    messageAlertEngine.callAlertMessage('ccmAccepted', 'Application is rejected', "danger", true);
                                }

                                //messageAlertEngine.callAlertMessage('ccmAccepted', 'Appointment is confirmed', "success", true);
                                $("body").animate({ scrollTop: $("#errorCCM").offset().top });

                                var delay = 2000; //2 seconds
                                setTimeout(function () {
                                    $window.location = '/credentialing/ccm/index';
                                }, delay);

                            }
                        } catch (e) {

                        }
                    }
                });
            }
        }
        else
        {
            $scope.errormessageforsignature = true;
            //$('#myModal').modal('hide');
        }
        
    }
    $scope.showuploaddiv = false;
    $scope.showsignaturediv = false;
    $scope.showreusesignaturediv = true;
    $scope.signaturefunction = function (type) {
        if (type == 'upload') {
            $('#selectfile').trigger('click', function () { });
            $scope.showreusesignaturediv = false;
            $scope.showsignaturediv = false;
            $scope.showuploaddiv = true;
        } else if (type == 'digitalsignature') {
            $scope.showreusesignaturediv = false;
            $scope.showuploaddiv = false;
            $scope.errormessageforsignature = false;
            $scope.showsignaturediv = true;
        }
        else if (type == 'reusedigitalsignature') {
            $scope.showsignaturediv = false;
            $scope.showuploaddiv = false;
            $scope.showreusesignaturediv = true;
        }
        else { }
        if (type == 'upload' && !$('#digitalsignature')[0].control.checked == true || type == 'reusedigitalsignature' && !$('#digitalsignature')[0].control.checked == true) {
            context.clearRect(0, 0, canvas.width, canvas.height);
        }
    }
    var canvas = document.getElementById('myCanvas');
    var context = canvas.getContext('2d');
    $scope.cleardigitalsignature = function()
    {
        context.clearRect(0, 0, canvas.width, canvas.height);
    }
    //document.getElementById('clear').addEventListener('click', function () {
    //    context.clearRect(0, 0, canvas.width, canvas.height);
    //}, false);
    $scope.image = "";
    $scope.savedigitalsignature = function () {
        console.log($scope.signature);
        $scope.image = canvas.toDataURL("image/png");
    }
});


//----------------- Calender hide on select data configuration ------------------
CCMActionApp.config(function ($datepickerProvider) {

    angular.extend($datepickerProvider.defaults, {
        startDate: 'today',
        autoclose: true,
        useNative: true
    });
})
$(document).ready(function () {
    //var sigPadOptions = {
    //    defaultAction: 'drawIt',
    //    drawOnly: true,
    //    validateFields: false
    //};
    //var sigApi = $('.sigPad').signaturePad(sigPadOptions);
    $("#sidemenu").addClass("menu-in");
    $("#page-wrapper").addClass("menuup");
    $(".ProviderTypeSelectAutoList").hide();
    $("body").tooltip({ selector: '[data-toggle=tooltip]' });
});
function ResetFormForValidation(form) {
    form.removeData('validator');
    form.removeData('unobtrusiveValidation');
    $.validator.unobtrusive.parse(form);
};
var toggleDiv = function (divId) {
    $('#' + divId).slideToggle();
};
$(document).click(function (event) {
    if (!$(event.target).hasClass("form-control") && $(event.target).parents(".ProviderTypeSelectAutoList").length === 0) {
        $(".ProviderTypeSelectAutoList").hide();
    }
});