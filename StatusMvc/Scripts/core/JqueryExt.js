/// <reference path="api/jquery-1.7.1.min.js" />
/// <reference path="api/jquery.validate.min.js" />
/// <reference path="api/knockout-2.0.0.js" />
/// <reference path="api/jquery-ui-1.8.16.min.js" />
/// <reference path="api/jquery.autoresize.js" />
/// <reference path="api/date.js" />
/// <reference path="api/jquery.hotkeys.js" />
/// <reference path="api/tag-it.js" />


/*!
* jQuery.parseJSON() extension (supports ISO & Asp.net date conversion)
*
* Version 1.0 (13 Jan 2011)
*
* Copyright (c) 2011 Robert Koritnik,
* Licensed under the terms of the MIT license
* http://www.opensource.org/licenses/mit-license.php
*/
(function ($) {

    // JSON RegExp
    var rvalidchars = /^[\],:{}\s]*$/;
    var rvalidescape = /\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g;
    var rvalidtokens = /"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g;
    var rvalidbraces = /(?:^|:|,)(?:\s*\[)+/g;
    var dateISO = /\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(?:[.,]\d+)?Z/i;
    var dateNet = /\/Date\((\d+)(?:-\d+)?\)\//i;

    // replacer RegExp
    var replaceISO = /"(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2})(?:[.,](\d+))?Z"/i;
    var replaceNet = /"\\\/Date\((\d+)(?:-\d+)?\)\\\/"/i;

    // determine JSON native support
    var nativeJSON = (window.JSON && window.JSON.parse) ? true : false;
    var extendedJSON = nativeJSON && window.JSON.parse('{"x":9}', function (k, v) { return "Y"; }) === "Y";

    var jsonDateConverter = function (key, value) {
        if (typeof (value) === "string") {
            if (dateISO.test(value)) {
                return new Date(value);
            }
            if (dateNet.test(value)) {
                return new Date(parseInt(dateNet.exec(value)[1], 10));
            }
        }
        return value;
    };

    $.extend({
        parseJSON: function (data, convertDates) {
            /// <summary>Takes a well-formed JSON string and returns the resulting JavaScript object.</summary>
            /// <param name="data" type="String">The JSON string to parse.</param>
            /// <param name="convertDates" optional="true" type="Boolean">Set to true when you want ISO/Asp.net dates to be auto-converted to dates.</param>

            // convertDates = convertDates === false ? false : true;

            if (typeof data !== "string" || !data) {
                return null;
            }

            // Make sure leading/trailing whitespace is removed (IE can't handle it)
            data = $.trim(data);

            // Make sure the incoming data is actual JSON
            // Logic borrowed from http://json.org/json2.js
            if (rvalidchars.test(data
				.replace(rvalidescape, "@")
				.replace(rvalidtokens, "]")
				.replace(rvalidbraces, ""))) {
                // Try to use the native JSON parser
                if (extendedJSON || (nativeJSON && convertDates !== true)) {
                    return window.JSON.parse(data, convertDates === true ? jsonDateConverter : undefined);
                }
                else {
                    data = convertDates === true ?
						data.replace(replaceISO, "new Date(parseInt('$1',10),parseInt('$2',10)-1,parseInt('$3',10),parseInt('$4',10),parseInt('$5',10),parseInt('$6',10),(function(s){return parseInt(s,10)||0;})('$7'))")
							.replace(replaceNet, "new Date($1)") :
						data;
                    return (new Function("return " + data))();
                }
            } else {
                $.error("Invalid JSON: " + data);
            }
        }
    });



})(jQuery);

