﻿


CCMDashboard.factory('CCMDashboardFactory', ['$q', '$rootScope', '$filter', '$timeout', '$window', '$log', function ($q, $rootScope, $filter, $timeout, $window, $log) {

    function getPage(start, number, params) {
        var deferred = $q.defer();
        $rootScope.filtered = $filter('orderBy')($rootScope.TempCCMAppointments, params.sort.predicate, params.sort.reverse);
        var result = $rootScope.filtered.slice(start, start + number);
        deferred.resolve({
            data: result,
            numberOfPages: Math.ceil($rootScope.filtered.length / number)
        });
        return deferred.promise;
    }
    function getFilteredAppointmentdataByPlan(PlanName) {
        return $filter('filter')($rootScope.CCMAppointments, { PlanName: PlanName })[0];
    }
    function resetTableState(tableState) {
        if (tableState !== undefined) {
            tableState.sort = {};
            tableState.pagination.start = 0;
            tableState.search.predicateObject = {};
            return tableState;
        }
    }
    function getFilteredAppointmentdataByAppointmentDate(AppointmentDate) {
        return $filter('filter')($rootScope.CCMAppointments, { AppointmentDate: AppointmentDate });
    }
    function exportToTable(type, tableId) {
        switch (type) {
            case "Excel":
                angular.element(tableId).tableExport({ type: 'excel', escape: 'false', ignoreColumn: '[7]' })
                break;
            case "CSV":
                angular.element(tableId).tableExport({ type: 'csv', escape: 'false', ignoreColumn: '[7]' })
                break;
            case "Pdf":
                angular.element(tableId).tableExport({ type: 'pdf', pdfFontSize: '10', escape: 'false', ignoreColumn: '[7]', htmlContent: 'true' })
                break;
        }
    }
    return {
        getPage: getPage,
        getFilteredAppointmentdataByPlan: getFilteredAppointmentdataByPlan,
        resetTableState: resetTableState,
        getFilteredAppointmentdataByAppointmentDate: getFilteredAppointmentdataByAppointmentDate,
        exportToTable: exportToTable

    }
}]);