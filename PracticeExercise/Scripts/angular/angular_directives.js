/**
* Angular metadata module
*/

angular.module('metadataModule', [])
.directive('transactionMetadata', function(){
    return {
        restrict: 'E', /* restrict this directive to elements */
        templateUrl: '/Static/metadata.html',
        //scope: 'inherit', //This would grab any scope available on the element already
        link: function (scope, element, attrs, componentCtrl) {
            
        },
       
        controller: function ($scope, $rootScope, $sce) {

            // this method will fetch all the rules and excutes that
            $scope.evaluateRules = function (data) {

                var formData = ''; // using for validation - single level data object
                $rootScope.multiLevelFormDataArray = []; // using for validation - multi level data object
                
                // adding the key and values for multi level data object
                if ($scope.transactionData) {
                    angular.forEach($scope.transactionData, function (objectValue, objectKey) {

                        // checking for json object is array or object
                        if (!angular.isArray(objectValue)) {
                            if (objectValue.pageSets) {
                                angular.forEach(objectValue.pageSets[0].pages, function (pageItem, pageIndex) {
                                    var pageObj = {};
                                    angular.forEach(pageItem.fields, function (fieldItem, fieldIndex) {
                                        // for validations require only key and value object
                                        // so iterating the value object and converting that in to key and value object
                                        if (fieldItem.fieldValue instanceof Object) {
                                            if (Object.keys(fieldItem.fieldValue).length > 1) {
                                                for (var key in fieldItem.fieldValue) {
                                                    if (key == 'code') {
                                                        pageObj[fieldItem.code] = fieldItem.fieldValue[key];
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        else {
                                            // if value contains string value then adding that value in to page object key
                                            pageObj[fieldItem.code] = fieldItem.fieldValue;
                                        }
                                    });
                                    $scope[pageItem.code] = pageObj;
                                    formData += 'var ' + pageItem.code + ' = ' + JSON.stringify(pageObj) + ';';
                                });
                            }
                        }
                        else {
                            // json object is array then iterating that
                            for (var i = 0; i < objectValue.length; i++) {
                                var multiLevelFormData = '';
                                var object = null;
                                if (objectValue[i]['pageSets']) {
                                    object = objectValue[i];
                                }
                                else {
                                    for (var key in objectValue[i]) {
                                        if (objectValue[i][key] instanceof Object) {
                                            if (objectValue[i][key]['pageSets']) {
                                                object = objectValue[i][key];
                                            }
                                        }
                                    }
                                }
                                if (object.pageSets) {
                                    angular.forEach(object.pageSets[0].pages, function (pageItem, pageIndex) {
                                        var pageObj = {};
                                        angular.forEach(pageItem.fields, function (fieldItem, fieldIndex) {                                           

                                            //applying  default value rules
                                            if (fieldItem.defaultValueRule && !fieldItem.fieldValue) {
                                                fieldItem.fieldValue = eval($rootScope.multiLevelFormDataArray[i] + ';' + fieldItem.defaultValueRule);
                                            }

                                            pageObj[fieldItem.code] = fieldItem.fieldValue;
                                        });
                                        $scope[pageItem.code] = pageObj;
                                        multiLevelFormData += 'var ' + pageItem.code + ' = ' + JSON.stringify(pageObj) + ';';
                                        //addind the given form data in to object - using for validations
                                        $rootScope.multiLevelFormDataArray[i] = formData + multiLevelFormData;


                                    });
                                }
                            }
                        }
                    })
                }
            }

            // Angular validation to evaluate the conditional based rules
            $rootScope.evaluateRule = function (expression, index) {
                if (expression == "0") {
                    return false;
                }
                //  replacing the expression object key values and evaluating the expression with given form data
                if (expression != undefined && expression != null && expression != '' && expression != "1") {
                    return eval($rootScope.multiLevelFormDataArray[index] + ';' + expression);
                }
                return true;
            }

            $rootScope.phoneFormat = function (fieldObject) {
                fieldObject.fieldValue = document.getElementById(fieldObject.code).value;
                if (fieldObject.fieldValue.length < 14)
                    fieldObject.isValid = false;
                else
                    fieldObject.isValid = true;
                var enteredPhoneNumber = fieldObject.fieldValue;
                /*if(!isNaN(originalPhoneNumber)){
                    angular.element(this).next().next().css('display','block');  
                }*/
                var originalPhoneNumber = enteredPhoneNumber.replace(/\(/g, "");
                originalPhoneNumber = originalPhoneNumber.replace(/\)/g, "");
                originalPhoneNumber = originalPhoneNumber.replace(/-/g, "");
                originalPhoneNumber = originalPhoneNumber.replace(/ /g, "");
                if (originalPhoneNumber.length > 10) {
                    originalPhoneNumber = originalPhoneNumber.substring(0, 10);
                }
                var formatedPhoneNumber = '';
                var i = 0;
                var closeBracket = false;
                if (originalPhoneNumber.length > 3) {
                    formatedPhoneNumber += '(' + originalPhoneNumber.substring(0, 3) + ') ';
                    for (i = 3; i < originalPhoneNumber.length; i++) {
                        if (i == 6)
                            formatedPhoneNumber += '-';
                        formatedPhoneNumber += originalPhoneNumber.substring(i, i + 1);
                    }
                } else {
                    formatedPhoneNumber = originalPhoneNumber;
                }

                fieldObject.fieldValue = formatedPhoneNumber;
            };

            $scope.renderHtml = function (html_code) {
                var iframe = document.getElementById('test_iframe');
                var iframedoc = iframe.document;
                if (iframe.contentDocument)
                    iframedoc = iframe.contentDocument;
                else if (iframe.contentWindow)
                    iframedoc = iframe.contentWindow.document;
                if (iframedoc) {
                    // Put the content in the iframe
                    iframedoc.open();
                    iframedoc.writeln($sce.trustAsHtml(html_code));
                    iframedoc.close();
                } else {
                    //just in case of browsers that don't support the above 3 properties.
                    //fortunately we don't come across such case so far.
                    //alert('Cannot inject dynamic contents into iframe.');
                }
                iframe.height = iframe.contentWindow.document.body.scrollHeight;
                iframedoc.body.style.wordWrap = 'break-word';
            };
        }
    };
})