//jqAuto -- main binding (should contain additional options to pass to autocomplete)
//jqAutoSource -- the array to populate with choices (needs to be an observableArray)
//jqAutoQuery -- function to return choices
//jqAutoValue -- where to write the selected value
//jqAutoSourceLabel -- the property that should be displayed in the possible choices
//jqAutoSourceInputValue -- the property that should be displayed in the input box
//jqAutoSourceValue -- the property to use for the value
ko.bindingHandlers.jqAuto = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        var options = valueAccessor() || {},
            allBindings = allBindingsAccessor(),
            unwrap = ko.utils.unwrapObservable,
            modelValue = allBindings.jqAutoValue,
            source = allBindings.jqAutoSource,
            query = allBindings.jqAutoQuery,
            valueProp = allBindings.jqAutoSourceValue,
            inputValueProp = allBindings.jqAutoSourceInputValue || valueProp,
            labelProp = allBindings.jqAutoSourceLabel || inputValueProp;

        //function that is shared by both select and change event handlers
        function writeValueToModel(valueToWrite) {
            if (ko.isWriteableObservable(modelValue)) {
                modelValue(valueToWrite);
            } else {  //write to non-observable
                if (allBindings['_ko_property_writers'] && allBindings['_ko_property_writers']['jqAutoValue'])
                    allBindings['_ko_property_writers']['jqAutoValue'](valueToWrite);
            }
        }

        //on a selection write the proper value to the model
        options.select = function (event, ui) {
            writeValueToModel(ui.item ? ui.item.actualValue : null);
        };

        //on a change, make sure that it is a valid value or clear out the model value
        options.change = function (event, ui) {
            var currentValue = $(element).val();
            var matchingItem = ko.utils.arrayFirst(unwrap(source), function (item) {
                return unwrap(inputValueProp ? item[inputValueProp] : item) === currentValue;
            });

            if (!matchingItem) {
                writeValueToModel(null);
            }
        }

        //hold the autocomplete current response
        var currentResponse = null;

        //handle the choices being updated in a DO, to decouple value updates from source (options) updates
        var mappedSource = ko.dependentObservable({
            read: function () {
                mapped = ko.utils.arrayMap(unwrap(source), function (item) {
                    var result = {};
                    result.label = labelProp ? unwrap(item[labelProp]) : unwrap(item).toString();  //show in pop-up choices
                    result.value = inputValueProp ? unwrap(item[inputValueProp]) : unwrap(item).toString();  //show in input box
                    result.actualValue = valueProp ? unwrap(item[valueProp]) : item;  //store in model
                    return result;
                });
                return mapped;
            },
            write: function (newValue) {
                source(newValue);  //update the source observableArray, so our mapped value (above) is correct
                if (currentResponse) {
                    currentResponse(mappedSource());
                }
            },
            disposeWhenNodeIsRemoved: element
        });

        if (query) {
            options.source = function (request, response) {
                currentResponse = response;
                query.call(this, request.term, mappedSource);
            }
        } else {
            //whenever the items that make up the source are updated, make sure that autocomplete knows it
            mappedSource.subscribe(function (newValue) {
                $(element).autocomplete("option", "source", newValue);
            });

            options.source = mappedSource();
        }


        //initialize autocomplete
        $(element).autocomplete(options);
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        //update value based on a model change
        var allBindings = allBindingsAccessor(),
           unwrap = ko.utils.unwrapObservable,
           modelValue = unwrap(allBindings.jqAutoValue) || '',
           valueProp = allBindings.jqAutoSourceValue,
           inputValueProp = allBindings.jqAutoSourceInputValue || valueProp;

        //if we are writing a different property to the input than we are writing to the model, then locate the object
        if (valueProp && inputValueProp !== valueProp) {
            var source = unwrap(allBindings.jqAutoSource) || [];
            var modelValue = ko.utils.arrayFirst(source, function (item) {
                return unwrap(item[valueProp]) === modelValue;
            }) || {};
        }

        //update the element with the value that should be shown in the input
        $(element).val(modelValue && inputValueProp !== valueProp ? unwrap(modelValue[inputValueProp]) : modelValue.toString());
    }
};

ko.bindingHandlers.autoComplete = {
    findSelectedItem: function (dataSource, binding, selectedValue) {
        var unwrap = ko.utils.unwrapObservable;
        //Go through the source and find the id, and use its label to set the autocomplete
        var source = unwrap(dataSource);
        var valueProp = unwrap(binding.optionsValue);

        var selectedItem = ko.utils.arrayFirst(source, function (item) {
            if (unwrap(item[valueProp]) === selectedValue)
                return true;
        }, this);

        return selectedItem;
    },
    buildDataSource: function (dataSource, labelProp, valueProp) {
        var unwrap = ko.utils.unwrapObservable;
        var source = unwrap(dataSource);
        var mapped = ko.utils.arrayMap(source, function (item) {
            var result = {};
            result.label = labelProp ? unwrap(item[labelProp]) : unwrap(item).toString();  //show in pop-up choices
            result.value = valueProp ? unwrap(item[valueProp]) : unwrap(item).toString();  //value 
            return result;
        });
        return mapped;
    },
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        var unwrap = ko.utils.unwrapObservable;
        var dataSource = valueAccessor();
        var binding = allBindingsAccessor();
        var valueProp = unwrap(binding.optionsValue);
        var labelProp = unwrap(binding.optionsText) || valueProp;
        var displayId = $(element).attr('id') + '-display';
        var displayElement;
        var options = {};

        if (binding.autoCompleteOptions) {
            options = $.extend(options, binding.autoCompleteOptions);
        }

        //Create a new input to be the autocomplete so that the label shows
        // also hide the original control since it will be used for the value binding
        $(element).hide();
        $(element).after('<input type="text" id="' + displayId + '" />')
        displayElement = $('#' + displayId);

        //handle value changing
        var modelValue = binding.value;
        if (modelValue) {
            var handleValueChange = function (event, ui) {
                var labelToWrite = ui.item ? ui.item.label : null
                var valueToWrite = ui.item ? ui.item.value : null;
                //The Label and Value should not be null, if it is
                // then they did not make a selection so do not update the 
                // ko model
                if (labelToWrite && valueToWrite) {
                    if (ko.isWriteableObservable(modelValue)) {
                        //Since this is an observable, the update part will fire and select the 
                        //  appropriate display values in the controls
                        modelValue(valueToWrite);
                    } else {  //write to non-observable
                        if (binding['_ko_property_writers'] && binding['_ko_property_writers']['value']) {
                            binding['_ko_property_writers']['value'](valueToWrite);
                            //Because this is not an observable, we have to manually change the controls values
                            // since update will not do it for us (it will not fire since it is not observable)
                            displayElement.val(labelToWrite);
                            $(element).val(valueToWrite);
                        }
                    }
                }
                //They did not make a valid selection so change the autoComplete box back to the previous selection
                else {
                    var currentModelValue = unwrap(modelValue);
                    //If the currentModelValue exists and is not nothing, then find out the display
                    // otherwise just blank it out since it is an invalid value
                    if (!currentModelValue)
                        displayElement.val('');
                    else {
                        //Go through the source and find the id, and use its label to set the autocomplete
                        var selectedItem = ko.bindingHandlers.autoComplete.findSelectedItem(dataSource, binding, currentModelValue);

                        //If we found the item then update the display
                        if (selectedItem) {
                            var displayText = labelProp ? unwrap(selectedItem[labelProp]) : unwrap(selectedItem).toString();
                            displayElement.val(displayText);
                        }
                        //if we did not find the item, then just blank it out, because it is an invalid value
                        else {
                            displayElement.val('');
                        }
                    }
                }

                return false;
            };

            var handleFocus = function (event, ui) {
                $(displayElement).val(ui.item.label);
                return false;
            };

            options.change = handleValueChange;
            options.select = handleValueChange;
            options.focus = handleFocus;
            //options.close = handleValueChange;
        }

        //handle the choices being updated in a Dependant Observable (DO), so the update function doesn't 
        // have to do it each time the value is updated. Since we are passing the dataSource in DO, if it is
        // an observable, when you change the dataSource, the dependentObservable will be re-evaluated
        // and its subscribe event will fire allowing us to update the autocomplete datasource
        var mappedSource = ko.dependentObservable(function () {
            return ko.bindingHandlers.autoComplete.buildDataSource(dataSource, labelProp, valueProp);
        }, viewModel);
        //Subscribe to the knockout observable array to get new/remove items
        mappedSource.subscribe(function (newValue) {
            displayElement.autocomplete("option", "source", newValue);
        });

        options.source = mappedSource();

        displayElement.autocomplete(options);
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        //update value based on a model change
        var unwrap = ko.utils.unwrapObservable;
        var dataSource = valueAccessor();
        var binding = allBindingsAccessor();
        var valueProp = unwrap(binding.optionsValue);
        var labelProp = unwrap(binding.optionsText) || valueProp;
        var displayId = $(element).attr('id') + '-display';
        var displayElement = $('#' + displayId);
        var modelValue = binding.value;

        if (modelValue) {
            var currentModelValue = unwrap(modelValue);
            //Set the hidden box to be the same as the viewModels Bound property
            $(element).val(currentModelValue);
            //Go through the source and find the id, and use its label to set the autocomplete
            var selectedItem = ko.bindingHandlers.autoComplete.findSelectedItem(dataSource, binding, currentModelValue);
            if (selectedItem) {
                var displayText = labelProp ? unwrap(selectedItem[labelProp]) : unwrap(selectedItem).toString();
                displayElement.val(displayText);
            }
        }
    }
};

function split(val) {
    return val.split(/,\s*/);
}
function extractLast(term) {
    return split(term).pop();
}

var jsonDateRE = /^\/Date\((-?\d+)(\+|-)?(\d+)?\)\/$/;

var parseJsonDateString = function (value) {
    var arr = value && jsonDateRE.exec(value);
    if (arr) {
        return new Date(parseInt(arr[1]));
    }
    return value;
};

var getShortDate = function (dateValue) {
    var val = ko.utils.unwrapObservable(dateValue);
    if (val) {
        var d = parseJsonDateString(val);
        var dt = new Date(d);
        return dt.toString("MM-dd-yyyy");
    }
    return val;
};

ko.bindingHandlers.datepicker = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        //initialize datepicker with some optional options
        var options = allBindingsAccessor().datepickerOptions || {};
        $(element).datepicker(options);

        //handle the field changing
        ko.utils.registerEventHandler(element, "change", function () {
            var observable = valueAccessor();
            observable($(element).datepicker("getDate"));
        });

        //handle disposal (if KO removes by the template binding)
        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            $(element).datepicker("destroy");
        });

    },
    update: function (element, valueAccessor) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        //console.log("value", value);
        $(element).datepicker("setDate", value);
    }
};

